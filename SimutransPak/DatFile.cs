using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SimutransPak
{
    /// <summary>
    /// Represents a .dat file, which can contain many related objects/vehicles/waytypes/etc.
    /// </summary>
    public class DatFile
    {
        private DatFile(FileInfo file)
        {
            var s = file.OpenRead();
            var sr = new StreamReader(s);
            OriginalFile = sr.ReadToEnd();

            var lines = OriginalFile.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            _objects = ParseObjects(lines).ToList();
            Objects = new ReadOnlyCollection<DatObject>(_objects);
        }

        private IEnumerable<DatObject> ParseObjects(IEnumerable<string> lines)
        {
            var objects = new List<DatObject>();
            var elements = new List<DictionaryEntry>();

            foreach (string line in lines.Where(line => !string.IsNullOrEmpty(line) && line[0] != '#'))
            {
                if (line[0] == '-')
                {
                    objects.Add(new DatObject(elements));
                    elements = new List<DictionaryEntry>();
                    continue;
                }

                var equalsIndex = line.IndexOf('=');
                if (equalsIndex < 0)
                    continue;

                var name = line.Substring(0, equalsIndex).TrimEnd(' ').ToLower();
                var value = line.Substring(equalsIndex + 1).TrimStart(' ').TrimEnd(' ').ToLower();

                elements.Add(new DictionaryEntry(name, value));
            }

            //Flush
            if (elements.Count > 0)
                objects.Add(new DatObject(elements));

            return objects;
        }

        /// <summary>
        /// Factory pattern to return null for invalid input.
        /// </summary>
        public static DatFile Create(FileInfo file)
        {
            try
            {
                if (!file.Exists)
                    return null;

                var instance = new DatFile(file);

                return instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        public string OriginalFile { get; private set; }

        private readonly IList<DatObject> _objects;
        public readonly ReadOnlyCollection<DatObject> Objects;

        public static IEnumerable<DatObject> FindObjectsRecursive(string directory)
        {
            return FindObjectsRecursive(new DirectoryInfo(directory));
        }

        public static IEnumerable<DatObject> FindObjectsRecursive(DirectoryInfo directory)
        {
            var files = FindFilesRecursive(directory);
            return files.GetAllObjects();
        }

        public static IEnumerable<DatFile> FindFilesRecursive(string directory)
        {
            return FindFilesRecursive(new DirectoryInfo(directory));
        }

        public static IEnumerable<DatFile> FindFilesRecursive(DirectoryInfo directory)
        {
            if (!directory.Exists)
                yield break;

            var thisDirectory = directory.EnumerateFiles("*.dat").Select(DatFile.Create).Where(f => f != null);
            foreach (var file in thisDirectory)
                yield return file;

            foreach (var subDirectoryFile in directory.GetDirectories().SelectMany(FindFilesRecursive))
                yield return subDirectoryFile;
        }
    }

    public static class DatFileExtensions
    {
        public static IEnumerable<DatObject> GetAllObjects(this IEnumerable<DatFile> files)
        {
            return files.SelectMany(x => x.Objects);
        }
    }
}
