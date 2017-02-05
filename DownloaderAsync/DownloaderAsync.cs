using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using Tools;

namespace DownloaderAsync
{
    public class DownloaderAsync
    {
        private Options _options;
        private List<Url> _urls;
        private Queue<Url> _toProcess;
        private Queue<Url> _processing;
        private int urlsProcessed = 0;
        private int urlsSuccess = 0;
        private int urlsFail = 0;
        private int filesWritten = 0;
        private int totalUrls;
        private Stopwatch stopwatch = new Stopwatch();
        public int MaxParallel { get; set; }

        public DownloaderAsync(string[] args)
        {
            Console.WriteLine("==========> START ASYNC DOWNLOADER <===========");

            stopwatch.Start();
            _options = new Options(args);
            Console.WriteLine($"Processing file: {_options.Input}.");
            Console.WriteLine($"Output files will be located in: {_options.Output}.");
            Console.WriteLine($"Performing {_options.MaxAsync} API calls at the same time.");
            
            MaxParallel = _options.MaxAsync;

            var urls = UrlExtractor.ExtractUrls(_options.Input).ToList();
            var subsetUrls = new List<Url>();
            foreach(var url in urls)
            {
                if((_options.Start != null && url.Id < _options.Start) || 
                    (_options.End != null && url.Id > _options.End))
                {
                    
                }
                else
                {
                    subsetUrls.Add(url);
                }
            }
            urls = subsetUrls;
            _toProcess = new Queue<Url>(urls);
            
            totalUrls = _toProcess.Count;
            Console.WriteLine($"Loaded {totalUrls} URLs for processing.");
            _processing = new Queue<Url>();
            
            for (int i = 0; i < MaxParallel; i++)
            {
                if (_toProcess.Count > 0)
                {
                    var dequeue = _toProcess.Dequeue();
                    _processing.Enqueue(dequeue);
                }
                else
                {
                    break;
                }
            }

            Process();
        }

        private void Process()
        {
            while (_processing.Count > 0)
            {
                var url = _processing.Dequeue();
                GetContent(url);
            }
        }

        async void GetContent(Url url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 0, 0, _options.Timeout);
                try
                {
                    Console.WriteLine($"Starting call out to URL #{url.Id}");
                    var response = await client.GetAsync(url.Uri);
                    var content = await response.Content.ReadAsStringAsync();
                    url.Content = content;
                    urlsSuccess++;
                    WriteToFile(url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"url #{url.Id} failed, error message: {ex.Message}.");
                    urlsFail++;
                }
                finally
                {
                    if (_toProcess.Count > 0)
                    {
                        var dequeued = _toProcess.Dequeue();
                        _processing.Enqueue(dequeued);
                        Process();
                    }
                    urlsProcessed++;
                    Tracker(url);
                }
            }
        }

        private void WriteToFile(Url url)
        {
            //write to file
            Console.WriteLine($"url #{url.Id}, returned {url.Content.Substring(0, 50)}.");
            filesWritten++;
            Tracker(url);
        }

        private void Tracker(Url url)
        {
            var processedPerc = urlsProcessed * 100 / totalUrls;
            var successPerc = urlsSuccess * 100 / totalUrls;
            var failPerc = urlsFail * 100 / totalUrls;
            var writtenPerc = (filesWritten - urlsFail) * 100 / totalUrls;
            if (urlsProcessed == totalUrls && filesWritten == urlsSuccess)
            {
                stopwatch.Stop();
                Console.WriteLine($"Url #{url.Id} is the last one to finish saving. " +
                    $"{processedPerc}% URL processed, {writtenPerc}% files saved, {successPerc}% URL succeeded, " +
                    $"{failPerc}% URL failed. Nothing more to process. Total time: {stopwatch.Elapsed}.");
                Console.WriteLine("==========> END <===========");
            }

            else
            {
                Console.WriteLine($"Url #{url.Id} just finished pricessing or saving. " +
                        $"{processedPerc}% URL processed, {writtenPerc}% files saved, {successPerc}% URL succeeded, " +
                        $"{failPerc}% URL failed. Still processing...");
            }
        }
    }
}
