using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using GpioMonitor.Models;
using Microsoft.Extensions.Logging;
using System.Threading;
using Unosquare.RaspberryIO.Gpio;

namespace GpioMonitor
{
    public abstract class GpioStatePublisher : GpioStateConnection, IGpioStatePublisher
    {

        protected CancellationTokenSource cancelTokenSource { get; private set; } = new CancellationTokenSource();
        protected CancellationToken cancelToken => cancelTokenSource.Token;
        private List<WiringPiPin> _monitorList = new List<WiringPiPin>();

        public GpioStatePublisher()
            : this(null,null) { }

        public GpioStatePublisher(string hubUrl,ILogger log=null) 
            : base(hubUrl,log) { }
        
        abstract protected Task<GpioState> readPinAsync(WiringPiPin pinId);

        public bool IsMonitored(WiringPiPin pin) 
            => _monitorList.Contains(pin);

        public IEnumerable<WiringPiPin> MonitorList 
            => _monitorList;

        public bool MonitorPin(WiringPiPin pin)
        {
            if(!IsMonitored(pin) && IsValidPin(pin))
                _monitorList.Add(pin);
            return IsMonitored(pin);
        }

        public void IgnorePin(WiringPiPin pin)
        {
            if(IsMonitored(pin))
            {
                _monitorList.Remove(pin);
            }
        }

        public int UpdateMilliseconds { get; set; } = 2000;

        public async Task StartMonitorAsync()
        {
            if (!IsConnected)
            {
                Logger?.LogError("Can't start monitoring, not connected");
                return;
            }
            
            while (!cancelToken.IsCancellationRequested)
            {
                foreach (WiringPiPin pin in _monitorList)
                {
                    GpioState st = await readPinAsync(pin);
                    saveState(st);
                    Logger?.LogDebug($"[SEND] GpioState:{st}");
                    // Finally send the value:
                    await _conn.SendAsync("Broadcast", st.Timestamp, st.Pin, st.Value);
                }
                await Task.Delay(UpdateMilliseconds, cancelToken);

            }
            await _conn.DisposeAsync();
        }

    }
}
