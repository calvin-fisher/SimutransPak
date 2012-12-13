using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimutransPak
{
    /// <summary>
    /// Represents the definition for a single object/vehicle/waytype/etc. -- basically just a collection of name-value pairs
    /// </summary>
    public class DatObject
    {
        public DatObject(IEnumerable<DictionaryEntry> elements)
        {
            _dictionary = elements.ToDictionary(x => (string)x.Key, x => (string)x.Value);
        }

        public Pak Pak { get; internal set; }

        private readonly Dictionary<string, string> _dictionary;

        #region Known Properties

        public string Name { get { return this["name"]; } }
        public string NameTranslated { get { return Pak.TranslationFile != null ? Pak.TranslationFile[Name] : null; } }
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
        public string FreightTranslated { get { return Pak.TranslationFile != null ? Pak.TranslationFile[Freight] : null; } }
        public int? Payload { get { return TryParse(this["payload"]); } }
        public int? OvercrowdedCapacity { get { return TryParse(this["overcrowded_capacity"]); } }

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
            sb.Append("{ ");
            foreach (var property in _dictionary)
            {
                sb.AppendFormat("{0}: {1}, ", property.Key, property.Value);
            }
            if (_dictionary.Count > 1)
                sb.Remove(sb.Length - 2, 2);
            sb.Append(" }");

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

    public static class DatObjectExtensions
    {
        #region Vehicles

        public static IEnumerable<DatObject> GetVehicles(this IEnumerable<DatObject> collection)
        {
            return collection.Where(x => x["obj"] == "vehicle");
        }

        public static IEnumerable<DatObject> GetTrains(this IEnumerable<DatObject> collection)
        {
            return collection.GetVehicles().Where(x => x["waytype"] == "track");
        }

        public static IEnumerable<DatObject> GetBuses(this IEnumerable<DatObject> collection)
        {
            return collection.GetVehicles().Where(x => x["waytype"] == "road");
        }

        public static IEnumerable<DatObject> GetShips(this IEnumerable<DatObject> collection)
        {
            return collection.GetVehicles().Where(x => x["waytype"] == "water");
        }

        public static IEnumerable<DatObject> GetNarrowgauge(this IEnumerable<DatObject> collection)
        {
            return collection.GetVehicles().Where(x => x["waytype"] == "narrowgauge_track");
        }

        public static IEnumerable<DatObject> GetTrams(this IEnumerable<DatObject> collection)
        {
            return collection.GetVehicles().Where(x => x["waytype"] == "tram_track");
        }

        #endregion

        #region Waytypes

        public static IEnumerable<DatObject> GetWaytypes(this IEnumerable<DatObject> collection)
        {
            return collection.Where(x => x["obj"] == "way");
        }

        public static IEnumerable<DatObject> GetTracks(this IEnumerable<DatObject> collection)
        {
            return collection.GetWaytypes().Where(x => x["waytype"] == "track");
        }

        public static IEnumerable<DatObject> GetRoads(this IEnumerable<DatObject> collection)
        {
            return collection.GetWaytypes().Where(x => x["waytype"] == "road");
        }

        public static IEnumerable<DatObject> GetRivers(this IEnumerable<DatObject> collection)
        {
            return collection.GetWaytypes().Where(x => x["waytype"] == "water");
        }

        public static IEnumerable<DatObject> GetNarrowgaugeTracks(this IEnumerable<DatObject> collection)
        {
            return collection.GetWaytypes().Where(x => x["waytype"] == "narrowgauge_track");
        }

        public static IEnumerable<DatObject> GetTramTracks(this IEnumerable<DatObject> collection)
        {
            return collection.GetWaytypes().Where(x => x["waytype"] == "tram_track");
        }

        #endregion
    }
}
