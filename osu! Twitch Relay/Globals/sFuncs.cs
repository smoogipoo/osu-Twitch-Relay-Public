using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace smgiFuncs
{
    #region "Updater"
    public class Updater
    {
        public Updater(Settings settings)
        {
            foreach (string f in System.IO.Directory.GetFiles(Application.StartupPath, "*.*", System.IO.SearchOption.TopDirectoryOnly))
            {
                if (f.Contains("."))
                {
                    if (f.Substring(f.LastIndexOf(".")) == ".old")
                    {
                        System.IO.File.Delete(f);
                    }
                }

            }
            Dictionary<string, string> versions = new Dictionary<string, string>();
            foreach (sString f in System.IO.Directory.GetFiles(Application.StartupPath, "*.*", System.IO.SearchOption.TopDirectoryOnly))
            {
                settings.AddSetting("v_" + f.SubString(f.LastIndexOf("\\") + 1), "1.0.0", false);
            }
            foreach (string key in settings.GetKeys())
            {
                if (key.StartsWith("v_"))
                {
                    string fname = key.Substring(2);
                    if (versions.ContainsKey(fname) == false)
                    {
                        versions.Add(fname, settings.GetSetting(key));
                    }
                }
            }
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                if (System.IO.File.Exists(Application.StartupPath + "\\files.updt"))
                {
                    System.IO.File.Delete(Application.StartupPath + "\\files.updt");
                }
                wc.DownloadFile("http://repo.smgi.me/" + Application.ProductName + "/files.updt", Application.StartupPath + "\\files.updt");
                using (System.IO.StreamReader sR = new System.IO.StreamReader(Application.StartupPath + "\\files.updt"))
                {
                    while (sR.Peek() != -1)
                    {
                        //Example /asdf.exe:\asdf.exe:1.0.1
                        sString line = sR.ReadLine();
                        string onlinepath = line.SubString(0, line.nthDexOf(":", 0));
                        sString localpath = line.SubString(line.nthDexOf(":", 0) + 1, line.nthDexOf(":", 1));
                        string version = line.SubString(line.nthDexOf(":", 1) + 1);
                        string name = localpath.SubString(localpath.LastIndexOf("\\") + 1);
                        if (versions.ContainsKey(name) == false)
                        {
                            wc.DownloadFile("http://repo.smgi.me/" + Application.ProductName + onlinepath, Application.StartupPath + localpath.ToString());
                            settings.AddSetting("v_" + name, version);
                        }
                        else
                        {
                            if (version != versions[name])
                            {
                                if (System.IO.File.Exists(Application.StartupPath + localpath.ToString() + ".old"))
                                {
                                    System.IO.File.Delete(Application.StartupPath + localpath.ToString() + ".old");
                                }
                                string n = Application.StartupPath + localpath.ToString() + ".old";
                                System.IO.File.Move(Application.StartupPath + localpath.ToString(), Application.StartupPath + localpath.ToString() + ".old");
                                wc.DownloadFile("http://repo.smgi.me/" + Application.ProductName + onlinepath, Application.StartupPath + localpath.ToString());
                                settings.AddSetting("v_" + name, version, true);
                            }
                        }
                    }
                }
            }
            catch { }
            if (System.IO.File.Exists(Application.StartupPath + "\\files.updt"))
            {
                System.IO.File.Delete(Application.StartupPath + "\\files.updt");
            }
            settings.Save();
        }
    }
    #endregion
    #region "Application Settings"
    public class Settings
    {
        Dictionary<string, string> s_settings = new Dictionary<string, string>();
        System.IO.FileStream s_file;

        public Settings()
        {
            LoadSettings();
        }
        public void LoadSettings()
        {
            s_file = new System.IO.FileStream(Application.StartupPath + "\\settings.dat", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
            using (System.IO.StreamReader sR = new System.IO.StreamReader(s_file))
            {
                while (sR.Peek() != -1)
                {
                    string s = sR.ReadLine();
                    s_settings.Add(s.Substring(0, s.IndexOf(":")), s.Substring(s.IndexOf(":") + 1));
                }
            }
            s_file = new System.IO.FileStream(Application.StartupPath + "\\settings.dat", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
        }
        public bool ContainsSetting(string name)
        {
            if (s_settings.ContainsKey(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string[] GetKeys()
        {
            string[] temp = null;
            temp = new string[s_settings.Count];
            int currentindex = 0;
            foreach (string k in s_settings.Keys)
            {
                temp[currentindex] = k;
                currentindex += 1;
            }
            return temp;
        }
        public void AddSetting(string name, string value, bool overwrite = true)
        {
            if ((s_settings.ContainsKey(name)) & (overwrite == true))
            {
                s_settings[name] = value;
            }
            else if (s_settings.ContainsKey(name) == false)
            {
                s_settings.Add(name, value);
            }
        }
        public string GetSetting(string name)
        {
            if (s_settings.ContainsKey(name))
            {
                return s_settings[name];
            }
            else { return ""; }
        }
        public void DeleteSetting(string name)
        {
            if (s_settings.ContainsKey(name))
            {
                s_settings.Remove(name);
            }
        }
        public void Save()
        {
            string constructedString = "";
            foreach (var di in s_settings)
            {
                constructedString += di.Key + ":" + di.Value + Environment.NewLine;
            }
            constructedString = constructedString.Substring(0, constructedString.LastIndexOf(Environment.NewLine));
            s_file.SetLength(constructedString.Length);
            s_file.Position = 0;
            byte[] bytesToWrite = System.Text.Encoding.ASCII.GetBytes(constructedString);
            s_file.Write(bytesToWrite, 0, bytesToWrite.Length);
            s_file.Flush();
        }
    }
    #endregion
    #region "String Datatype"
    public class sString
    {
        private System.Globalization.CultureInfo lastcltr = Application.CurrentCulture;
        private System.Globalization.CultureInfo cltr = Application.CurrentCulture;
        private string _data;
        public sString(String s)
        {
            _data = s;
        }
        public static implicit operator sString(String s)
        {
            return new sString(s);
        }
        public override string ToString()
        {
            return _data;
        }
        public System.Globalization.CultureInfo Culture
        {
            get { return cltr; }
            set { cltr = value; }
        }
        public int CountOf(string splitter)
        {
            lastcltr = Application.CurrentCulture;
            Application.CurrentCulture = cltr;
            int indx = -1;
            int count = 0;
            indx = _data.IndexOf(splitter, indx + 1);
            if (indx != -1)
            {
                while (indx != -1)
                {
                    count += 1;
                    indx = _data.IndexOf(splitter, indx + 1);
                }
            }
            Application.CurrentCulture = lastcltr;
            return count;
        }
        public int nthDexOf(string splitter, int count)
        {
            lastcltr = Application.CurrentCulture;
            Application.CurrentCulture = cltr;
            int camnt = -1;
            int indx = 0;
            indx = _data.IndexOf(splitter);
            camnt += 1;
            while (!((camnt == count) | (indx == -1)))
            {
                indx = _data.IndexOf(splitter, indx + 1);
                if (indx == -1)
                {
                    return indx;
                }
                camnt += 1;
            }
            Application.CurrentCulture = lastcltr;
            return indx;
        }
        public string SubString(int startindex, int endindex = -1)
        {
            if (startindex < 0)
            {
                throw new Exception("The startindex value of '" + startindex + "' is less than zero.", new Exception("String: " + _data));
            }
            if (endindex < -1)
            {
                throw new Exception("The endindex value of '" + endindex + "' is less than -1.", new Exception("String: " + _data));
            }
            if ((endindex < startindex) & (endindex != -1))
            {
                throw new Exception("The endindex value of '" + endindex + "' is less than the startindex value of '" + startindex + "'.", new Exception("String: " + _data));
            }
            if (endindex == -1)
            {
                return _data.Substring(startindex, _data.Length - startindex);
            }
            else
            {
                if (endindex > _data.Length)
                {
                    throw new Exception("The endindex value of '" + endindex + "' exceeds the string length.", new Exception("String: " + _data, new Exception("Length: " + _data.Length)));
                }
                return _data.Substring(startindex, endindex - startindex);
            }
        }
        public int LastIndexOf(string splitter)
        {
            lastcltr = Application.CurrentCulture;
            Application.CurrentCulture = cltr;
            int tempval = _data.LastIndexOf(splitter);
            Application.CurrentCulture = lastcltr;
            return tempval;
        }
    }
    #endregion
}