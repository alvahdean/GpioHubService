using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using GpioMonitor.Models;
using Microsoft.Extensions.Logging;
using Unosquare.RaspberryIO.Gpio;

namespace GpioMonitor
{

    public class GpioStateSubscriber : GpioStateConnection, IGpioStateSubscriber
    {
        private object _syncRoot = new object();
        private List<WiringPiPin> _subscriptions = new List<WiringPiPin>();
        
        public GpioStateSubscriber()
            : this(null,null) { }

        public GpioStateSubscriber(string hubUrl,ILogger log)
            : base(hubUrl, log) { }

        public override async Task ConnectAsync(Uri uri)
        {
            await base.ConnectAsync(uri);
            if (IsConnected)
            {
                _conn.On<DateTime, WiringPiPin, GpioPinValue>("GpioState", (timestamp, pin, value) =>
                {
                    new GpioState(pin, value, timestamp);
                    onGpioStateChanged(new GpioState((WiringPiPin)pin, value, timestamp));
                });

                _conn.On<GpioState>("GpioState", (state) =>
                {
                    onGpioStateChanged(state);
                });
            }
        }

        protected virtual void onGpioStateChanged(GpioState state)
        {
            if (IsSubscribed(state.Pin))
            {
                saveState(state);
                Logger?.LogInformation($"[GpioStateChanged]: {state}");
            }
        }

        public bool IsSubscribed(WiringPiPin pinId) 
            => _subscriptions.Contains(pinId);

        public IEnumerable<WiringPiPin> Subscriptions 
            => _subscriptions;

        public bool SubscribePin(WiringPiPin pin)
        {
            if(!IsSubscribed(pin) && IsValidPin(pin))
            {
                _subscriptions.Add(pin);
            }
            return IsSubscribed(pin);
        }

        public void UnsubscribePin(WiringPiPin pin)
        {
            if(IsSubscribed(pin))
            {
                _subscriptions.Remove(pin);
            }
        }

    }
}
