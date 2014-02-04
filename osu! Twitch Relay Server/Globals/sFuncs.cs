using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace smgiFuncs
{

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
            s_file = new System.IO.FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\settings.dat", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
            using (System.IO.StreamReader sR = new System.IO.StreamReader(s_file))
            {
                while (sR.Peek() != -1)
                {
                    string s = sR.ReadLine();
                    s_settings.Add(s.Substring(0, s.IndexOf(":")), s.Substring(s.IndexOf(":") + 1));
                }
            }
            s_file = new System.IO.FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\settings.dat", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
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
        public List<string> GetKeys()
        {
            List<string> temp = new List<string>();
            foreach (string k in s_settings.Keys)
            {
                temp.Add(k);
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
            if (constructedString != "")
            {
                constructedString = constructedString.Substring(0, constructedString.LastIndexOf(Environment.NewLine));
            }
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
        public int CountOf(string splitter)
        {
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
            return count;
        }
        public int nthDexOf(string splitter, int count)
        {
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
            int tempval = _data.LastIndexOf(splitter);
            return tempval;
        }
    }
    #endregion
}
