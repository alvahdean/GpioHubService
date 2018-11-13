using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starbender.GpioHub.Hubs
{
    public interface ITestHubBase
    {
        Task JoinGroupAsync(string group);
        Task LeaveGroupAsync(string group);
        Task OnConnectedAsync();
        Task OnDisconnectedAsync(Exception exception);
    }
}