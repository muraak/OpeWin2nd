using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.calitha.goldparser;
using System.IO;

namespace OpeLang
{

    class Interpreter
    {
        private Context _context;
        private Parser _parser;

        public Interpreter(string cgtPath)
        {
            _parser = new Parser(cgtPath);
            _context = new Context();
        }

        public void Execute(string script)
        {
            try
            {
                object program = _parser.Parse(script);

                if (program is OpeLang.NonTerminal.Program)
                {
                    ((OpeLang.NonTerminal.Program)program).Eval(_context);
                }
                else
                {
                    throw new Exception("Failed to parse.");
                }
            }
            catch(Exception e)
            {
                throw new Exception("Failed to parse.");
            }
        }

        public void ExecuteFile(string path)
        {
            StreamReader sr = new StreamReader(path);
            string script = sr.ReadToEnd();
            sr.Close();

            Execute(script);
        }

        public bool RegisterOpe(MethodInfo method)
        {
            if (_context.Opes.Count == 0)
            {
                _context.Opes.Add(new InstanceMethodInfo() { methodInfo = method, instance = null });
                return true;
            }

            if (_context.Opes.FindIndex(x => { return x.methodInfo.Name == method.Name; }) >= 0)
            {
                return false;
            }

            _context.Opes.Add(new InstanceMethodInfo() { methodInfo = method, instance = null });

            return true;
        }

        public bool RegisterOpe(object instance, MethodInfo method)
        {
            if (_context.Opes.Count == 0)
            {
                _context.Opes.Add(new InstanceMethodInfo() { methodInfo = method, instance = instance });
                return true;
            }

            if (_context.Opes.FindIndex(x => { return x.methodInfo.Name == method.Name; }) >= 0)
            {
                return false;
            }

            _context.Opes.Add(new InstanceMethodInfo() { methodInfo = method, instance = instance });

            return true;
        }

        public bool UnregisterOpe(string name)
        {
            return _context.Opes.Remove(_context.Opes.Find(x => { return x.methodInfo.Name == name; }));
        }
    }

    class Context
    {
        public List<InstanceMethodInfo> Opes = new List<InstanceMethodInfo>();
    }

    class InstanceMethodInfo
    {
        public MethodInfo methodInfo;
        public object instance;
    }

    namespace Terminal
    {
        class UInteger
        {
            private uint _value = 0;

            public UInteger(uint value)
            {
                _value = value;
            }

            public uint Eval(Context context)
            {
                return _value;
            }
        }

        class Double
        {
            private double _value = 0;

            public Double(double value)
            {
                _value = value;
            }

            public double Eval(Context context)
            {
                return _value;
            }
        }

        class Identifier
        {
            private string _value = "";

            public Identifier(string value)
            {
                _value = value;
            }

            public string Eval(Context context)
            {
                return _value;
            }
        }
    }

    namespace NonTerminal
    {
        using Terminal;

        class Program
        {
            public List<Sentence> _values = new List<Sentence>();

            public Program() { }

            public Program(Sentence sentence)
            {
                Add(sentence);
            }

            public void Add(Sentence sentence)
            {
                _values.Add(sentence);
            }

            public void Eval(Context context)
            {
                foreach (Sentence sentence in _values)
                {
                    if (sentence != null)
                        sentence.Eval(context);
                }
            }
        }

        abstract class Sentence
        {
            public abstract object Eval(Context context);
        }

        class Ope : Sentence
        {
            public string _id;
            public Args _args;

            public Ope(string id, Args args)
            {
                _id = id;
                _args = args;
            }

            public override object Eval(Context context)
            {
                if (context == null)
                {
                    throw new Exception("Context is null.");
                }

                if (context.Opes == null)
                {
                    throw new Exception("_opes in context is null.");
                }

                InstanceMethodInfo ope = context.Opes.Find((i) => { return i.methodInfo.Name == this._id; });

                if (ope == null)
                {
                    throw new Exception(string.Format("ope \"{0}\" was not found.", _id));
                }

                return ope.methodInfo.Invoke(ope.instance, _args.Eval(context));
            }
        }

        class Args
        {
            public List<Expression> _values = new List<Expression>();

            public Args() { }

            public Args(Expression expr)
            {
                Add(expr);
            }

            public void Add(Expression expr)
            {
                _values.Add(expr);
            }

            public object[] Eval(Context context)
            {
                IEnumerable<object> args = _values.Select((x) =>
                {
                    return x.isDouble ? (object)x.EvalAsDouble(context) : (object)x.EvalAsInt(context);
                });

                return args.ToArray<object>();
            }
        }

        class IfStatement : Sentence
        {
            private Condition _condition;
            private Program _then;
            private Program _else;
            private List<ElsIfSection> _elsifSections = null;

            public IfStatement(Condition condition, Program then, Program els, List<ElsIfSection> elsIfs)
            {
                _condition = condition;
                _then = then;
                _else = els;
                _elsifSections = elsIfs;
            }

            public IfStatement(Condition condition, Program then, List<ElsIfSection> elsIfs)
            {
                _condition = condition;
                _then = then;
                _elsifSections = elsIfs;
            }

            public override object Eval(Context context)
            {
                if (_condition.Eval(context))
                {
                    _then.Eval(context);
                    return null;
                }

                if (_elsifSections.Count > 0)
                {
                    foreach (ElsIfSection item in _elsifSections)
                    {
                        if (item.Eval(context)) break;
                    }

                    return null;
                }

                if (_else != null)
                {
                    _else.Eval(context);
                    return null;
                }


                return null;
            }
        }

        class ElsIfSection
        {
            private Condition _condition;
            private Program _then;

            public ElsIfSection(Condition condition, Program then)
            {
                _condition = condition;
                _then = then;
            }

            public bool Eval(Context context)
            {
                bool cond = _condition.Eval(context);

                if (cond)
                {
                    _then.Eval(context);
                }

                return cond;
            }
        }

        abstract class Condition
        {
            public abstract bool Eval(Context context);
        }

        class BinaryCondition : Condition
        {
            private Expression _left;
            private Expression _right;
            private string _operator;

            public BinaryCondition(Expression left, Expression right, string ope)
            {
                _left = left;
                _right = right;
                _operator = ope;
            }

            public override bool Eval(Context context)
            {
                if (_operator.Equals("=="))
                    return ((_left.isDouble) ? _left.EvalAsDouble(context) : _left.EvalAsInt(context)) == ((_right.isDouble) ? _right.EvalAsDouble(context) : _right.EvalAsInt(context));
                if (_operator.Equals("!="))
                    return ((_left.isDouble) ? _left.EvalAsDouble(context) : _left.EvalAsInt(context)) != ((_right.isDouble) ? _right.EvalAsDouble(context) : _right.EvalAsInt(context));
                if (_operator.Equals("<"))
                    return ((_left.isDouble) ? _left.EvalAsDouble(context) : _left.EvalAsInt(context)) < ((_right.isDouble) ? _right.EvalAsDouble(context) : _right.EvalAsInt(context));
                if (_operator.Equals(">"))
                    return ((_left.isDouble) ? _left.EvalAsDouble(context) : _left.EvalAsInt(context)) > ((_right.isDouble) ? _right.EvalAsDouble(context) : _right.EvalAsInt(context));
                if (_operator.Equals("<="))
                    return ((_left.isDouble) ? _left.EvalAsDouble(context) : _left.EvalAsInt(context)) <= ((_right.isDouble) ? _right.EvalAsDouble(context) : _right.EvalAsInt(context));
                if (_operator.Equals(">="))
                    return ((_left.isDouble) ? _left.EvalAsDouble(context) : _left.EvalAsInt(context)) >= ((_right.isDouble) ? _right.EvalAsDouble(context) : _right.EvalAsInt(context));

                throw new Exception();
            }
        }

        abstract class Expression
        {
            public abstract int EvalAsInt(Context context);
            public abstract double EvalAsDouble(Context context);
            public virtual bool isDouble { get; set; }
        }

        class AddExpression : Expression
        {
            private Expression _expr;
            private Term _term;

            public AddExpression(Expression expr, Term term)
            {
                _expr = expr;
                _term = term;
            }

            public override bool isDouble { get { return _expr.isDouble || _term.isDouble; } }

            public override int EvalAsInt(Context context)
            {
                return _expr.EvalAsInt(context) + _term.EvalAsInt(context);
            }

            public override double EvalAsDouble(Context context)
            {
                return ((_expr.isDouble) ? _expr.EvalAsDouble(context) : _expr.EvalAsInt(context)) + ((_term.isDouble) ? _term.EvalAsDouble(context) : _term.EvalAsInt(context));
            }


        }

        class SubExpression : Expression
        {
            private Expression _expr;
            private Term _term;

            public SubExpression(Expression expr, Term term)
            {
                _expr = expr;
                _term = term;
            }

            public override bool isDouble { get { return _expr.isDouble || _term.isDouble; } }

            public override int EvalAsInt(Context context)
            {
                return _expr.EvalAsInt(context) - _term.EvalAsInt(context);
            }

            public override double EvalAsDouble(Context context)
            {
                return ((_expr.isDouble) ? _expr.EvalAsDouble(context) : _expr.EvalAsInt(context)) - ((_term.isDouble) ? _term.EvalAsDouble(context) : _term.EvalAsInt(context));
            }
        }

        abstract class Term : Expression
        {
        }

        class DivTerm : Term
        {
            private Num _num;
            private Term _term;

            public DivTerm(Term term, Num num)
            {
                _term = term;
                _num = num;
            }

            // Div opertion always returns double value.
            // I'm planning to add IntDiv operation as operator `//` like lua.
            //public override bool isDouble { get { return _term.isDouble || _num.isDouble; } }
            public override bool isDouble { get { return true; } }

            public override int EvalAsInt(Context context)
            {
                return _term.EvalAsInt(context) / _num.EvalAsInt(context);
            }

            public override double EvalAsDouble(Context context)
            {
                return ((_term.isDouble) ? _term.EvalAsDouble(context) : _term.EvalAsInt(context)) / ((_num.isDouble) ? _num.EvalAsDouble(context) : _num.EvalAsInt(context));
            }
        }

        class MulTerm : Term
        {
            private Num _num;
            private Term _term;

            public override bool isDouble { get { return _term.isDouble || _num.isDouble; } }

            public MulTerm(Term term, Num num)
            {
                _term = term;
                _num = num;
            }

            public override int EvalAsInt(Context context)
            {
                return _term.EvalAsInt(context) * _num.EvalAsInt(context);
            }

            public override double EvalAsDouble(Context context)
            {
                return ((_term.isDouble) ? _term.EvalAsDouble(context) : _term.EvalAsInt(context)) * ((_num.isDouble) ? _num.EvalAsDouble(context) : _num.EvalAsInt(context));
            }
        }

        abstract class Num : Term
        {
        }

        class PositiveNum : Num
        {
            private UInteger _value_uint = null;
            private Double _value_double = null;

            public override bool isDouble { get; set; } = false;

            public PositiveNum(Object value)
            {
                if (value is UInteger)
                {
                    _value_uint = (UInteger)value;
                }

                if (value is Double)
                {
                    _value_double = (Double)value;
                    isDouble = true;
                }

            }

            public override int EvalAsInt(Context context)
            {
                return (int)_value_uint.Eval(context);
            }

            public override double EvalAsDouble(Context context)
            {
                return _value_double.Eval(context);
            }

        }

        class NegativeNum : Num
        {
            private UInteger _value_uint = null;
            private Double _value_double = null;

            public override bool isDouble { get; set; } = false;

            public NegativeNum(object value)
            {
                if (value is UInteger)
                {
                    _value_uint = (UInteger)value;
                }

                if (value is Double)
                {
                    _value_double = (Double)value;
                    isDouble = true;
                }
            }

            public override int EvalAsInt(Context context)
            {
                return (-1*(int)_value_uint.Eval(context));
            }

            public override double EvalAsDouble(Context context)
            {
                return (-1 * _value_double.Eval(context));
            }
        }

        class NonVoidOpe : PositiveNum
        {
            private Ope _adaptee;
            private bool _isNegative;

            public override bool isDouble { get { return _isDouble(); } }

            public NonVoidOpe(object adaptee, bool isNegative) : base(null)
            {
                if (adaptee is Ope)
                    _adaptee = (Ope)adaptee;

                _isNegative = isNegative;

            }

            public override int EvalAsInt(Context context)
            {
                return (int)_adaptee.Eval(context);
            }

            public override double EvalAsDouble(Context context)
            {
                return (double)_adaptee.Eval(context);
            }

            private bool _isDouble()
            {
                // TODO
                return false;
            }

            public bool isNonVoid(string ope_name)
            {
                // TODO
                return false;
            }
        }
    }
}
