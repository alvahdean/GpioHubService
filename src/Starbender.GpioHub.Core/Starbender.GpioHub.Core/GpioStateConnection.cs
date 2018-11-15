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
    public class GpioStateConnection : IGpioStateConnection
    {
        public const int InvalidPin = -1;

        private object _syncRoot = new object();
        private Uri _hubUri = null;
        private Dictionary<int, List<GpioState>> _history = new Dictionary<int, List<GpioState>>();
        protected HubConnection _conn = null;
        
        public GpioStateConnection()
            : this(null,null) { }

        public GpioStateConnection(string hubUrl,ILogger log)
        {
            Logger = log;
            HubUrl = !String.IsNullOrWhiteSpace(hubUrl)
                ? hubUrl.Trim()
                : null;
        }

        protected virtual async Task onClose(Exception arg)
        {
            Logger?.LogInformation("Connection closed");
            if (AutoReconnect)
            {
                Logger?.LogInformation("Attempting to reconnect...");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _conn.StartAsync();
                Logger?.LogInformation("Reconnected, resuming processing");
            }
        }

        protected virtual void saveState(GpioState state)
        {
            if(!ValidateState(state))
            {
                Logger?.LogWarning("Invalid GpioState, skipping save");
                return;
            }
            if (!_history.ContainsKey(state.PinId))
                _history.Add(state.PinId, new List<GpioState>());
            if (!SaveHistory)
                _history[state.PinId].Clear();
            _history[state.PinId].Add(state);
        }

        protected virtual void removeHistory(int pinId)
        {
            if (!_history.ContainsKey(pinId))
                _history.Remove(pinId);
        }

        internal ILogger Logger { get; set; }

        public bool ValidateState(GpioState state) 
            => IsValidPin(state?.PinId??InvalidPin);

        public string HubUrl
        {
            get => HubUri?.AbsoluteUri;
            internal set
            {
                if (HubUrl != value)
                {
                    if (IsConnected)
                    {
                        Logger?.LogWarning("Hub Uri change will no take affect until disconnected");
                    }
                    if (value != null)
                    {
                        HubUri = new Uri(value);
                    }
                }
            }
        }

        public Uri HubUri
        {
            get => _hubUri;
            set => _hubUri = value;
        }

        public bool AutoReconnect
        { get; set; } = true;

        public bool SaveHistory
        { get; set; }

        public bool IsConnected
        { get; protected set; } = false;

        public HubConnection Connection => _conn;

        public virtual bool IsValidPin(int pinId) 
            => pinId > 0;

        public IEnumerable<GpioState> GetHistory(int pinId)
        {
            return _history.ContainsKey(pinId) 
                ? _history[pinId] 
                : (IEnumerable<GpioState>)new GpioState[] { };
        }

        public GpioState GetLast(int pinId)
        {
            return _history.ContainsKey(pinId)
                ? _history[pinId].OrderByDescending(t=>t.Timestamp).FirstOrDefault()
                : null;
        }

        public void Connect() => Connect((Uri)null);

        public void Connect(string url) => Connect(String.IsNullOrWhiteSpace(url)? null: new Uri(url));

        public void Connect(Uri uri)
        {
            uri = uri ?? HubUri;
            Task.Run(async () => { await ConnectAsync(uri); }).Wait();
        }

        public async Task ConnectAsync()
            => await ConnectAsync((Uri)null);

        public async Task ConnectAsync(string url)
            => await ConnectAsync(String.IsNullOrWhiteSpace(url) ? null : new Uri(url));

        public virtual async Task ConnectAsync(Uri uri)
        {
            if (uri != null)
                HubUri = uri;

            if(HubUri==null)
            {
                Logger.LogError("HubUri not set, cannot connect");
                return;
            }

            if (IsConnected)
            {
                if (uri == HubUri)
                {
                    Logger?.LogInformation("Already connected");
                    return;
                }
                await DisconnectAsync();
            }

            try
            {
                _conn = new HubConnectionBuilder()
                    .WithUrl(HubUrl)
                    .Build();
                _conn.Closed += onClose;

                await _conn.StartAsync();
                IsConnected = true;
                Logger?.LogInformation($"Connected to {HubUrl}");
            }
            catch (Exception ex)
            {
                Logger?.LogInformation($"[{ex.GetType().Name}]: {ex.Message}");
            }
        }

        public async Task DisconnectAsync()
        {
            if (!IsConnected)
            {
                Logger?.LogInformation("Already disconnected");
                return;
            }
            bool ar = AutoReconnect;
            AutoReconnect = false;
            await _conn.StopAsync();
            AutoReconnect = ar;
            IsConnected = false;
        }

        public void Disconnect()
        {
            Task.Run(async () => { await DisconnectAsync(); }).Wait();
        }
    }
}
