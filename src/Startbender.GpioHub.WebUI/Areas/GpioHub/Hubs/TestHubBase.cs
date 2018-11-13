using GpioMonitor.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Starbender.GpioHub.Hubs
{
    public class TestHubBase : Hub, ITestHubBase
    {
        private readonly string _defaultGroup;
        protected readonly ILogger logger;

        public TestHubBase() : this(null, null) { }
        public TestHubBase(string defaultGroup) : this(defaultGroup, null) { }
        public TestHubBase(ILogger log) : this(null, log) { }
        public TestHubBase(string defaultGroup, ILogger log)
        {
            _defaultGroup = !String.IsNullOrWhiteSpace(defaultGroup)
                ? defaultGroup.Trim()
                : "SignalR Users";
            logger = log ?? createLogger(GetType().Name);
        }

        protected virtual ILogger createLogger(string loggerName)
        {
            return new LoggerFactory()
                .AddConsole(LogLevel.Trace)
                .CreateLogger(loggerName);
        }

        public string DefaultGroup { get => _defaultGroup; }

        public override async Task OnConnectedAsync()
        {
            logger.LogDebug($"Connection[{Context.ConnectionId}]: Connected as User={Context.UserIdentifier}");
            await Groups.AddToGroupAsync(Context.ConnectionId, DefaultGroup);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            logger.LogDebug($"Connection[{Context.ConnectionId}]: Disconnected");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, DefaultGroup);
            await base.OnDisconnectedAsync(exception);
        }

        public virtual async Task JoinGroupAsync(string group)
        {
            logger.LogDebug($"Connection[{Context.ConnectionId}]: Joining group {group}");
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public virtual async Task LeaveGroupAsync(string group)
        {
            logger.LogDebug($"Connection[{Context.ConnectionId}]: Leaving group {group}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }
    }
}
