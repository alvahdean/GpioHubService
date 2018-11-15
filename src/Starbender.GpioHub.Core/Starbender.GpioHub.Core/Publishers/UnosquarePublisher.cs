using GpioMonitor.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace GpioMonitor
{
    public class UnoSquarePublisher : GpioStatePublisher
    {
        private static GpioController controller=GpioController.Instance;

        public UnoSquarePublisher(): this(null, null) { }

        public UnoSquarePublisher(string hubUrl, ILogger log) : base(hubUrl,log)
        {
        }

        override protected async Task<GpioState> readPinAsync(WiringPiPin pin)
        {
            GpioState result = null;
            if (IsValidPin(pin))
            {
                result = new GpioState(pin, 0, DateTime.Now);
                result.Value = await Pi.Gpio[pin].ReadValueAsync();
            }
            return result;
        }
    }
}
