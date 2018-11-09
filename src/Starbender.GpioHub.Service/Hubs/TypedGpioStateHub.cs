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
    public class TypedGpioStateHub : TestHubBase<ITypedGpioStateClient>
    {
        public TypedGpioStateHub() : base() { }

        public async Task Broadcast(DateTime timestamp,int pinId,bool isAnalog,int value)
        {
            await Clients.All.ReceiveGpioState(timestamp, pinId, isAnalog, value);
        }

        public async Task BroadcastOthers(DateTime timestamp, int pinId, bool isAnalog, int value)
        {
            await Clients.Others.ReceiveGpioState(timestamp, pinId, isAnalog, value);
        }

        public virtual async Task SendToCaller(DateTime timestamp, int pinId, bool isAnalog, int value)
        {
            await Clients.Caller.ReceiveGpioState(timestamp, pinId, isAnalog, value);
        }

        public virtual async Task SendToGroup(string groupName, DateTime timestamp, int pinId, bool isAnalog, int value)
        {
            await SendToGroups(new List<string>() { groupName }, timestamp, pinId, isAnalog, value);
        }

        public virtual async Task SendToGroups(IEnumerable<string> groupNames, DateTime timestamp, int pinId, bool isAnalog, int value)
        {
            List<string> groups = new List<string>(groupNames);
            await Clients.Groups(groups).ReceiveGpioState(timestamp, pinId, isAnalog, value);
        }
    }
}
