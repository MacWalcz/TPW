using System.Globalization;

namespace TP.ConcurrentProgramming.Data.Diagnostics
{
    internal class AsciiDiagnosticSerializer : IDiagnosticSerializer<BallDiagnosticData>
    {
        public string Serialize(BallDiagnosticData data)
        {
            string timestamp = data.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
            return $"{timestamp} | [Data] BallId: {data.BallId}  position: ({data.X.ToString("F3", CultureInfo.InvariantCulture)}, {data.Y.ToString("F3", CultureInfo.InvariantCulture)}), " +
                   $"velocity: ({data.Vx.ToString("F3", CultureInfo.InvariantCulture)}, {data.Vy.ToString("F3", CultureInfo.InvariantCulture)})";
        }
    }
}
