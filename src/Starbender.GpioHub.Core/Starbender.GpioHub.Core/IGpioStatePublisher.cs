using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GpioMonitor.Models;

namespace GpioMonitor
{
    public interface IGpioStatePublisher : IGpioStateConnection
    {
        IEnumerable<int> MonitorList { get; }
        bool IsMonitored(int pinId);
        bool MonitorPin(int pinId);
        void IgnorePin(int pinId);
        int UpdateMilliseconds { get; set; }
        Task StartMonitorAsync();
    }
}