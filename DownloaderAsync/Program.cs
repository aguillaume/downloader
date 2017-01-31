
using System;

namespace DownloaderAsync
{
    class Program
    {
        private static void Main(string[] args)
        {
            var downloader = new DownloaderAsync(args);

            Console.ReadKey();
        }
    }
}
