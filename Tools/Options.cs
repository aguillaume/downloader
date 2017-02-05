using System;

namespace Tools
{
    public class Options
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public int Timeout { get; set; } = 10000;
        public int? Start { get; set; } = null;
        public int? End { get; set; } = null;
        public int MaxAsync { get; set; } = 10;

        public Options(string[] args)
        {
            if (args.Length < 2)
            {
                if (args.Length == 1 && (args[0] == "-h" || args[0] == "--help"))
                {
                    ShowOptions();
                    throw new Exception("Showing Help");
                }
                else
                {
                    ShowHelp();
                    throw new Exception("Specify Key and Value");
                }
            }

            for (var i = 0; i < args.Length; i += 2)
            {
                var value = args[i + 1];
                switch (args[i])
                {
                    case "-i":
                    case "--input":
                        Input = value;
                        break;
                    case "-t":
                    case "--timeout":
                        Timeout = int.Parse(value) * 1000;
                        break;
                    case "-o":
                    case "--output":
                        Output = value;
                        break;
                    case "-s":
                    case "--start":
                        Start = int.Parse(value);
                        break;
                    case "-e":
                    case "--end":
                        End = int.Parse(value);
                        break;
                    case "-m":
                    case "--max-async":
                        MaxAsync = int.Parse(value);
                        break;
                    default:
                        ShowHelp();
                        throw new Exception("Invalid Option");
                }
            }

            if (Input == null) throw new Exception("Input filepath is requited");
            if (Output == null) throw new Exception("Output path is requited");
        }

        private void ShowOptions()
        {
            Console.WriteLine("Use -i or --input to specify the filepath for the input file.");
            Console.WriteLine("Use -o or --output to specify the folderpath for the output files.");
            Console.WriteLine("Use -t or --timeout to specify the timeout for HTTP requests in seconds. Default is 10s.");
            Console.WriteLine("Use -s or --start to specify the start number of the URLs.");
            Console.WriteLine("Use -e or --end to specify the end number of the URLs.");
            Console.WriteLine("Use -m or --max-async to specify the number of URLs to be called at the same time.");
        }

        private void ShowHelp()
        {
            Console.WriteLine("Type -h or --help to view the available options");
        }
    }
}
