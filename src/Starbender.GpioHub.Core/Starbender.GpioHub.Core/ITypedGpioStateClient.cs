using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GpioMonitor.Models;

namespace GpioMonitor
{
    public interface ITypedGpioStateClient
    {
        Task ReceiveGpioState(DateTime timestamp,int pinId,bool isAnalog, int value);
    }
}