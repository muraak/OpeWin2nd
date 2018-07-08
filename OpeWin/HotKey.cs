using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OpeWin
{
    [Serializable]
    public class HotKey : IXmlSerializable
    {
        public const string NOT_ASIGNED = "not asigned";

        public int __mod_keys;

        public int __key;

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

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            MyReadXml(reader);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            MyWriteXml(writer);
        }

        protected virtual void MyReadXml(XmlReader reader)
        {
            try
            {
                __mod_keys = int.Parse(reader["__mod_keys"]);
                __key = int.Parse(reader["__key"]);
            }
            catch
            {
                if (reader.ReadToDescendant("__mod_keys"))
                {
                    __mod_keys = reader.ReadElementContentAsInt("__mod_keys", reader.NamespaceURI);
                    __key = reader.ReadElementContentAsInt("__key", reader.NamespaceURI);
                    reader.Read();

                }
            }
        }

        protected virtual void MyWriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("__mod_keys", __mod_keys.ToString());
            writer.WriteAttributeString("__key", __key.ToString());
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
            return (e.Key == Key.Delete);
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

        public virtual string MyToString()
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

    [Serializable]
    public class ComboKey : HotKey
    {
        private static int[] RecentHotkeyIds = new int[5] {-1,-1,-1,-1,-1}; // from most recent to older
        private static List<ComboKey> ComboList = new List<ComboKey>();

        private int Id;
        private List<int> ComboSequence = new List<int>();
        private int Priority = 0;
        private bool isFiring = false;

        protected override void MyReadXml(XmlReader reader)
        {
            Id = int.Parse(reader["Id"]);
            Priority = int.Parse(reader["Priority"]);

            if (reader.ReadToDescendant("Combo"))
            {
                int i = 0;
                while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Combo")
                {
                    ComboSequence.Add(int.Parse(reader["Id"]));
                    reader.Read();
                }
            }
        }

        protected override void MyWriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Id", Id.ToString());
            writer.WriteAttributeString("Priority", Priority.ToString());

            for (int i = 0; i < ComboSequence.Count; i++)
            {
                writer.WriteStartElement("Combo");
                writer.WriteAttributeString("Id", ComboSequence[i].ToString());
                writer.WriteEndElement();
            }

        }

        public ComboKey()
        {

        }

        public ComboKey(int id, List<int> combo_sequence, int priority)
        {
            ComboSequence = combo_sequence;
            Priority = priority;
            Id = id;
        }

        public override string MyToString()
        {
            string result = "combo({";

            for (int i = 0; i < ComboSequence.Count; i++)
            {
                if (i != 0)
                {
                    result += ", ";
                }

                result += ComboSequence[i];
       
            }

            result += "}, "+ Priority + ")";

            return result;
        }

        public static bool RegisterComboKeys(ComboKey combo_key)
        {
            if (ComboList.Find(x => x.Priority == combo_key.Priority) != null)
                return false;

            ComboList.Add(combo_key);

            ComboList.OrderBy(x => -x.Priority);

            return true;
        }

        public static bool UnregisterComboKeys(ComboKey combo)
        {
            return ComboList.Remove(combo);
        }

        public static bool CanSet(string combo_setting, out List<int> combo_sequence, out int priority)
        {
            combo_sequence = null;
            priority = 0;

            const string COMBO_SETTING_SYNTAX = @"\s*combo\s*\(\s*\{(?<SEQ>.+)\}\s*,\s*(?<PRIORITY>.+)\s*\)";

            System.Text.RegularExpressions.MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(combo_setting, COMBO_SETTING_SYNTAX);

            if (matches.Count == 0)
                return false;

            try
            {
                combo_sequence = matches[0].Groups["SEQ"].Value.Split(',').Select(x => int.Parse(x)).ToList();
            }
            catch
            {
                return false;
            }

            try
            {
                priority = int.Parse(matches[0].Groups["PRIORITY"].Value);
            }
            catch
            {
                return false;
            }
            
            if((combo_sequence.Count > 5) || (combo_sequence.Count == 0))
            {
                return false;
            }

            return true;
        }

        public static int GetNextHighestPriority()
        {
            int max_priority = 0;
            
            foreach(DataRow r in OpeInfoTable.GetInstance().Rows)
            {
                if(r["HotKeyObject"] is ComboKey)
                {
                    int tmp = ((ComboKey)r["HotKeyObject"]).Priority;

                    if (tmp > max_priority) max_priority = tmp;
                }
            }

            return max_priority + 1;
        }

        public static bool FindComboToFire(int new_hotkey_id, out int combo_key_id_to_fire)
        {
            combo_key_id_to_fire = -1;

            DateTime now = DateTime.Now;

            UpdateRecentHotkeyIds(new_hotkey_id, now);

            bool found = false;

            if (IsExpired(now) == false)
            {
                for (int i = 0; i < ComboList.Count; i++)
                {
                    if (ComboList[i].ShouldFire() && (found == false))
                    {
                        combo_key_id_to_fire = ComboList[i].Id;
                        ComboList[i].isFiring = true;
                        found = true;
                    }
                    else
                    {
                        ComboList[i].isFiring = false;
                    }
                }


            }
            else
            {
                foreach(ComboKey combo in ComboList) { combo.isFiring = false; }
                found = false;
            }

            LastUpdateDateTime = now;

            return found;
        }

        static DateTime LastUpdateDateTime = new DateTime();
        private static void UpdateRecentHotkeyIds(int new_hotkey_id, DateTime now)
        {
            if(IsExpired(now))
            {
                // Clear except for first element
                for(int i = 1; i < RecentHotkeyIds.Length; i++) { RecentHotkeyIds[i] = -1; }
                RecentHotkeyIds[0] = new_hotkey_id;
                return;
            }

            // shift right to RecentHotkeyIds to push new key to first elem.
            // before: [0][1][2][3][4]
            // after:  [new][0][1][2][3] (elem[4] is removed from this list.)
            for (int i = (RecentHotkeyIds.Length - 2); i >= 0 ; i--)
            {
                RecentHotkeyIds[i + 1] = RecentHotkeyIds[i];
            }
            // now: [0][0][1][2][3]
            RecentHotkeyIds[0] = new_hotkey_id;
            // now: [new][0][1][2][3]!!
        }

        public bool ShouldFire()
        {
            if(ComboSequence.Last() != RecentHotkeyIds[0])
            {
                return false;
            }
            
            if(isFiring)
            {
                return true;
            }

            int last_idx_of_combo_sequence = ComboSequence.Count - 1;
            for (int i = 0; i < ComboSequence.Count; i++)
            {
                if(ComboSequence[last_idx_of_combo_sequence - i] != RecentHotkeyIds[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsExpired(DateTime now)
        {
            TimeSpan span = now - LastUpdateDateTime;

            if (span.TotalMilliseconds > 500.0d)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

