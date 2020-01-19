# AbotX [![Build Status](https://dev.azure.com/sjdirect0945/AbotX/_apis/build/status/AbotX%20CI?branchName=master)](https://dev.azure.com/sjdirect0945/AbotX/_build/latest?definitionId=1&branchName=master) [![NuGet](https://img.shields.io/nuget/v/Abotx.svg)](https://www.nuget.org/packages/Abotx/)

A powerful C# web crawler that makes advanced crawling features easy to use. AbotX builds upon the [open source Abot C# Web Crawler](https://github.com/sjdirect/abot) by providing a powerful set of wrappers and extensions. 

##### Features
* Crawl multiple sites concurrently
* Pause/resume live crawls
* Render javascript before processing
* Simplified pluggability/extensibility
* Avoid getting blocked by sites
* Automatically tune speed/concurrency

##### Technical Details
* Version 2.x targets .NET Standard 2.0 (compatible with .NET framework 4.6.1+ or .NET Core 2+)
* Version 1.x targets .NET Framework 4.0 (support ends soon, please upgrade)

## Quick Start 

AbotX adds advanced functionality, shortcuts and configurations to the rock solid [Abot C# Web Crawler](https://github.com/sjdirect/abot). It is recommended that you start with Abot's documentation and quick start before coming here. 

AbotX consists of the two main entry points. They are CrawlerX and ParallelCrawlerEngine. CrawlerX is a single crawler instance (child of Abot's PoliteWebCrawler class) while ParallelCrawlerEngine creates and manages multiple instances of CrawlerX. If you want to just crawl a single site then CrawlerX is where you want to start. If you want to crawl a configurable number of sites concurrently within the same process then the ParallelCrawlerEngine is what you are after. 

###### Installing AbotX
  * Install AbotX using [Nuget](https://www.nuget.org/packages/Abotx/)

###### Using AbotX
```c#
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Abot2;
using AbotX2.Crawler;
using AbotX2.Parallel;
using AbotX2.Poco;
using Serilog;

namespace AbotX2.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Use Serilog to log
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithThreadId()
                .WriteTo.Console(outputTemplate: Constants.LogFormatTemplate)
                .CreateLogger();

            var siteToCrawl = new Uri("YourSiteHere");

            //Uncomment to demo major features
            //await DemoCrawlerX_PauseResumeStop(siteToCrawl);
            //await DemoCrawlerX_JavascriptRendering(siteToCrawl);
            //await DemoCrawlerX_AutoTuning(siteToCrawl);
            //await DemoCrawlerX_Throttling(siteToCrawl);
            //await DemoParallelCrawlerEngine();
        }

        private static async Task DemoCrawlerX_PauseResumeStop(Uri siteToCrawl)
        {
            using (var crawler = new CrawlerX(GetSafeConfig()))
            {
                crawler.PageCrawlCompleted += (sender, args) =>
                {
                    //Check out args.CrawledPage for any info you need
                };
                var crawlTask = crawler.CrawlAsync(siteToCrawl);

                crawler.Pause();    //Suspend all operations

                Thread.Sleep(7000);

                crawler.Resume();   //Resume as if nothing happened

                crawler.Stop(true); //Stop or abort the crawl

                await crawlTask;
            }
        }

        private static async Task DemoCrawlerX_JavascriptRendering(Uri siteToCrawl)
        {
            var pathToPhantomJSExeFolder = @"[YourNugetPackagesLocationAbsolutePath]\PhantomJS.2.1.1\tools\phantomjs]";
            var config = new CrawlConfigurationX
            {
                IsJavascriptRenderingEnabled = true,
                JavascriptRendererPath = pathToPhantomJSExeFolder,
                IsSendingCookiesEnabled = true,
                MaxConcurrentThreads = 1,
                MaxPagesToCrawl = 1,
                JavascriptRenderingWaitTimeInMilliseconds = 3000,
                CrawlTimeoutSeconds = 20
            };

            using (var crawler = new CrawlerX(config))
            {
                crawler.PageCrawlCompleted += (sender, args) =>
                {
                    //JS should be fully rendered here args.CrawledPage.Content.Text
                };

                await crawler.CrawlAsync(siteToCrawl);
            }
        }

        private static async Task DemoCrawlerX_AutoTuning(Uri siteToCrawl)
        {
            var config = GetSafeConfig();
            config.AutoTuning = new AutoTuningConfig
            {
                IsEnabled = true,
                CpuThresholdHigh = 85,
                CpuThresholdMed = 65,
                MinAdjustmentWaitTimeInSecs = 10
            };
            //Optional, configure how aggressively to speed up or down during throttling
            config.Accelerator = new AcceleratorConfig();
            config.Decelerator = new DeceleratorConfig();

            //Now the crawl is able to "AutoTune" itself if the host machine
            //is showing signs of stress.
            using (var crawler = new CrawlerX(config))
            {
                crawler.PageCrawlCompleted += (sender, args) =>
                {
                    //Check out args.CrawledPage for any info you need
                };
                await crawler.CrawlAsync(siteToCrawl);
            }
        }

        private static async Task DemoCrawlerX_Throttling(Uri siteToCrawl)
        {
            var config = GetSafeConfig();
            config.AutoThrottling = new AutoThrottlingConfig
            {
                IsEnabled = true,
                ThresholdHigh = 2,
                ThresholdMed = 2,
                MinAdjustmentWaitTimeInSecs = 10
            };
            //Optional, configure how aggressively to speed up or down during throttling
            config.Accelerator = new AcceleratorConfig();
            config.Decelerator = new DeceleratorConfig();

            //Now the crawl is able to "Throttle" itself if the site being crawled
            //is showing signs of stress.
            using (var crawler = new CrawlerX(config))
            {
                crawler.PageCrawlCompleted += (sender, args) =>
                {
                    //Check out args.CrawledPage for any info you need
                };
                await crawler.CrawlAsync(siteToCrawl);
            }
        }

        private static async Task DemoParallelCrawlerEngine()
        {
            var siteToCrawlProvider = new SiteToCrawlProvider();
            siteToCrawlProvider.AddSitesToCrawl(new List<SiteToCrawl>
            {
                new SiteToCrawl{ Uri = new Uri("YOURSITE1") },
                new SiteToCrawl{ Uri = new Uri("YOURSITE2") },
                new SiteToCrawl{ Uri = new Uri("YOURSITE3") },
                new SiteToCrawl{ Uri = new Uri("YOURSITE4") },
                new SiteToCrawl{ Uri = new Uri("YOURSITE5") }
            });

            var config = GetSafeConfig();
            config.MaxConcurrentSiteCrawls = 3;

            var crawlEngine = new ParallelCrawlerEngine(
                config, 
                new ParallelImplementationOverride(config)
                {
                    SiteToCrawlProvider = siteToCrawlProvider,
                    WebCrawlerFactory = new WebCrawlerFactory(config)//Same config will be used for every crawler instance created
                });
            
            var crawlCounts = new Dictionary<Guid, int>();
            var siteStartingEvents = 0;
            var allSitesCompletedEvents = 0;
            crawlEngine.CrawlerInstanceCreated += (sender, eventArgs) =>
            {
                var crawlId = Guid.NewGuid();
                eventArgs.Crawler.CrawlBag.CrawlId = crawlId;
            };
            crawlEngine.SiteCrawlStarting += (sender, args) =>
            {
                Interlocked.Increment(ref siteStartingEvents);
            };
            crawlEngine.SiteCrawlCompleted += (sender, eventArgs) =>
            {
                lock (crawlCounts)
                {
                    crawlCounts.Add(eventArgs.CrawledSite.SiteToCrawl.Id, eventArgs.CrawledSite.CrawlResult.CrawlContext.CrawledCount);
                }
            };
            crawlEngine.AllCrawlsCompleted += (sender, eventArgs) =>
            {
                Interlocked.Increment(ref allSitesCompletedEvents);
            };

            await crawlEngine.StartAsync();
        }

        private static CrawlConfigurationX GetSafeConfig()
        {
            /*The following settings will help not get your ip banned
             by the sites you are trying to crawl. The idea is to crawl
             only 5 pages and wait 2 seconds between http requests
             */
            return new CrawlConfigurationX
            {
                MaxPagesToCrawl = 10,
                MinCrawlDelayPerDomainMilliSeconds = 2000
            };
        }
    }
}

```

<br /><br /><br />
<hr />

