using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using GpioMonitor.Models;
using Microsoft.Extensions.Logging;

namespace GpioMonitor
{

    public class GpioStateSubscriber : GpioStateConnection, IGpioStateSubscriber
    {
        private object _syncRoot = new object();
        private List<int> _subscriptions = new List<int>();
        
        public GpioStateSubscriber()
            : this(null,null) { }

        public GpioStateSubscriber(string hubUrl,ILogger log)
            : base(hubUrl, log) { }

        public override async Task ConnectAsync(Uri uri)
        {
            await base.ConnectAsync(uri);
            if (IsConnected)
            {
                _conn.On<DateTime, int, bool, int>("GpioState", (timestamp, pinId, isAnalog, value) =>
                {
                    onGpioStateChanged(new GpioState(pinId, value, timestamp, isAnalog));
                });

                _conn.On<GpioState>("GpioState", (state) =>
                {
                    onGpioStateChanged(state);
                });
            }
        }

        protected virtual void onGpioStateChanged(GpioState state)
        {
            if (IsSubscribed(state.PinId))
            {
                saveState(state);
                Logger?.LogInformation($"[GpioStateChanged]: {state}");
            }
        }

        public bool IsSubscribed(int pinId) 
            => _subscriptions.Contains(pinId);

        public IEnumerable<int> Subscriptions 
            => _subscriptions;

        public bool SubscribePin(int pinId)
        {
            if(!IsSubscribed(pinId) && IsValidPin(pinId))
            {
                _subscriptions.Add(pinId);
            }
            return IsSubscribed(pinId);
        }

        public void UnsubscribePin(int pinId)
        {
            if(IsSubscribed(pinId))
            {
                _subscriptions.Remove(pinId);
            }
        }

    }
}
