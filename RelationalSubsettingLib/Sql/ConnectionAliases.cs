using RelationalSubsettingLib.Properties;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace RelationalSubsettingLib.Sql
{
    public class ConnectionAliases : IDictionary<string, string>
    {
        #region Public Properties

        public int Count => ((IDictionary<string, string>)m_Aliases).Count;
        public bool IsReadOnly => ((IDictionary<string, string>)m_Aliases).IsReadOnly;
        public ICollection<string> Keys => ((IDictionary<string, string>)m_Aliases).Keys;
        public ICollection<string> Values => ((IDictionary<string, string>)m_Aliases).Values;

        #endregion Public Properties

        #region Private Fields

        private Dictionary<string, string> m_Aliases;

        #endregion Private Fields

        #region Public Constructors

        public ConnectionAliases()
        {
            ReadFile();
        }

        #endregion Public Constructors

        #region Public Methods

        public void Add(string key, string value)
        {
            ((IDictionary<string, string>)m_Aliases).Add(key, value);
            RefreshFile();
        }

        public void Add(KeyValuePair<string, string> item)
        {
            ((IDictionary<string, string>)m_Aliases).Add(item);
            RefreshFile();
        }

        public void Clear()
        {
            ((IDictionary<string, string>)m_Aliases).Clear();
            RefreshFile();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return ((IDictionary<string, string>)m_Aliases).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return m_Aliases.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            ((IDictionary<string, string>)m_Aliases).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return ((IDictionary<string, string>)m_Aliases).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, string>)m_Aliases).GetEnumerator();
        }

        public bool Remove(string key)
        {
            var r = ((IDictionary<string, string>)m_Aliases).Remove(key);
            RefreshFile();
            return r;
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            var r = ((IDictionary<string, string>)m_Aliases).Remove(item);
            RefreshFile();
            return r;
        }

        public bool TryGetValue(string key, out string value)
        {
            return ((IDictionary<string, string>)m_Aliases).TryGetValue(key, out value);
        }

        #endregion Public Methods

        #region Private Methods

        private void ReadFile()
        {
            string path = $"{Directory.GetCurrentDirectory()}\\.rds\\{Settings.ConnectionAliasFileName}";
            if (File.Exists(path))
            {
                m_Aliases = m_Aliases.LoadFromFile(path);
            }
            else
            {
                m_Aliases = new Dictionary<string, string>();
            }
        }

        private void RefreshFile()
        {
            string path = $"{Directory.GetCurrentDirectory()}\\.rds\\{Settings.ConnectionAliasFileName}";
            m_Aliases.SaveToFile(path);
        }

        #endregion Private Methods

        #region Public Indexers

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

        #endregion Public Indexers
    }
}