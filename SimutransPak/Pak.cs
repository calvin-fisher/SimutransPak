using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace SimutransPak
{
    public class Pak
    {
        public Pak(string path, string language = "en")
        {
            Path = path;
            Language = language;

            var directory = new DirectoryInfo(Path);
            if (!directory.Exists)
                throw new DirectoryNotFoundException(string.Format("Path {0} does not exist, is not a directory, or you do not have permissions.", Path));

            var files = DatFile.FindFilesRecursive(directory, this);

            var objects = files.SelectMany(f => f.Objects).ToList();
            _objects = objects;
            Objects = new ReadOnlyCollection<DatObject>(_objects);

            var textDirectory = directory.GetDirectories().FirstOrDefault(x => x.Name.ToLower() == "text");
            if (textDirectory != null)
            {
                var translationFile = textDirectory.GetFiles().FirstOrDefault(x => x.Name.ToLower() == Language + ".tab");
                if (translationFile != null)
                    TranslationFile = TranslationFile.Create(translationFile);
            }
        }

        public string Path { get; private set; }
        public string Language { get; private set; }

        private readonly IList<DatObject> _objects;
        public readonly ReadOnlyCollection<DatObject> Objects;

        public TranslationFile TranslationFile { get; private set; }

        #region Vehicles

        public IEnumerable<DatObject> GetAllVehicles()
        {
            return _objects.Where(x => x["obj"] == "vehicle");
        }

        public IEnumerable<DatObject> GetTrackVehicles()
        {
            return GetAllVehicles().Where(x => x["waytype"] == "track");
        }

        public IEnumerable<DatObject> GetRoadVehicles()
        {
            return GetAllVehicles().Where(x => x["waytype"] == "road");
        }

        public IEnumerable<DatObject> GetWaterVehicles()
        {
            return GetAllVehicles().Where(x => x["waytype"] == "water");
        }

        public IEnumerable<DatObject> GetNarrowgaugeVehicles()
        {
            return GetAllVehicles().Where(x => x["waytype"] == "narrowgauge_track");
        }

        public IEnumerable<DatObject> GetTramVehicles()
        {
            return GetAllVehicles().Where(x => x["waytype"] == "tram_track");
        }

        #endregion

        #region Waytypes

        public IEnumerable<DatObject> GetAllWays()
        {
            return _objects.Where(x => x["obj"] == "way");
        }

        public IEnumerable<DatObject> GetTrackWays()
        {
            return GetAllWays().Where(x => x["waytype"] == "track");
        }

        public IEnumerable<DatObject> GetRoadWays()
        {
            return GetAllWays().Where(x => x["waytype"] == "road");
        }

        public IEnumerable<DatObject> GetWaterWays()
        {
            return GetAllWays().Where(x => x["waytype"] == "water");
        }

        public IEnumerable<DatObject> GetNarrowgaugeWays()
        {
            return GetAllWays().Where(x => x["waytype"] == "narrowgauge_track");
        }

        public IEnumerable<DatObject> GetTramWays()
        {
            return GetAllWays().Where(x => x["waytype"] == "tram_track");
        }

        #endregion
    }
}
