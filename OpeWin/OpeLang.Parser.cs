
using System;
using System.IO;
using System.Runtime.Serialization;
using com.calitha.goldparser.lalr;
using com.calitha.commons;
using System.Collections.Generic;

namespace com.calitha.goldparser
{

    [Serializable()]
    public class SymbolException : System.Exception
    {
        public SymbolException(string message) : base(message)
        {
        }

        public SymbolException(string message,
            Exception inner) : base(message, inner)
        {
        }

        protected SymbolException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

    }

    [Serializable()]
    public class RuleException : System.Exception
    {

        public RuleException(string message) : base(message)
        {
        }

        public RuleException(string message,
                             Exception inner) : base(message, inner)
        {
        }

        protected RuleException(SerializationInfo info,
                                StreamingContext context) : base(info, context)
        {
        }

    }

    enum SymbolConstants : int
    {
        SYMBOL_EOF = 0, // (EOF)
        SYMBOL_ERROR = 1, // (Error)
        SYMBOL_WHITESPACE = 2, // Whitespace
        SYMBOL_MINUS = 3, // '-'
        SYMBOL_EXCLAMEQ = 4, // '!='
        SYMBOL_LPAREN = 5, // '('
        SYMBOL_RPAREN = 6, // ')'
        SYMBOL_TIMES = 7, // '*'
        SYMBOL_COMMA = 8, // ','
        SYMBOL_DIV = 9, // '/'
        SYMBOL_PLUS = 10, // '+'
        SYMBOL_LT = 11, // '<'
        SYMBOL_LTEQ = 12, // '<='
        SYMBOL_EQEQ = 13, // '=='
        SYMBOL_GT = 14, // '>'
        SYMBOL_GTEQ = 15, // '>='
        SYMBOL_DOUBLE = 16, // Double
        SYMBOL_ELSE = 17, // else
        SYMBOL_ELSIF = 18, // elsif
        SYMBOL_ENDIF = 19, // endif
        SYMBOL_IDENTIFIER = 20, // Identifier
        SYMBOL_IF = 21, // if
        SYMBOL_NEWLINE = 22, // NewLine
        SYMBOL_THEN = 23, // then
        SYMBOL_UINTEGER = 24, // UInteger
        SYMBOL_ARGS = 25, // <Args>
        SYMBOL_CONDITION = 26, // <Condition>
        SYMBOL_ELSEIF = 27, // <ElseIf>
        SYMBOL_EXPRESSION = 28, // <Expression>
        SYMBOL_IFSTATMENT = 29, // <IfStatment>
        SYMBOL_NL = 30, // <nl>
        SYMBOL_NLOPT = 31, // <nl Opt>
        SYMBOL_NUM = 32, // <Num>
        SYMBOL_OPE = 33, // <Ope>
        SYMBOL_POSITIVENUM = 34, // <PositiveNum>
        SYMBOL_PROGRAM = 35, // <Program>
        SYMBOL_SENTENCE = 36, // <Sentence>
        SYMBOL_START = 37, // <Start>
        SYMBOL_TERM = 38  // <Term>
    };

    enum RuleConstants : int
    {
        RULE_NL_NEWLINE = 0, // <nl> ::= NewLine <nl>
        RULE_NL_NEWLINE2 = 1, // <nl> ::= NewLine
        RULE_NLOPT_NEWLINE = 2, // <nl Opt> ::= NewLine <nl Opt>
        RULE_NLOPT = 3, // <nl Opt> ::= 
        RULE_START = 4, // <Start> ::= <nl Opt> <Program>
        RULE_PROGRAM = 5, // <Program> ::= <Sentence>
        RULE_PROGRAM2 = 6, // <Program> ::= <Program> <Sentence>
        RULE_SENTENCE = 7, // <Sentence> ::= <nl>
        RULE_SENTENCE2 = 8, // <Sentence> ::= <Ope> <nl>
        RULE_SENTENCE3 = 9, // <Sentence> ::= <IfStatment>
        RULE_OPE_IDENTIFIER_LPAREN_RPAREN = 10, // <Ope> ::= Identifier '(' <Args> ')'
        RULE_ARGS = 11, // <Args> ::= <Expression>
        RULE_ARGS_COMMA = 12, // <Args> ::= <Args> ',' <Expression>
        RULE_ARGS2 = 13, // <Args> ::= 
        RULE_IFSTATMENT_IF_THEN_ENDIF = 14, // <IfStatment> ::= if <Condition> then <Program> <ElseIf> endif
        RULE_IFSTATMENT_IF_THEN_ELSE_ENDIF = 15, // <IfStatment> ::= if <Condition> then <Program> <ElseIf> else <Program> endif
        RULE_ELSEIF_ELSIF_THEN = 16, // <ElseIf> ::= elsif <Condition> then <Program>
        RULE_ELSEIF_ELSIF_THEN2 = 17, // <ElseIf> ::= <ElseIf> elsif <Condition> then <Program>
        RULE_ELSEIF = 18, // <ElseIf> ::= 
        RULE_CONDITION_EQEQ = 19, // <Condition> ::= <Expression> '==' <Expression>
        RULE_CONDITION_EXCLAMEQ = 20, // <Condition> ::= <Expression> '!=' <Expression>
        RULE_CONDITION_GTEQ = 21, // <Condition> ::= <Expression> '>=' <Expression>
        RULE_CONDITION_LTEQ = 22, // <Condition> ::= <Expression> '<=' <Expression>
        RULE_CONDITION_GT = 23, // <Condition> ::= <Expression> '>' <Expression>
        RULE_CONDITION_LT = 24, // <Condition> ::= <Expression> '<' <Expression>
        RULE_EXPRESSION_PLUS = 25, // <Expression> ::= <Expression> '+' <Term>
        RULE_EXPRESSION_MINUS = 26, // <Expression> ::= <Expression> '-' <Term>
        RULE_EXPRESSION = 27, // <Expression> ::= <Term>
        RULE_TERM_TIMES = 28, // <Term> ::= <Term> '*' <Num>
        RULE_TERM_DIV = 29, // <Term> ::= <Term> '/' <Num>
        RULE_TERM = 30, // <Term> ::= <Num>
        RULE_NUM_MINUS = 31, // <Num> ::= '-' <PositiveNum>
        RULE_NUM = 32, // <Num> ::= <PositiveNum>
        RULE_NUM_MINUS2 = 33, // <Num> ::= '-' <Ope>
        RULE_NUM2 = 34, // <Num> ::= <Ope>
        RULE_POSITIVENUM_UINTEGER = 35, // <PositiveNum> ::= UInteger
        RULE_POSITIVENUM_DOUBLE = 36  // <PositiveNum> ::= Double
    };

    public class Parser
    {
        private LALRParser parser;

        public Parser(string filename)
        {
            FileStream stream = new FileStream(filename,
                                               FileMode.Open,
                                               FileAccess.Read,
                                               FileShare.Read);
            Init(stream);
            stream.Close();
        }

        public Parser(string baseName, string resourceName)
        {
            byte[] buffer = ResourceUtil.GetByteArrayResource(
                System.Reflection.Assembly.GetExecutingAssembly(),
                baseName,
                resourceName);
            MemoryStream stream = new MemoryStream(buffer);
            Init(stream);
            stream.Close();
        }

        public Parser(Stream stream)
        {
            Init(stream);
        }

        private void Init(Stream stream)
        {
            CGTReader reader = new CGTReader(stream);
            parser = reader.CreateNewParser();
            parser.TrimReductions = false;
            parser.StoreTokens = LALRParser.StoreTokensMode.NoUserObject;

            parser.OnReduce += new LALRParser.ReduceHandler(ReduceEvent);
            parser.OnTokenRead += new LALRParser.TokenReadHandler(TokenReadEvent);
            parser.OnAccept += new LALRParser.AcceptHandler(AcceptEvent);
            parser.OnTokenError += new LALRParser.TokenErrorHandler(TokenErrorEvent);
            parser.OnParseError += new LALRParser.ParseErrorHandler(ParseErrorEvent);
        }

        public object Parse(string source)
        {
            return parser.Parse(source).UserObject;
        }

        private void TokenReadEvent(LALRParser parser, TokenReadEventArgs args)
        {
            try
            {
                args.Token.UserObject = CreateObject(args.Token);
            }
            catch (Exception e)
            {
                args.Continue = false;
                //todo: Report message to UI?
            }
        }

        private Object CreateObject(TerminalToken token)
        {
            switch (token.Symbol.Id)
            {
                case (int)SymbolConstants.SYMBOL_EOF:
                    //(EOF)
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_ERROR:
                    //(Error)
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_WHITESPACE:
                    //Whitespace
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_MINUS:
                    //'-'
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_EXCLAMEQ:
                    //'!='
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_LPAREN:
                    //'('
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_RPAREN:
                    //')'
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_TIMES:
                    //'*'
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_COMMA:
                    //','
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_DIV:
                    //'/'
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_PLUS:
                    //'+'
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_LT:
                    //'<'
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_LTEQ:
                    //'<='
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_EQEQ:
                    //'=='
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_GT:
                    //'>'
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_GTEQ:
                    //'>='
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_DOUBLE:
                    //Double
                    //todo: Create a new object that corresponds to the symbol
                    return new OpeLang.Terminal.Double(double.Parse(token.Text));

                case (int)SymbolConstants.SYMBOL_ELSE:
                    //else
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_ELSIF:
                    //elsif
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_ENDIF:
                    //endif
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_IDENTIFIER:
                    //Identifier
                    //todo: Create a new object that corresponds to the symbol
                    return new OpeLang.Terminal.Identifier(token.Text);

                case (int)SymbolConstants.SYMBOL_IF:
                    //if
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_NEWLINE:
                    //NewLine
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_THEN:
                    //then
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_UINTEGER:
                    //UInteger
                    //todo: Create a new object that corresponds to the symbol
                    return new OpeLang.Terminal.UInteger(uint.Parse(token.Text));

                case (int)SymbolConstants.SYMBOL_ARGS:
                    //<Args>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_CONDITION:
                    //<Condition>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_ELSEIF:
                    //<ElseIf>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_EXPRESSION:
                    //<Expression>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_IFSTATMENT:
                    //<IfStatment>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_NL:
                    //<nl>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_NLOPT:
                    //<nl Opt>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_NUM:
                    //<Num>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_OPE:
                    //<Ope>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_POSITIVENUM:
                    //<PositiveNum>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_PROGRAM:
                    //<Program>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_SENTENCE:
                    //<Sentence>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_START:
                    //<Start>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

                case (int)SymbolConstants.SYMBOL_TERM:
                    //<Term>
                    //todo: Create a new object that corresponds to the symbol
                    return null;

            }
            throw new SymbolException("Unknown symbol");
        }

        private void ReduceEvent(LALRParser parser, ReduceEventArgs args)
        {
            try
            {
                args.Token.UserObject = CreateObject(args.Token);
            }
            catch (Exception e)
            {
                args.Continue = false;
                //todo: Report message to UI?
            }
        }

        public static Object CreateObject(NonterminalToken token)
        {
            switch (token.Rule.Id)
            {
                case (int)RuleConstants.RULE_NL_NEWLINE:
                    //<nl> ::= NewLine <nl>
                    //todo: Create a new object using the stored user objects.
                    return null;

                case (int)RuleConstants.RULE_NL_NEWLINE2:
                    //<nl> ::= NewLine
                    //todo: Create a new object using the stored user objects.
                    return null;

                case (int)RuleConstants.RULE_NLOPT_NEWLINE:
                    //<nl Opt> ::= NewLine <nl Opt>
                    //todo: Create a new object using the stored user objects.
                    return null;

                case (int)RuleConstants.RULE_NLOPT:
                    //<nl Opt> ::= 
                    //todo: Create a new object using the stored user objects.
                    return null;

                case (int)RuleConstants.RULE_START:
                    //<Start> ::= <nl Opt> <Program>
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[1].UserObject;

                case (int)RuleConstants.RULE_PROGRAM:
                    //<Program> ::= <Sentence>
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.Program((OpeLang.NonTerminal.Sentence)token.Tokens[0].UserObject);

                case (int)RuleConstants.RULE_PROGRAM2:
                    //<Program> ::= <Program> <Sentence>
                    //todo: Create a new object using the stored user objects.
                    ((OpeLang.NonTerminal.Program)token.Tokens[0].UserObject).Add((OpeLang.NonTerminal.Sentence)token.Tokens[1].UserObject);
                    return (OpeLang.NonTerminal.Program)token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_SENTENCE:
                    //<Sentence> ::= <nl>
                    //todo: Create a new object using the stored user objects.
                    return null;

                case (int)RuleConstants.RULE_SENTENCE2:
                    //<Sentence> ::= <Ope> <nl>
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_SENTENCE3:
                    //<Sentence> ::= <IfStatment>
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_OPE_IDENTIFIER_LPAREN_RPAREN:
                    //<Ope> ::= Identifier '(' <Args> ')'
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.Ope(
                        token.Tokens[0].ToString(),
                        (OpeLang.NonTerminal.Args)token.Tokens[2].UserObject);

                case (int)RuleConstants.RULE_ARGS:
                    //<Args> ::= <Expression>
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.Args((OpeLang.NonTerminal.Expression)token.Tokens[0].UserObject);

                case (int)RuleConstants.RULE_ARGS_COMMA:
                    //<Args> ::= <Args> ',' <Expression>
                    //todo: Create a new object using the stored user objects.
                    ((OpeLang.NonTerminal.Args)token.Tokens[0].UserObject).Add((OpeLang.NonTerminal.Expression)token.Tokens[2].UserObject);
                    return (OpeLang.NonTerminal.Args)token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_ARGS2:
                    //<Args> ::= 
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.Args();

                case (int)RuleConstants.RULE_IFSTATMENT_IF_THEN_ENDIF:
                    //<IfStatment> ::= if <Condition> then <Program> <ElseIf> endif
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.IfStatement(
                        (OpeLang.NonTerminal.Condition)token.Tokens[1].UserObject,
                        (OpeLang.NonTerminal.Program)token.Tokens[3].UserObject,
                        (List<OpeLang.NonTerminal.ElsIfSection>)token.Tokens[4].UserObject);

                case (int)RuleConstants.RULE_IFSTATMENT_IF_THEN_ELSE_ENDIF:
                    //<IfStatment> ::= if <Condition> then <Program> <ElseIf> else <Program> endif
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.IfStatement(
                        (OpeLang.NonTerminal.Condition)token.Tokens[1].UserObject,
                        (OpeLang.NonTerminal.Program)token.Tokens[3].UserObject,
                        (OpeLang.NonTerminal.Program)token.Tokens[5].UserObject,
                        (List<OpeLang.NonTerminal.ElsIfSection>)token.Tokens[4].UserObject);

                case (int)RuleConstants.RULE_ELSEIF_ELSIF_THEN:
                    //<ElseIf> ::= elsif <Condition> then <Program>
                    //todo: Create a new object using the stored user objects.
                    return new List<OpeLang.NonTerminal.ElsIfSection>()
                    { new OpeLang.NonTerminal.ElsIfSection((OpeLang.NonTerminal.Condition)token.Tokens[1].UserObject,
                                                                (OpeLang.NonTerminal.Program)token.Tokens[3].UserObject) };

                case (int)RuleConstants.RULE_ELSEIF_ELSIF_THEN2:
                    //<ElseIf> ::= <ElseIf> elsif <Condition> then <Program>
                    //todo: Create a new object using the stored user objects.
                    ((List<OpeLang.NonTerminal.ElsIfSection>)token.Tokens[0].UserObject).Add(
                        new OpeLang.NonTerminal.ElsIfSection(
                            (OpeLang.NonTerminal.Condition)token.Tokens[2].UserObject,
                            (OpeLang.NonTerminal.Program)token.Tokens[4].UserObject));
                    return token.Tokens[0].UserObject;
                case (int)RuleConstants.RULE_ELSEIF:
                    //<ElseIf> ::= 
                    //todo: Create a new object using the stored user objects.
                    return null;

                case (int)RuleConstants.RULE_CONDITION_EQEQ:
                case (int)RuleConstants.RULE_CONDITION_EXCLAMEQ:
                case (int)RuleConstants.RULE_CONDITION_GTEQ:
                case (int)RuleConstants.RULE_CONDITION_LTEQ:
                case (int)RuleConstants.RULE_CONDITION_GT:
                case (int)RuleConstants.RULE_CONDITION_LT:
                    //<Condition> ::= <Expression> '<' <Expression>
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.BinaryCondition(
                        (OpeLang.NonTerminal.Expression)token.Tokens[0].UserObject,
                        (OpeLang.NonTerminal.Expression)token.Tokens[2].UserObject,
                        (string)token.Tokens[1].ToString());

                case (int)RuleConstants.RULE_EXPRESSION_PLUS:
                    //<Expression> ::= <Expression> '+' <Term>
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.AddExpression(
                        (OpeLang.NonTerminal.Expression)token.Tokens[0].UserObject,
                        (OpeLang.NonTerminal.Term)token.Tokens[2].UserObject);

                case (int)RuleConstants.RULE_EXPRESSION_MINUS:
                    //<Expression> ::= <Expression> '-' <Term>
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.SubExpression(
                        (OpeLang.NonTerminal.Expression)token.Tokens[0].UserObject,
                        (OpeLang.NonTerminal.Term)token.Tokens[2].UserObject);

                case (int)RuleConstants.RULE_EXPRESSION:
                    //<Expression> ::= <Term>
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_TERM_TIMES:
                    //<Term> ::= <Term> '*' <Num>
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.MulTerm(
                        (OpeLang.NonTerminal.Term)token.Tokens[0].UserObject,
                        (OpeLang.NonTerminal.Num)token.Tokens[2].UserObject);

                case (int)RuleConstants.RULE_TERM_DIV:
                    //<Term> ::= <Term> '/' <Num>
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.DivTerm(
                        (OpeLang.NonTerminal.Term)token.Tokens[0].UserObject,
                        (OpeLang.NonTerminal.Num)token.Tokens[2].UserObject);

                case (int)RuleConstants.RULE_TERM:
                    //<Term> ::= <Num>
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_NUM_MINUS:
                    //<Num> ::= '-' <PositiveNum>
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.NegativeNum(token.Tokens[0].UserObject);

                case (int)RuleConstants.RULE_NUM:
                    //<Num> ::= <PositiveNum>
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.PositiveNum(token.Tokens[0].UserObject);

                case (int)RuleConstants.RULE_NUM_MINUS2:
                    //<Num> ::= '-' <Ope>
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.NonVoidOpe(token.Tokens[0].UserObject, true/* isNegative */);

                case (int)RuleConstants.RULE_NUM2:
                    //<Num> ::= <Ope>
                    //todo: Create a new object using the stored user objects.
                    return new OpeLang.NonTerminal.NonVoidOpe(token.Tokens[0].UserObject, false/* isNegative */);

                case (int)RuleConstants.RULE_POSITIVENUM_UINTEGER:
                    //<PositiveNum> ::= UInteger
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

                case (int)RuleConstants.RULE_POSITIVENUM_DOUBLE:
                    //<PositiveNum> ::= Double
                    //todo: Create a new object using the stored user objects.
                    return token.Tokens[0].UserObject;

            }
            throw new RuleException("Unknown rule");
        }

        private void AcceptEvent(LALRParser parser, AcceptEventArgs args)
        {
            //todo: Use your fully reduced args.Token.UserObject
        }

        private void TokenErrorEvent(LALRParser parser, TokenErrorEventArgs args)
        {
            string message = "Token error with input: '" + args.Token.ToString() + "'";
            //todo: Report message to UI?
        }

        private void ParseErrorEvent(LALRParser parser, ParseErrorEventArgs args)
        {
            string message = "Parse error caused by token: '" + args.UnexpectedToken.ToString() + "'";
            //todo: Report message to UI?
        }


    }
}
