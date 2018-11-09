using System;
using System.Threading.Tasks;
using System.Windows;
using GpioMonitor;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace GpioClientConsole
{
    public static class Program
    {
        private static string hubUrl = "http://localhost:5000/GpioState";
        private static readonly ILogger logger = CreateLogger("GpioClient");
        public static void Main(string[] args)
        {
            IGpioStateSubscriber client = new GpioStateSubscriber(hubUrl, logger);
            client.SubscribePin(18);
            client.SubscribePin(21);
            //client.SubscribePin(18);
            //client.SubscribePin(18);
            Task.Run(() => client.Connect());
            Console.WriteLine("Listening for notifications from the hub...");
            Console.WriteLine("Press <Enter> to quit");
            Console.ReadLine();
            client.DisconnectAsync().Wait(5000);
            Console.WriteLine("Exiting program");
        }

        private static ILogger CreateLogger(string loggerName)
        {
            return new LoggerFactory()
                .AddConsole(LogLevel.Trace)
                .CreateLogger(loggerName);
        }

    }
}