using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SimutransPak
{
    public class TranslationFile
    {
        private TranslationFile(FileInfo file)
        {
            var s = file.OpenRead();
            var sr = new StreamReader(s);
            var contents = sr.ReadToEnd();
            
            var lines = contents.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            
            var entries = ParseFile(lines);
            _dictionary = entries.ToDictionary(x => (string)x.Key, x => (string)x.Value);
        }

        private Dictionary<string, string> ParseFile(string[] lines)
        {
            var dictionary = new Dictionary<string, string>();
            string key = null;
            foreach (var line in lines.Where(line => !string.IsNullOrEmpty(line) && line[0] != '#'))
            {
                if (key == null)
                {
                    key = line.ToLower();
                    continue;
                }

                if (!dictionary.ContainsKey(key))
                    dictionary.Add(key, line);

                key = null;
            }
            return dictionary;
        }

        public static TranslationFile Create(FileInfo file)
        {
            try
            {
                if (!file.Exists)
                    return null;

                var instance = new TranslationFile(file);
                return instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        private readonly Dictionary<string, string> _dictionary;

        public string this[string name]
        {
            get
            {
                string value;
                return _dictionary.TryGetValue(name, out value)
                    ? value
                    : null;
            }
        }

        public static implicit operator Dictionary<string, string>(TranslationFile instance)
        {
            return instance._dictionary;
        }

    }
}
