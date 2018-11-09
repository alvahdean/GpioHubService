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

    public static class GpioPublisherFactory
    {
        public static IGpioStatePublisher Create<TPub>()
            where TPub : GpioStatePublisher, new() => new TPub();

        public static IGpioStatePublisher Create<TPub>(string hubUrl, ILogger log)
            where TPub : GpioStatePublisher, new() => new TPub() { HubUrl = hubUrl, Logger = log };

    }
}
