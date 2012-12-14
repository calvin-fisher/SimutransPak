using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimutransPak
{
    /// <summary>
    /// Represents the definition for a single object/vehicle/waytype/etc. -- basically just a collection of name-value pairs
    /// </summary>
    public class DatObject
    {
        #region Constructor/Factories

        public DatObject(IEnumerable<DictionaryEntry> elements, Pak pak, DatFile file)
        {
            _pak = pak;
            _datFile = file;
            _dictionary = elements.ToDictionary(x => (string)x.Key, x => (string)x.Value);
        }

        public static IEnumerable<DatObject> FindObjectsRecursive(string directory, Pak pak)
        {
            return FindObjectsRecursive(new DirectoryInfo(directory), pak);
        }

        public static IEnumerable<DatObject> FindObjectsRecursive(DirectoryInfo directory, Pak pak)
        {
            var files = DatFile.FindFilesRecursive(directory, pak);
            return files.SelectMany(f => f.Objects);
        }

        #endregion

        public Pak _pak { get; set; }
        public DatFile _datFile { get; set; }

        private readonly Dictionary<string, string> _dictionary;

        #region Known Properties

        public string Name { get { return this["name"]; } }
        public string NameTranslated { get { return _pak.TranslationFile != null ? _pak.TranslationFile[Name] : null; } }
        public string Obj { get { return this["obj"]; } }
        public string Waytype { get { return this["waytype"]; } }
        public int? IntroYear { get { return TryParse(this["intro_year"]); } }
        public int? IntroMonth { get { return TryParse(this["intro_month"]); } }
        public int? RetireYear { get { return TryParse(this["retire_year"]); } }
        public int? RetireMonth { get { return TryParse(this["retire_month"]); } }
        public int? Cost { get { return TryParse(this["cost"]); } }
        public int? Maintenance { get { return TryParse(this["maintenance"]); } }
        public int? Runningcost { get { return TryParse(this["runningcost"]); } }
        public int? Topspeed { get { return TryParse(this["topspeed"]); } }
        public int? Maxweight { get { return TryParse(this["maxweight"]); } }
        public int? Speed { get { return TryParse(this["speed"]); } }
        public int? Power { get { return TryParse(this["power"]); } }
        public int? TractiveEffort { get { return TryParse(this["tractive_effort"]); } }
        public string EngineType { get { return this["engine_type"]; } }
        public int? Weight { get { return TryParse(this["weight"]); } }
        public int? Length { get { return TryParse(this["length"]); } }
        public string Freight { get { return this["freight"]; } }
        public string FreightTranslated { get { return _pak.TranslationFile != null ? _pak.TranslationFile[Freight] : null; } }
        public int? Payload { get { return TryParse(this["payload"]); } }
        public int? OvercrowdedCapacity { get { return TryParse(this["overcrowded_capacity"]); } }
        public int? Comfort { get { return TryParse(this["comfort"]); } }
        public int? MinimumRunwayLength { get { return TryParse(this["minimum_runway_length"]); } }

        #endregion

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

        public static implicit operator Dictionary<string, string>(DatObject instance)
        {
            return instance._dictionary;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var property in _dictionary)
            {
                sb.AppendLine(string.Format("{0}={1}", property.Key, property.Value));
            }

            return sb.ToString();
        }

        private int? TryParse(string value)
        {
            int parsed;
            return int.TryParse(value, out parsed)
                       ? parsed
                       : (int?)null;
        }
    }
}
