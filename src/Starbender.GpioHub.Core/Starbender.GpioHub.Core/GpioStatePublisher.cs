using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using GpioMonitor.Models;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace GpioMonitor
{
    public abstract class GpioStatePublisher : GpioStateConnection, IGpioStatePublisher
    {

        protected CancellationTokenSource cancelTokenSource { get; private set; } = new CancellationTokenSource();
        protected CancellationToken cancelToken => cancelTokenSource.Token;
        private List<int> _monitorList = new List<int>();

        public GpioStatePublisher()
            : this(null,null) { }

        public GpioStatePublisher(string hubUrl,ILogger log=null) 
            : base(hubUrl,log) { }
        
        abstract protected Task<GpioState> readPinAsync(int pinId);

        public bool IsMonitored(int pinId) 
            => _monitorList.Contains(pinId);

        public IEnumerable<int> MonitorList 
            => _monitorList;

        public bool MonitorPin(int pinId)
        {
            if(!IsMonitored(pinId) && IsValidPin(pinId))
                _monitorList.Add(pinId);
            return IsMonitored(pinId);
        }

        public void IgnorePin(int pinId)
        {
            if(IsMonitored(pinId))
            {
                _monitorList.Remove(pinId);
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
                foreach (int pinId in _monitorList)
                {
                    GpioState st = await readPinAsync(pinId);
                    saveState(st);
                    Logger?.LogDebug($"[SEND] GpioState:{st}");
                    // Finally send the value:
                    await _conn.SendAsync("Broadcast", st.Timestamp, st.PinId, st.IsAnalog, st.Value);
                }
                await Task.Delay(UpdateMilliseconds, cancelToken);

            }
            await _conn.DisposeAsync();
        }

    }
}
