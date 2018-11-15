using System;
using Newtonsoft.Json;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace GpioMonitor.Models
{
    public class GpioState
    {
        public GpioState()
            : this(WiringPiPin.Unknown) { }

        public GpioState(GpioPin pin)
            : this(pin, false) { }

        public GpioState(WiringPiPin pin)
            : this(pin, false) { }

        public GpioState(GpioState other)
            : this(other.Pin, other.Value, other.Timestamp) { }

        public GpioState(GpioPin pin, bool value, DateTime? timestamp = null)
            : this(pin.WiringPiPinNumber, value ? GpioPinValue.High : GpioPinValue.Low,timestamp) { }

        public GpioState(GpioPin pin, GpioPinValue value, DateTime? timestamp = null)
            : this(pin.WiringPiPinNumber, value,timestamp) { }

        public GpioState(WiringPiPin pin, bool value, DateTime? timestamp = null)
            : this(pin, value ? GpioPinValue.High : GpioPinValue.Low) { }

        public GpioState(WiringPiPin pin, GpioPinValue value, DateTime? timestamp = null)
        {
            if (timestamp == null)
                timestamp = DateTime.Now;
            Timestamp = timestamp.Value;
            Pin = pin;
            Value = value;
        }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("pin")]
        public WiringPiPin Pin { get; set; }

        [JsonProperty("value")]
        public GpioPinValue Value { get; set; }

        public string ValueText 
            => Value.ToString();

        public GpioPin GpioPin { get => Pin != WiringPiPin.Unknown ? Pi.Gpio[Pin] : null; }

        public override string ToString() 
            => $"{Timestamp} GPIO[{Pin}]: {ValueText}";
    }
}
