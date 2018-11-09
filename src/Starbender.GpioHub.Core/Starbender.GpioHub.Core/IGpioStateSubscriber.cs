using System;
using System.Collections.Generic;
using GpioMonitor.Models;

namespace GpioMonitor
{
    public interface IGpioStateSubscriber: IGpioStateConnection
    {
        IEnumerable<int> Subscriptions { get; }
        bool IsSubscribed(int pinId);
        bool SubscribePin(int pinId);
        void UnsubscribePin(int pinId);
    }
}