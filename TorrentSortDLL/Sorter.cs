﻿using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace TorrentSortDLL
{
    public class Sorter
    {
        private const string MutexName = "TorrentSort";
        private Config config;

        public Sorter()
        {
            config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json")));
        }

        public void Sort()
        {
            if (HasOpenInstance())
            {
                return;
            }

            Regex regexFilename = new Regex(config.FileRegex);
            Regex regexFolder = new Regex(config.FolderRegex);

            foreach (var file in Directory.GetFiles(config.RootFolder, config.FileFilter))
            {
                var filename = Path.GetFileNameWithoutExtension(file);

                foreach (Match match in regexFilename.Matches(filename))
                {
                    filename = filename.Replace(match.Value, string.Empty).Trim();
                }

                var ext = $"{filename}{Path.GetExtension(file)}";
                var path = Path.Combine(config.RootFolder, regexFolder.Matches(filename)[0].Value.Trim());
                Directory.CreateDirectory(path);
                File.Move(file, Path.Combine(path, ext));
            }
        }

        private bool HasOpenInstance()
        {
            try
            {
                Mutex.OpenExisting(MutexName);
            }
            catch
            {
                new Mutex(true, MutexName);

                return false;
            }
            return true;
        }
    }
}