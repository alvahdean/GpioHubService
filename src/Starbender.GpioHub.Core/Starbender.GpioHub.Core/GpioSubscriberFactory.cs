using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using GpioMonitor.Models;
using Microsoft.Extensions.Logging;

namespace GpioMonitor
{

    public static class GpioSubscriberFactory
    {
        public static IGpioStateSubscriber Create() => Create<GpioStateSubscriber>();

        public static IGpioStateSubscriber Create(string hubUrl, ILogger log) => Create<GpioStateSubscriber>(hubUrl,log);

        public static IGpioStateSubscriber Create<TSub>()
            where TSub : GpioStateSubscriber, new() => new TSub();

        public static IGpioStateSubscriber Create<TSub>(string hubUrl, ILogger log)
            where TSub : GpioStateSubscriber, new() => new TSub()
            {
                HubUrl = hubUrl,
                Logger = log
            };
    }
}
