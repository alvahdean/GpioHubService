using System;
using System.Collections.Generic;
using GpioMonitor.Models;
using Unosquare.RaspberryIO.Gpio;

namespace GpioMonitor
{
    public interface IGpioStateSubscriber: IGpioStateConnection
    {
        IEnumerable<WiringPiPin> Subscriptions { get; }
        bool IsSubscribed(WiringPiPin pin);
        bool SubscribePin(WiringPiPin pin);
        void UnsubscribePin(WiringPiPin pin);
    }
}