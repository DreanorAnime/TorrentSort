using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace TorrentSort
{
    public class Program
    {
        private const string MutexName = "TorrentSort";

        public static void Main(string[] args)
        {
            if (HasOpenInstance())
            {
                return;
            }

            Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json")));
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

        private static bool HasOpenInstance()
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
