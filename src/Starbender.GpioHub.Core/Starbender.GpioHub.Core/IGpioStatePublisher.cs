using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GpioMonitor.Models;
using Unosquare.RaspberryIO.Gpio;

namespace GpioMonitor
{
    public interface IGpioStatePublisher : IGpioStateConnection
    {
        IEnumerable<WiringPiPin> MonitorList { get; }
        bool IsMonitored(WiringPiPin pinId);
        bool MonitorPin(WiringPiPin pinId);
        void IgnorePin(WiringPiPin pinId);
        int UpdateMilliseconds { get; set; }
        Task StartMonitorAsync();
    }
}