using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace automation.Models
{
    public class TelemetryData
    {
        public UInt64? Id { get; set; }

        public Sensor Sensor { get; set; }

        public double? Temperature { get; set; }

        public double? Humidity { get; set; }

        public DateTime? Timestamp { get; set; }

        public byte[] CustomData { get; set; }

        public override string ToString()
        {
            return $"{nameof(Temperature)}: {Temperature}, {nameof(Humidity)}: {Humidity}, {nameof(Timestamp)}: {Timestamp}, {nameof(CustomData)}: {CustomData}";
        }
    }

    public class DecoratedTelemetryData
    {
        private readonly TelemetryData _telemetry;
        private readonly long _epochTs;

        public DecoratedTelemetryData(TelemetryData telemetry, long epochTs)
        {
            _telemetry = telemetry;
            _epochTs = epochTs;
        }

        public long Epoch => _epochTs;

        public TelemetryData Telemetry => _telemetry;
    }
}
