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

        public IEnumerable<DatObject> GetVehicles()
        {
            return _objects.Where(x => x["obj"] == "vehicle");
        }

        public IEnumerable<DatObject> GetTrains()
        {
            return GetVehicles().Where(x => x["waytype"] == "track");
        }

        public IEnumerable<DatObject> GetBuses()
        {
            return GetVehicles().Where(x => x["waytype"] == "road");
        }

        public IEnumerable<DatObject> GetShips()
        {
            return GetVehicles().Where(x => x["waytype"] == "water");
        }

        public IEnumerable<DatObject> GetNarrowgauge()
        {
            return GetVehicles().Where(x => x["waytype"] == "narrowgauge_track");
        }

        public IEnumerable<DatObject> GetTrams()
        {
            return GetVehicles().Where(x => x["waytype"] == "tram_track");
        }

        #endregion

        #region Waytypes

        public IEnumerable<DatObject> GetWaytypes()
        {
            return _objects.Where(x => x["obj"] == "way");
        }

        public IEnumerable<DatObject> GetTracks()
        {
            return GetWaytypes().Where(x => x["waytype"] == "track");
        }

        public IEnumerable<DatObject> GetRoads()
        {
            return GetWaytypes().Where(x => x["waytype"] == "road");
        }

        public IEnumerable<DatObject> GetRivers()
        {
            return GetWaytypes().Where(x => x["waytype"] == "water");
        }

        public IEnumerable<DatObject> GetNarrowgaugeTracks()
        {
            return GetWaytypes().Where(x => x["waytype"] == "narrowgauge_track");
        }

        public IEnumerable<DatObject> GetTramTracks()
        {
            return GetWaytypes().Where(x => x["waytype"] == "tram_track");
        }

        #endregion
    }
}
