using Newtonsoft.Json;
using RelationalSubsettingLib.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RelationalSubsettingLib.Sql
{
    public class ConnectionAliases : IDictionary<string,string>
    {
        private Dictionary<string, string> m_Aliases;


        #region interface
        public string this[string key]
        {
            get
            {
                if (m_Aliases == null)
                {
                    ReadFile();
                }
                return m_Aliases[key];
            }
            set
            {
                m_Aliases[key] = value;
                RefreshFile();
            }
        }
        public ICollection<string> Keys => ((IDictionary<string, string>)m_Aliases).Keys;

        public ICollection<string> Values => ((IDictionary<string, string>)m_Aliases).Values;

        public int Count => ((IDictionary<string, string>)m_Aliases).Count;

        public bool IsReadOnly => ((IDictionary<string, string>)m_Aliases).IsReadOnly;

        public void Add(string key, string value)
        {
            ((IDictionary<string, string>)m_Aliases).Add(key, value);
        }

        public void Add(KeyValuePair<string, string> item)
        {
            ((IDictionary<string, string>)m_Aliases).Add(item);
        }

        public void Clear()
        {
            ((IDictionary<string, string>)m_Aliases).Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return ((IDictionary<string, string>)m_Aliases).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, string>)m_Aliases).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            ((IDictionary<string, string>)m_Aliases).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return ((IDictionary<string, string>)m_Aliases).GetEnumerator();
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, string>)m_Aliases).Remove(key);
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return ((IDictionary<string, string>)m_Aliases).Remove(item);
        }

        public bool TryGetValue(string key, out string value)
        {
            return ((IDictionary<string, string>)m_Aliases).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, string>)m_Aliases).GetEnumerator();
        }

        #endregion

        private void ReadFile()
        {
            string path = $"{Directory.GetCurrentDirectory()}\\.rds\\{Settings.ConnectionAliasFileName}";
            m_Aliases.LoadFromFile(path);
        }
        private void RefreshFile()
        {
            string path = $"{Directory.GetCurrentDirectory()}\\.rds\\{Settings.ConnectionAliasFileName}";
            m_Aliases.SaveToFile(path);
        }
    }
}
