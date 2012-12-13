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
            var directory = new DirectoryInfo(path);
            if (!directory.Exists)
                throw new DirectoryNotFoundException(string.Format("Path {0} does not exist, is not a directory, or you do not have permissions.", path));

            var files = DatFile.FindFilesRecursive(directory);

            var objects = files.GetAllObjects().ToList();
            objects.ForEach(o => o.Pak = this);
            _objects = objects;
            Objects = new ReadOnlyCollection<DatObject>(_objects);

            var textDirectory = directory.GetDirectories().FirstOrDefault(x => x.Name.ToLower() == "text");
            if (textDirectory != null)
            {
                var translationFile = textDirectory.GetFiles().FirstOrDefault(x => x.Name.ToLower() == language + ".tab");
                if (translationFile != null)
                    TranslationFile = TranslationFile.Create(translationFile);
            }
        }

        private readonly IList<DatObject> _objects;
        public readonly ReadOnlyCollection<DatObject> Objects;

        public TranslationFile TranslationFile { get; private set; }
    }
}
