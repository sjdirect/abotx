﻿using System;
using Abot.Crawler;
using Abot.Poco;
using AbotX.Crawler;
using AbotX.Poco;
using log4net.Repository.Hierarchy;

namespace abotx_usage
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            var config = new CrawlConfigurationX
            {
                IsJavascriptRenderingEnabled = true,
                JavascriptRenderingWaitTimeInMilliseconds = 3000,
                MaxPagesToCrawl = 1, 
                MaxConcurrentThreads = 1       
            };
            var crawler = new CrawlerX(config);
            crawler.PageCrawlCompleted += Crawler_PageCrawlCompleted;

            var result = crawler.CrawlAsync(new Uri("https://www.google.com/search?q=dogs")).Result;

            Console.ReadLine();
        }

        private static void Crawler_PageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            if (e.CrawledPage.Content.Text.Contains("dogs"))
            {
                //it worked
            }
            else
            {
                throw new Exception("Javascript rendering did not work");
            }
        }
    }
}