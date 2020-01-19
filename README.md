# AbotX [![Build Status](https://dev.azure.com/sjdirect0945/AbotX/_apis/build/status/AbotX%20CI?branchName=master)](https://dev.azure.com/sjdirect0945/AbotX/_build/latest?definitionId=1&branchName=master) [![NuGet](https://img.shields.io/nuget/v/Abotx.svg)](https://www.nuget.org/packages/Abotx/)

A powerful C# web crawler that makes advanced crawling features easy to use. AbotX builds upon the [open source Abot C# Web Crawler](https://github.com/sjdirect/abot) by providing a powerful set of wrappers and extensions. 

## Features
* Crawl multiple sites concurrently
* Pause/resume live crawls
* Render javascript before processing
* Simplified pluggability/extensibility
* Avoid getting blocked by sites
* Automatically tune speed/concurrency

AbotX has both free and paid features. See licensing info at the bottom of this page for more information.

## Technical Details
* Version 2.x targets .NET Standard 2.0 (compatible with .NET framework 4.6.1+ or .NET Core 2+)
* Version 1.x targets .NET Framework 4.0 (support ends soon, please upgrade)

## Installing AbotX
  * Install AbotX using [Nuget](https://www.nuget.org/packages/Abotx/)
  
```command
PM> Install-Package AbotX
```

## Quick Start 

AbotX adds advanced functionality, shortcuts and configurations to the rock solid [Abot C# Web Crawler](https://github.com/sjdirect/abot). It is recommended that you start with Abot's documentation and quick start before coming here. 

AbotX consists of the two main entry points. They are CrawlerX and ParallelCrawlerEngine. CrawlerX is a single crawler instance (child of Abot's PoliteWebCrawler class) while ParallelCrawlerEngine creates and manages multiple instances of CrawlerX. If you want to just crawl a single site then CrawlerX is where you want to start. If you want to crawl a configurable number of sites concurrently within the same process then the ParallelCrawlerEngine is what you are after. 

### Using AbotX
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
## CrawlerX
CrawlerX is an object that represents an individual crawler that crawls a single site at a time. It is a subclass of Abot's PoliteWebCrawler and adds some useful functionality.

### Example Usage
Create an instance and register for events...
```c#
var crawler = new CrawlerX();
crawler.PageCrawlStarting += crawler_ProcessPageCrawlStarting;
crawler.PageCrawlCompleted += crawler_ProcessPageCrawlCompleted;
crawler.PageCrawlDisallowed += crawler_PageCrawlDisallowed;
crawler.PageLinksCrawlDisallowed += crawler_PageLinksCrawlDisallowed;
```
Working with some common events...
```c#
void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
{
    PageToCrawl pageToCrawl = e.PageToCrawl;
    Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri,   pageToCrawl.ParentUri.AbsoluteUri);
}

void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
{
    CrawledPage crawledPage = e.CrawledPage;

    if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
        Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
    else
        Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);

    if (string.IsNullOrEmpty(crawledPage.Content.Text))
        Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
}

void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
{
    CrawledPage crawledPage = e.CrawledPage;
    Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
}

void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
{
    PageToCrawl pageToCrawl = e.PageToCrawl;
    Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
}
```
Run the crawl synchronously
```c#
var result = crawler.Crawl(new Uri("YourSiteHere"));
```
Run the crawl asynchronously
```c#
var result = await crawler.CrawlAsync(new Uri("YourSiteHere"));
```
### Easy Override
CrawlerX has default implementations for all its dependencies. However, there are times where you may want to override one or all of those implementations. Below is an example of how you would plugin your own implementations. The new ImplementationOverride class makes plugging in nested dependencies much easier than it use to be with Abot. It will handle finding exactly where that implementation is needed.

```c#
var impls = new ImplementationOverride(config, ImplementationContainer {
    HyperlinkParser = new YourImpl1(),
    PageRequester = new YourImpl2()
});

var crawler = new CrawlerX(config, impls);
```
### Pause And Resume
Pause and resume work as you would expect. However, just be aware that any in progress http requests will be finished, processed and any events related to those will be fired.
```c#
var crawler = new CrawlerX();

crawler.PageCrawlCompleted += (sender, args) =>
{
    //You will be interested in args.CrawledPage & args.CrawlContext
};

var crawlerTask = crawler.CrawlAsync(new Uri("http://blahblahblah.com"));

System.Threading.Thread.Sleep(3000);
crawler.Pause();
System.Threading.Thread.Sleep(10000);
crawler.Resume();

var result = crawlerTask.Result;
```

### Stop
Stopping the crawl is as simple as calling Stop(). The call to Stop() tells AbotX to not make any new http requests but to finish any that are in progress. Any events and processing of the in progress requests will finish before CrawlerX stops the crawl.
```c#
var crawler = new CrawlerX();

crawler.PageCrawlCompleted += (sender, args) =>
{
    //You will be interested in args.CrawledPage & args.CrawlContext
};

var crawlerTask = crawler.CrawlAsync(new Uri("http://blahblahblah.com"));

System.Threading.Thread.Sleep(3000);
crawler.Stop();
var result = crawlerTask.Result;
```
By passing true to the Stop() method, AbotX will stop the crawl more abruptly. Anything in pogress will be aborted.
```c#
crawler.Stop(true);
```
### Speed Up
CrawlerX can be "sped up" by calling the SpeedUp() method. The call to SpeedUp() tells AbotX to increase the number of concurrent http requests to the currently running sites. You can can call this method as many times as you like. Adjustments are made instantly so you should see more concurrency immediately.

```c#
crawler.CrawlAsync(new Uri("http://localhost:1111/"));

System.Threading.Thread.Sleep(3000);
crawler.SpeedUp();

System.Threading.Thread.Sleep(3000);
crawler.SpeedUp();
```
See the "Configure Speed Up And Slow Down" section for more details on how to control exactly what happens when SpeedUp() is called.

### Slow Down

CrawlerX can be "slowed down" by calling the SlowDown() method. The call to SlowDown() tells AbotX to reduce the number of concurrent http requests to the currently runnning sites. You can can call this method as many times as you like. Any currently executing http requests will finish normally before any adjustments are made.

```c#
crawler.CrawlAsync(new Uri("http://localhost:1111/"));

System.Threading.Thread.Sleep(3000);
crawler.SlowDown();

System.Threading.Thread.Sleep(3000);
crawler.SlowDown();
```
See the "Configure Speed Up And Slow Down" section for more details on how to control exactly what happens when SlowDown() is called.

## Parallel Crawler Engine
A crawler instance can crawl a single site quickly. However, if you have to crawl 10,000 sites quickly you need the ParallelCrawlerEngine. It allows you to crawl a configurable number of sites concurrently to maximize throughput.

### Example Usage
The concurrency is configurable by setting the maxConcurrentSiteCrawls in the config. The default value is 3 so the following block of code will crawl three sites simultaneously.
```c#
static void Main(string[] args)
{
    var siteToCrawlProvider = new SiteToCrawlProvider();
    siteToCrawlProvider.AddSitesToCrawl(new List<SiteToCrawl>
    {
        new SiteToCrawl{ Uri = new Uri("http://somesitetocrawl1.com/") },
        new SiteToCrawl{ Uri = new Uri("http://somesitetocrawl2.com/") },
        new SiteToCrawl{ Uri = new Uri("http://somesitetocrawl3.com/") },
    });

    //Create the crawl engine instance
    var impls = new ParallelImplementationOverride(
        config,
        new ParallelImplementationContainer
        {
            SiteToCrawlProvider = siteToCrawlProvider
            WebCrawlerFactory = yourWebCrawlerFactory //YOU NEED TO IMPLEMENT THIS!!!!
        }
    );

    var crawlEngine = new ParallelCrawlerEngine(config, impls);

    //Register for site level events
    crawlEngine.AllCrawlsCompleted += (sender, eventArgs) =>
    {
        Console.WriteLine("Completed crawling all sites");
    };
    crawlEngine.SiteCrawlCompleted += (sender, eventArgs) =>
    {
        Console.WriteLine("Completed crawling site {0}", eventArgs.CrawledSite.SiteToCrawl.Uri);       
    };
    crawlEngine.CrawlerInstanceCreated += (sender, eventArgs) =>
    {
        //Register for crawler level events. These are Abot's events!!!
        eventArgs.Crawler.PageCrawlCompleted += (abotSender, abotEventArgs) =>
        {
            Console.WriteLine("You have the crawled page here in abotEventArgs.CrawledPage...");
        };
    };

    crawlEngine.StartAsync();

    Console.WriteLine("Press enter key to stop");
    Console.Read();
}
```
### Easy Override Of Default Implementations
ParallelCrawlerEngine allows easy override of one or all of it's dependent implementations. Below is an example of how you would plugin your own implementations (same as above). The new ParallelImplementationOverride class makes plugging in nested dependencies much easier than it use to be. It will handle finding exactly where that implementation is needed.

```c#
var impls = new ParallelImplementationOverride(config, new ImplementationContainer {
    SiteToCrawlProvider = yourSiteToCrawlProvider,
    WebCrawlerFactory = yourFactory,
        ...(Excluded)
});

var crawlEngine = new ParallelCrawlerEngine(config, impls);
```

### Pause And Resume
Pause and resume on the ParallelCrawlerEngine simply relays the command to each active CrawlerX instance. However, just be aware that any in progress http requests will be finished, processed and any events related to those will be fired.

```c#
crawlEngine.StartAsync();

System.Threading.Thread.Sleep(3000);
crawlEngine.Pause();
System.Threading.Thread.Sleep(10000);
crawlEngine.Resume();
```

### Stop
Stopping the crawl is as simple as calling Stop(). The call to Stop() tells AbotX to not make any new http requests but to finish any that are in progress. Any events and processing of the in progress requests will finish before each CrawlerX instance stops its crawl as well.

```c#
crawlEngine.StartAsync();

System.Threading.Thread.Sleep(3000);
crawlEngine.Stop();
```

By passing true to the Stop() method, it will stop each CrawlerX instance more abruptly. Anything in pogress will be aborted.

```c#
crawlEngine.Stop(true);
```

### Speed Up
The ParallelCrawlerEngine can be "sped up" by calling the SpeedUp() method. The call to SpeedUp() tells AbotX to increase the number of concurrent site crawls that are currently running. You can can call this method as many times as you like. Adjustments are made instantly so you should see more concurrency immediately.

```c#
crawlEngine.StartAsync();

System.Threading.Thread.Sleep(3000);
crawlEngine.SpeedUp();

System.Threading.Thread.Sleep(3000);
crawlEngine.SpeedUp();
```

See the "Configure Speed Up And Slow Down" section for more details on how to control exactly what happens when SpeedUp() is called.

### Slow Down
The ParallelCrawlerEngine can be "slowed down" by calling the SlowDown() method. The call to SlowDown() tells AbotX to reduce the number of concurrent site crawls that are currently running. You can can call this method as many times as you like. Any currently executing crawls will finish normally before any adjustments are made.

```c#
crawlEngine.StartAsync();

System.Threading.Thread.Sleep(3000);
crawlEngine.SlowDown();

System.Threading.Thread.Sleep(3000);
crawlEngine.SlowDown();
```

See the "Configure Speed Up And Slow Down" section for more details on how to control exactly what happens when SlowDown() is called.



## Configure Speed Up And Slow Down
Multiple features trigger AbotX to speed up or to slow down crawling. The Accelerator and Decelerator are two indipendently configurable components that determine exactly how agressively AbotX reacts to a situation that triggers a SpeedUp or SlowDown. The default works fine for most cases but the following are options you have to take further control.

### Accelerator

Name | Description | Used By
--- | --- | ---
config.Accelerator.ConcurrentSiteCrawlsIncrement | The number to increment the MaxConcurrentSiteCrawls for each call the the SpeedUp() method. This deals with site crawl concurrency, NOT the number of concurrent http requests to a single site crawl. | ParallelCrawlerEngine
config.Accelerator.ConcurrentRequestIncrement	| The number to increment the MaxConcurrentThreads for each call the the SpeedUp() method. This deals with the number of concurrent http requests for a single crawl. |	CrawlerX
config.Accelerator.DelayDecrementInMilliseconds	| If there is a configured (manual or programatically determined) delay in between requests to a site, this is the amount of milliseconds to remove from that configured value on every call to the SpeedUp() method.	| CrawlerX
config.Accelerator.MinDelayInMilliseconds |	If there is a configured (manual or programatically determined) delay in between requests to a site, this is the minimum amount of milliseconds to delay no matter how many calls to the SpeedUp() method. |	CrawlerX
config.Accelerator.ConcurrentSiteCrawlsMax	| The maximum amount of concurrent site crawls to allow no matter how many calls to the SpeedUp() method.	| ParallelCrawlerEngine
config.Accelerator.ConcurrentRequestMax	| The maximum amount of concurrent http requests to a single site no matter how many calls to the SpeedUp() method.	| CrawlerX

### Decelerator

Name | Description | Used By
--- | --- | ---
config.Decelerator.ConcurrentSiteCrawlsDecrement |	The number to decrement the MaxConcurrentSiteCrawls for each call the the SlowDown() method. This deals with site crawl concurrency, NOT the number of concurrent http requests to a single site crawl.	| ParallelCrawlerEngine
config.Decelerator.ConcurrentRequestDecrement	| The number to decrement the MaxConcurrentThreads for each call the the SlowDown() method. This deals with the number of concurrent http requests for a single crawl. |	CrawlerX
config.Decelerator.DelayIncrementInMilliseconds |	If there is a configured (manual or programatically determined) delay in between requests to a site, this is the amount of milliseconds to add to that configured value on every call to the SlowDown() method	CrawlerX
config.Decelerator.MaxDelayInMilliseconds	| The maximum value the delay can be.	| CrawlerX
config.Decelerator.ConcurrentSiteCrawlsMin |	The minimum amount of concurrent site crawls to allow no matter how many calls to the SlowDown() method.	| ParallelCrawlerEngine
config.Decelerator.ConcurrentRequestMin |	The minimum amount of concurrent http requests to a single site no matter how many calls to the SlowDown() method.	| CrawlerX


#### Javascript Rendering

#### Auto Throttling

#### Auto Tuning

## Paid License
All plans except AbotX Basic require a [paid license](https://abotx.org/Buy/Pricing) after the 30 day trial. After the purchase you will receive an AbotX.lic file. This file must reside in the same directory as the AbotX.dll file during execution.

Please remember that your AbotX.lic file should not be shared outside the organization that purchased the license. This especially means not sharing it in online forums, blogs or packaging it in other software packages that are distributed to end users. Please contact us directly for redistribution requests.
<br /><br /><br />
<hr />

