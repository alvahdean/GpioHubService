// Copyright (c) 2018 Dean Fuqua. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using GpioMonitor.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GpioMonitor
{
    public class Program
    {
        private static readonly ILogger logger = CreateLogger("GpioMonitor");
        private static string hubUrl = "http://localhost:5000/gpioState";
        private static List<int> pinList = new List<int>();
        private static readonly Random rnd = new Random();

        public static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddCommandLine(args)
                .Build();

            if (!String.IsNullOrWhiteSpace(config["hubUrl"]))
                hubUrl = config["hubUrl"];

            if (!String.IsNullOrWhiteSpace(config["pinList"]))
                pinList = JsonConvert.DeserializeObject<List<int>>(config["pinList"]);

            logger.LogInformation($"Configuration:");
            logger.LogInformation($"   hubUrl={hubUrl}");
            logger.LogInformation($"   pinList={String.Join(',', pinList)}");

            Task.Run(() => StartMonitor(cancellationTokenSource.Token).GetAwaiter().GetResult(), cancellationTokenSource.Token);

            Console.WriteLine("Press Enter to Exit ...");
            Console.ReadLine();
            cancellationTokenSource.Cancel();
        }

        private static async Task StartMonitor(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Connecting to SignalR hub @ {hubUrl}");

            IGpioStatePublisher monitor = GpioPublisherFactory.Create<MockPublisher>(hubUrl, logger);
            monitor.UpdateMilliseconds = 3000;
            foreach (int pin in pinList)
                monitor.MonitorPin(pin);
            await monitor.ConnectAsync();
            await monitor.StartMonitorAsync();
            await monitor.Connection.DisposeAsync();
        }

        private static ILogger CreateLogger(string loggerName)
        {
            return new LoggerFactory()
                .AddConsole(LogLevel.Trace)
                .CreateLogger(loggerName);
        }
    }
}
