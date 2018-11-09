// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using GpioMonitor.Models;
using Microsoft.AspNetCore.SignalR;

namespace GpioMonitor.Hubs
{
    public class SensorHub : Hub
    {
        public Task Broadcast(string sender, Measurement measurement)
        {
            return Clients
                // Do not Broadcast to Caller:
                .AllExcept(new[] { Context.ConnectionId })
                // Broadcast to all connected clients:
                .SendAsync("Broadcast", sender, measurement);
                //.InvokeAsync("Broadcast", sender, measurement);
        }
    }
}
