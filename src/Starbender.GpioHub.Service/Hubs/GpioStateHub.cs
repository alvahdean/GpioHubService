using GpioMonitor.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GpioMonitor;
using System.Threading;

namespace GpioStateServer.Hubs
{
    public class GpioStateHub : TestHubBase
    {
        public GpioStateHub() : base() { }
        //public GpioStateHub(ILogger log) : base(log) { }

        public async Task Broadcast(DateTime timestamp, int pinId, bool isAnalog, int value)
        {
            GpioState st = new GpioState() { PinId = pinId, IsAnalog = isAnalog, Value = value };
            logger.LogDebug($"Broadcasting to all other clients ...");
            IHubCallerClients hubClients = Clients;
            IClientProxy clients = Clients.All;
            IClientProxy all = Clients.All;
            IClientProxy others = Clients.Others;
            logger.LogDebug($"[GPIOState.Hub]; {st}");
            await others.SendAsync("GpioState", timestamp, pinId, isAnalog, value);
        }

        public async Task BroadcastOthers(DateTime timestamp, int pinId, bool isAnalog, int value)
        {
            GpioState st = new GpioState() { PinId = pinId, IsAnalog = isAnalog, Value = value };
            logger.LogDebug($"Broadcasting to all other clients ...");
            IHubCallerClients hubClients = Clients;
            IClientProxy clients = Clients.All;
            IClientProxy all = Clients.All;
            IClientProxy others = Clients.Others;
            logger.LogDebug($"[GPIOState.Hub]; {st}");
            
            await others.SendAsync("GpioState",timestamp, pinId, isAnalog, value);
        }

        public virtual async Task SendToCaller(DateTime timestamp, int pinId, bool isAnalog, int value)
        {
            await Clients.Caller.SendAsync("GpioState",timestamp, pinId, isAnalog, value);
        }

        public virtual async Task SendToGroup(string groupName, DateTime timestamp, int pinId, bool isAnalog, int value)
        {
            await SendToGroups(new List<string>() { groupName }, timestamp, pinId, isAnalog, value);
        }

        public virtual async Task SendToGroups(IEnumerable<string> groupNames, DateTime timestamp, int pinId, bool isAnalog, int value)
        {
            List<string> groups = new List<string>(groupNames);
            await Clients.Groups(groups).SendAsync("GpioState", timestamp, pinId, isAnalog, value);
        }
    }
}
