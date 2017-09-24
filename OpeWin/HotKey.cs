using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using System.Xml.Serialization;

namespace OpeWin
{
    [Serializable]
    public class HotKey
    {
        public const string NOT_ASIGNED = "not asigned";

        public int __mod_keys;

        public int __key;

        [XmlIgnore]
        public ModifierKeys ModKeys
        {
            get
            {
                return (ModifierKeys)__mod_keys;
            }

            set
            {
                __mod_keys = (int)value;
            }
        }

        [XmlIgnore]
        public Key Key
        {
            get
            {
                return (Key)__key;
            }

            set
            {
                __key = (int)value;
            }
        }

        public static bool CanSet(KeyEventArgs key_event_args, ModifierKeys mod_keys)
        {
            if (mod_keys == ModifierKeys.None)
                return false;

            Key key = (key_event_args.Key == Key.System ? key_event_args.SystemKey : key_event_args.Key);

            String key_str = key.ToString();

            if (key_str == ""
                || key.ToString().Contains("Ctrl")
                || key.ToString().Contains("Shift")
                || key.ToString().Contains("Alt")
                || key.ToString().Contains("Win"))
                return false;

            return true;
        }

        public static bool IsClearKey(KeyEventArgs e)
        {
            return (e.Key == Key.Escape);
        }

        public void Set(KeyEventArgs key_event_args, ModifierKeys mod_keys)
        {
            if (CanSet(key_event_args, mod_keys) == true)
            {

                this.ModKeys = mod_keys;
                this.Key = (key_event_args.Key == Key.System ?
                    key_event_args.SystemKey : key_event_args.Key);
            }
            else
            {
                Exception e = new Exception("Can't set to HotKey.");
            }
        }

        public string MyToString()
        {
            string key_str = "";

            if ((ModKeys & ModifierKeys.Alt) != ModifierKeys.None)
                key_str += "Alt + ";
            if ((ModKeys & ModifierKeys.Control) != ModifierKeys.None)
                key_str += "Ctrl + ";
            if ((ModKeys & ModifierKeys.Shift) != ModifierKeys.None)
                key_str += "Shift + ";
            if ((ModKeys & ModifierKeys.Windows) != ModifierKeys.None)
                key_str += "Win + ";

            key_str += (" " + Key.ToString());

            return key_str;
        }

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public class HotKeyException : Exception
        {
            public HotKeyException(string msg) : base(msg)
            {
            }
        }
    }
}

