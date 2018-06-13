using System.IO;
using System.Reflection;
using TorrentSortDLL;

namespace TorrentSort
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Sorter sorter = new Sorter(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            sorter.Sort();
        }
    }
}