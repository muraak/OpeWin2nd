using System;
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

        private const int MAX_NUM_OF_OPE = 9;
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

            this.Columns["ID"].Unique = true;
            this.PrimaryKey = new DataColumn[] { this.Columns["ID"] };

            for (int idx = 0; idx < MAX_NUM_OF_OPE; idx++)
            {
                DataRow row = this.NewRow();
                row["ID"] = idx + 1;
                row["Name"] = "Ope" + (idx + 1).ToString();
                row["HotKey"] = "None";
                row["HotKeyObject"] = null;
                row["ScriptBody"] = @"Print(""Ope" + (idx + 1) + @""")";

                this.Rows.Add(row);
            }
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
                return table;
            }
            else
            {
                return new OpeInfoTable(true);
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
            catch (Exception exception)
            {
                dt = null;
                return;
            }
        }

        public void RegisterAllOpeToHotKey(IntPtr hWnd)
        {
            try
            {
                foreach (DataRow item in Rows)
                {
                    bool result = HotKey.RegisterHotKey(
                        hWnd, int.Parse(item["ID"].ToString()),
                        (uint)((HotKey)item["HotKeyObject"]).ModKeys,
                        (uint)(KeyInterop.VirtualKeyFromKey(((HotKey)item["HotKeyObject"]).Key)));

                    if (result == false)
                    {
                        throw new HotKey.HotKeyException("HotKey registration was failed.");
                    }
                }
            }
            catch(HotKey.HotKeyException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        public void UnregisterAllOpeToHotKey(IntPtr hWnd)
        {
            try
            {
                foreach (DataRow item in Rows)
                {
                    bool result = HotKey.UnregisterHotKey(hWnd, int.Parse(item["ID"].ToString()));

                    if (result == false)
                    {
                        throw new HotKey.HotKeyException("HotKey unregistration was failed.");
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
    }
}
