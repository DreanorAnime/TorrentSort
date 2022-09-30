using System;
using Newtonsoft.Json;
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

        public Sorter(string path)
        {
            config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine(path, "config.json")));
        }

        public void Sort()
        {
            if (HasOpenInstance())
            {
                return;
            }

            Regex regexFilename = new Regex(config.FileRegex);

            foreach (var file in Directory.GetFiles(config.RootFolder, config.FileFilter))
            {
                var filename = Path.GetFileNameWithoutExtension(file);

                var newFolderName = filename;
                foreach (Match match in regexFilename.Matches(newFolderName))
                {
                    newFolderName = newFolderName.Replace(match.Value, string.Empty).Trim();
                }

                var index = newFolderName.LastIndexOf("-");
                if (index > 0)
                {
                    newFolderName = newFolderName.Remove(index).Trim();
                }
                
                var indexUnderscore = newFolderName.LastIndexOf("_");
                if (indexUnderscore > 0)
                {
                    newFolderName = newFolderName.Remove(indexUnderscore).Trim();
                }
                
                var ext = $"{filename}{Path.GetExtension(file)}";
                var path = Path.Combine(config.RootFolder, newFolderName);
                Directory.CreateDirectory(path);
                Thread.Sleep(10000);
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