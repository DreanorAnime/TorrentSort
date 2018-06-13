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
            Regex regexFolder = new Regex(config.FolderRegex);
            Regex regexWhitespace = new Regex("[ ]{2,}");

            foreach (var file in Directory.GetFiles(config.RootFolder, config.FileFilter))
            {
                var filename = Path.GetFileNameWithoutExtension(file);

                foreach (Match match in regexFilename.Matches(filename))
                {
                    filename = filename.Replace(match.Value, string.Empty).Trim();
                }

                var folderNameWhitespaces = regexFolder.Replace(filename, string.Empty);
                var folderNameFinal = regexWhitespace.Replace(folderNameWhitespaces, " ");

                var ext = $"{filename}{Path.GetExtension(file)}";
                var path = Path.Combine(config.RootFolder, folderNameFinal);
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
