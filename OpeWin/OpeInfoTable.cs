﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace OpeWin
{
    [Serializable]
    public class OpeInfoTable : DataTable
    {
        private static OpeInfoTable Instance = OpeInfoTable.Load();

        private const string SETTING_FILE_NAME = "OpeWinSettings.xml";
        private const string TABLE_NAME = "OpeWinSettings";

        private OpeInfoTable()
        {
            // Do nothing here!
        }

        public static OpeInfoTable GetInstance()
        {
            return Instance;
        }

        public OpeInfoTable(bool is_first)
        {
            this.Columns.Add("ID", typeof(int));
            this.Columns.Add("Name");
            this.Columns.Add("HotKey");
            this.Columns.Add("HotKeyObject", typeof(HotKey));
            this.Columns.Add("ScriptBody");
            this.Columns.Add("Enabled", typeof(bool));

            this.Columns["Enabled"].DefaultValue = true;

            this.Columns["ID"].Unique = true;
            this.PrimaryKey = new DataColumn[] { this.Columns["ID"] };
        }

        public void AddRowAsDefault()
        {
            DataRow row = this.NewRow();
            row["ID"] = GetNewIdx();
            row["Name"] = "New Ope";
            row["HotKey"] = HotKey.NOT_ASIGNED;
            row["HotKeyObject"] = null;
            row["ScriptBody"] = @"Print(""ID:" + row["ID"].ToString() + @""")";

            this.Rows.Add(row);
        }

        private int GetNewIdx()
        {
            int[] idxs = new int[Rows.Count];

            for(int i = 0; i < Rows.Count; i++)
            {
                idxs[i] = (int)Rows[i]["ID"];
            }

            Array.Sort(idxs);

            for(int i = 0; i < idxs.Length; i++)
            {
                if(idxs[i] != i)
                {
                    return i;
                }
            }
            
            return idxs.Length;
        }

        public void Save()
        {
            this.TableName = TABLE_NAME;
            SaveDataTableToXML(this, SETTING_FILE_NAME);
        }

        private static void SaveDataTableToXML(OpeInfoTable dt, string xmlPath)
        {
            using (FileStream fs
                = new FileStream(xmlPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(dt.GetType());
                serializer.Serialize(fs, dt);
            }
        }

        private static OpeInfoTable Load()
        {
            OpeInfoTable table = null;
            LoadDataTableFromXML(ref table, SETTING_FILE_NAME);

            if (table != null)
            {
                addRuntimeData(ref table);
                return table;
            }
            else
            {
                return new OpeInfoTable(true);
            }
        }

        private static void addRuntimeData(ref OpeInfoTable table)
        {
            if(table.Columns.Contains("Enabled") == false)
            {
                table.Columns.Add("Enabled", typeof(bool));
                table.Columns["Enabled"].DefaultValue = true;

                foreach(DataRow row in table.Rows)
                {
                    row["Enabled"] = true;
                }
            }
        }

        public void RollBackBeforeEditing()
        {
            Instance.Rows.Clear();

            foreach (DataRow dr in Load().Rows)
            {
                Instance.Rows.Add(dr.ItemArray);
            }
        }

        private static void LoadDataTableFromXML(ref OpeInfoTable dt, string xmlPath)
        {
            try
            {
                using (FileStream fs = new FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(OpeInfoTable));
                    dt = (OpeInfoTable)serializer.Deserialize(fs);
                }
            }
            catch(Exception e)
            {
                dt = null;
                return;
            }
        }

        public void RegisterAllOpeToHotKey(IntPtr hWnd)
        {
            bool hasProblem = false;

            try
            {
                foreach (DataRow item in Rows)
                {
                    if(item["HotKeyObject"] == DBNull.Value)
                        continue;
                    if (item["HotKeyObject"] is ComboKey)
                    {
                        bool result = ComboKey.RegisterComboKeys((ComboKey)item["HotKeyObject"]);
                        if (result == false)
                        {
                            Rows[Rows.IndexOf(item)]["Enabled"] = false;
                            hasProblem = true;
                        }
                        else
                        {
                            Rows[Rows.IndexOf(item)]["Enabled"] = true;
                        }
                    }
                    else
                    {
                        bool result = HotKey.RegisterHotKey(
                            hWnd, int.Parse(item["ID"].ToString()),
                            (uint)((HotKey)item["HotKeyObject"]).ModKeys,
                            (uint)(KeyInterop.VirtualKeyFromKey(((HotKey)item["HotKeyObject"]).Key)));

                        if (result == false)
                        {
                            Rows[Rows.IndexOf(item)]["Enabled"] = false;
                            hasProblem = true;
                        }
                        else
                        {
                            Rows[Rows.IndexOf(item)]["Enabled"] = true;
                        }
                    }
                }
            }
            catch(HotKey.HotKeyException exception)
            {
                MessageBox.Show(exception.Message);
            }

            if(hasProblem)
            {
                MessageBox.Show("Some hotkey was failed to register." 
                    + Environment.NewLine
                    + "Please check setting that had been gray-outed.");
            }
        }

        public void UnregisterAllOpeToHotKey(IntPtr hWnd)
        {
            try
            {
                foreach (DataRow item in Rows)
                {
                    if (item["HotkeyObject"] is ComboKey)
                    {
                        ComboKey.UnregisterComboKeys(item["HotkeyObject"] as ComboKey);
                    }
                    else
                    {
                        // We don't care the failure of hotkey unregistration
                        // because there is nothing to do for.
                        HotKey.UnregisterHotKey(hWnd, int.Parse(item["ID"].ToString()));
                    }
                }
            }
            catch (HotKey.HotKeyException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        public void DoOpeScript(int idx)
        {
            OpeScript.GetInstance().DoScript(
                        Rows.Find(idx)["ScriptBody"].ToString(),
                        idx);
        }

        public DataRow GetRowById(int idx)
        {
            return Rows.Find(idx);
        }

        public static void SetHotkey(KeyEventArgs e, ModifierKeys modifierKeys, DataRow row)
        {
            HotKey hot_key = new HotKey();
            if (HotKey.CanSet(e, modifierKeys))
            {
                hot_key.Set(e, modifierKeys);
            }
            else
            {
                return;
            }

            row["HotKey"] = hot_key.MyToString();
            row["HotKeyObject"] = hot_key;
        }

        public static bool SetComboKey(string combo_setting, DataRow row)
        {
            List<int> combo_seq;
            int priorty;

            if (ComboKey.CanSet(combo_setting, out combo_seq, out priorty) == false)
            {
                return false;
            }

            ComboKey combo_key = new ComboKey((int)row["ID"], combo_seq, priorty);

            row["HotKey"] = combo_key.MyToString();
            row["HotKeyObject"] = combo_key;

            return true;
        }
        
        public static void ClearHotKey(DataRow row)
        {
            row["HotKey"] = HotKey.NOT_ASIGNED;
            row["HotKeyObject"] = null;
        }
    }
}
