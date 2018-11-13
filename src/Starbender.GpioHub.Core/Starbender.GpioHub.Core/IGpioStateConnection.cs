using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GpioMonitor.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace GpioMonitor
{
    public interface IGpioStateConnection
    {
        bool AutoReconnect { get; set; }
        Uri HubUri { get; set; }
        string HubUrl { get; }
        bool IsConnected { get; }
        bool SaveHistory { get; set; }
        HubConnection Connection { get; }

        void Connect();
        void Connect(string url);
        void Connect(Uri uri);
        Task ConnectAsync();
        Task ConnectAsync(string url);
        Task ConnectAsync(Uri uri);
        void Disconnect();
        Task DisconnectAsync();
        IEnumerable<GpioState> GetHistory(int pinId);
        GpioState GetLast(int pinId);
        bool IsValidPin(int pinId);
        bool ValidateState(GpioState state);
    }
}