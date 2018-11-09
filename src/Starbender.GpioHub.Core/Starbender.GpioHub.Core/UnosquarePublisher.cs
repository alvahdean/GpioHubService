using GpioMonitor.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        override protected async Task<GpioState> readPinAsync(int pinId)
        {
            GpioState result = null;
            GpioPin pin = controller.GetGpioPinByBcmPinNumber(pinId);
            if (pin != null)
            {
                result = new GpioState(pinId, 0, DateTime.Now, true);
                result.Value = await pin.ReadLevelAsync();
            }
            return result;
        }
    }
}
