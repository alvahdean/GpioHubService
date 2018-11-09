using System;
using Newtonsoft.Json;

namespace GpioMonitor.Models
{
    public class GpioState
    {
        public const UInt16 MaxValue=UInt16.MaxValue;
        public const UInt16 DigitalThreshold= UInt16.MaxValue / 2;
        public const UInt16 HighValue = UInt16.MaxValue;
        public const UInt16 LowValue = 0;


        public GpioState()
            : this(-1) { }

        public GpioState(int pinId)
            : this(pinId, false) { }

        public GpioState(int pinId, bool value, DateTime? timestamp = null)
            : this(pinId, value ? HighValue : LowValue, timestamp, false) { }
    
        public GpioState(GpioState other)
            : this(other.PinId, other.Value, other.Timestamp, other.IsAnalog) { }

        public GpioState(int pinId, int value, DateTime? timestamp = null, bool isAnalog = false)
        {
            if (timestamp == null)
                timestamp = DateTime.Now;
            Timestamp = timestamp.Value;
            PinId = pinId;
            Value = value;
            IsAnalog = isAnalog;
        }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("pinId")]
        public int PinId { get; set; }

        [JsonProperty("isAnalog")]
        public bool IsAnalog { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }

        public string ValueText 
            => IsAnalog ? $"{Value:f4}" : Value >= DigitalThreshold ? "High" : "Low";

        public override string ToString() 
            => $"{Timestamp} GPIO[{PinId}]: {ValueText}";
    }
}
