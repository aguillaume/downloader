using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using Tools;

namespace downloader
{
    public class Downlowder
    {
        private static string date = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day} {DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}";
        private static List<Error> errorList = new List<Error>();
        private static Options options;
        private static List<string> urls = new List<string>();

        public static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                options = new Options(args);

                Console.WriteLine("===========================================");
                Console.WriteLine("Welcome to the downloder!");
                Console.WriteLine($"Reading file from {options.Input}.");
                GetUrls();

                var start = options.Start == null ? 0 : options.Start.Value;
                var end = options.End == null ? urls.Count : options.End.Value;
                for (var count = start; count < end; count++)
                {
                    DownloadSync(count);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (errorList.Count > 0)
                {
                    File.AppendAllLines($"{options.Output}\\errors-{date}.txt", Error.ToListString(errorList));
                }
                stopwatch.Stop();
                Console.WriteLine($"Time Taken: {stopwatch.Elapsed} Type any key to close...");
                Console.ReadLine();
            }
        }

        private static void DownloadSync(int count)
        {
            Console.WriteLine($"Call #{count + 1} starting...");
            HttpGet(urls[count], count + 1, out bool success);
            var message = success ? "ended successfully" : "failed";
            Console.WriteLine($"Call #{count + 1} {message}");
            File.AppendAllLines($"{options.Output}\\log{date}.txt", new List<string> { $"Item number {count + 1} {message}" });
        }

        private static void GetUrls()
        {
            using (StreamReader reader = new StreamReader(options.Input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    urls.Add(line); // Add to list.
                }
            }
            Console.WriteLine("Grabbed all URLs. Starting call outs.");
        }

        private static void HttpGet(string url, int count, out bool success)
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(url);
            request.Timeout = options.Timeout;
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                var reader = new StreamReader(dataStream);
                var all = reader.ReadToEnd();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        // Write the content.
                        var line1 = all.Substring(0, "<!DOCTYPE html>".Length);
                        if (line1.Equals("<!DOCTYPE html>"))
                        {
                            errorList.Add(new Error(count, response.StatusCode, "Something went wrong, but we were told everything was fine."));
                            success = false;
                        }
                        else
                        {
                            File.WriteAllText($"{options.Output}\\output{count}.csv", all);
                            success = true;
                        }
                        break;
                    default:
                        errorList.Add(new Error(count, response.StatusCode, all));
                        success = false;
                        break;
                }
                response.Close();
            }
            catch (Exception ex)
            {
                errorList.Add(new Error(count, HttpStatusCode.InternalServerError, ex.Message));
                success = false;
            }
        }
    }
}
