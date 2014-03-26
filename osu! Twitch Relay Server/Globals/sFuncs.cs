using System;
using System.Collections.Generic;
using System.Linq;

namespace smgiFuncs
{

    #region "Application Settings"
    public class Settings
    {
        internal readonly Dictionary<string, string> s_settings = new Dictionary<string, string>();
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
                    if (s != null)
                        s_settings.Add(s.Substring(0, s.IndexOf(":", StringComparison.Ordinal)), s.Substring(s.IndexOf(":", StringComparison.Ordinal) + 1));
                }
            }
            s_file = new System.IO.FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\settings.dat", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
        }
        public bool ContainsSetting(string name)
        {
            return s_settings.ContainsKey(name);
        }

        public List<string> GetKeys()
        {
            lock (this)
            {
                return s_settings.Keys.ToList();
            }     
        }
        public void AddSetting(string name, string value, bool overwrite = true)
        {
            lock (this)
            {
                if ((s_settings.ContainsKey(name)) & (overwrite))
                {
                    s_settings[name] = value;
                }
                else if (s_settings.ContainsKey(name) == false)
                {
                    s_settings.Add(name, value);
                }
            }

        }
        public string GetSetting(string name)
        {
            lock (this)
            {
                return s_settings.ContainsKey(name) ? s_settings[name] : "";
            }          
        }

        public void DeleteSetting(string name)
        {
            lock (this)
            {
                if (s_settings.ContainsKey(name))
                {
                    s_settings.Remove(name);
                }
            }

        }
        public void Save()
        {
            lock (this)
            {
                string constructedString = s_settings.Aggregate("", (str, di) => str + (di.Key + ":" + di.Value + Environment.NewLine));
                if (constructedString != "")
                {
                    constructedString = constructedString.Substring(0, constructedString.LastIndexOf(Environment.NewLine, StringComparison.Ordinal));
                }
                s_file.SetLength(constructedString.Length);
                s_file.Position = 0;
                byte[] bytesToWrite = System.Text.Encoding.ASCII.GetBytes(constructedString);
                s_file.Write(bytesToWrite, 0, bytesToWrite.Length);
                s_file.Flush();
            }

        }
    }
    #endregion
    #region "String Datatype"
    public class sString
    {
        internal readonly string _data;
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
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            var other = obj as sString;
            if (other == null)
            {
                return false;
            }
            return other._data == _data;
        }
        public override int GetHashCode()
        {
            return _data.GetHashCode();
        }
        public int CountOf(string splitter)
        {
            int indx = -1;
            int count = 0;
            indx = _data.IndexOf(splitter, indx + 1, StringComparison.Ordinal);
            if (indx != -1)
            {
                while (indx != -1)
                {
                    count += 1;
                    indx = _data.IndexOf(splitter, indx + 1, StringComparison.Ordinal);
                }
            }
            return count;
        }
        public int nthDexOf(string splitter, int count)
        {
            int camnt = -1;
            int indx = _data.IndexOf(splitter, StringComparison.Ordinal);
            camnt += 1;
            while (!((camnt == count) | (indx == -1)))
            {
                indx = _data.IndexOf(splitter, indx + 1, StringComparison.Ordinal);
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
            if (endindex > _data.Length)
            {
                throw new Exception("The endindex value of '" + endindex + "' exceeds the string length.", new Exception("String: " + _data, new Exception("Length: " + _data.Length)));
            }
            return _data.Substring(startindex, endindex - startindex);
        }
        public int LastIndexOf(string splitter)
        {
            return _data.LastIndexOf(splitter, StringComparison.Ordinal);
        }
    }
    #endregion
}
