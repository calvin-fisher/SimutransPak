using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SimutransPak
{
    /// <summary>
    /// Represents a .dat file, which can contain many related objects/vehicles/waytypes/etc.
    /// </summary>
    public class DatFile
    {
        #region Constructor/Factories

        private DatFile(FileInfo file, Pak pak)
        {
            _pak = pak;
            SourceFile = file;

            var s = file.OpenRead();
            var sr = new StreamReader(s);
            var contents = sr.ReadToEnd();

            var lines = contents.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            _objects = ParseObjects(lines).ToList();
            Objects = new ReadOnlyCollection<DatObject>(_objects);
        }

        /// <summary>
        /// Factory pattern to return null for invalid input.
        /// </summary>
        public static DatFile Create(FileInfo file, Pak pak)
        {
            try
            {
                if (!file.Exists)
                    return null;

                var instance = new DatFile(file, pak);

                return instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        public static IEnumerable<DatFile> FindFilesRecursive(string directory, Pak pak)
        {
            return FindFilesRecursive(new DirectoryInfo(directory), pak);
        }

        public static IEnumerable<DatFile> FindFilesRecursive(DirectoryInfo directory, Pak pak)
        {
            if (!directory.Exists)
                yield break;

            var thisDirectory = directory
                .GetFiles("*.dat")
                .Select(f => DatFile.Create(f, pak))
                .Where(f => f != null);

            foreach (var file in thisDirectory)
                yield return file;

            foreach (var subDirectory in directory.GetDirectories())
                foreach (var subDirectoryFile in FindFilesRecursive(subDirectory, pak))
                    yield return subDirectoryFile;
        }

        #endregion

        private Pak _pak { get; set; }
        public FileInfo SourceFile { get; private set; }

        private readonly IList<DatObject> _objects;
        public readonly ReadOnlyCollection<DatObject> Objects;

        public override string ToString()
        {
            const string separator = "----------";

            var objectStrings = _objects.Select(x => x.ToString());
            var output = string.Join(Environment.NewLine + separator + Environment.NewLine, objectStrings);

            return output;
        }

        private IEnumerable<DatObject> ParseObjects(IEnumerable<string> lines)
        {
            var objects = new List<DatObject>();
            var elements = new List<DictionaryEntry>();

            foreach (string line in lines.Where(line => !string.IsNullOrEmpty(line) && line[0] != '#'))
            {
                // Separator means finish the current object and start assembling a new one
                if (line[0] == '-')
                {
                    if (elements.Any())
                        objects.Add(new DatObject(elements, _pak, this));

                    elements = new List<DictionaryEntry>();
                    continue;
                }

                var equalsIndex = line.IndexOf('=');
                if (equalsIndex < 0 || line.Length == equalsIndex + 1)
                    continue;

                var name = line.Substring(0, equalsIndex).TrimStart(' ').TrimEnd(' ').ToLower();
                var value = line.Substring(equalsIndex + 1).TrimStart(' ').TrimEnd(' ').ToLower();

                elements.Add(new DictionaryEntry(name, value));
            }

            // Flush last object if no trailing separator
            if (elements.Any())
                objects.Add(new DatObject(elements, _pak, this));

            return objects;
        }
    }
}
