using System.Globalization;

namespace TP.ConcurrentProgramming.Data.Diagnostics
{
    internal class AsciiDiagnosticSerializer : IDiagnosticSerializer<BallDiagnosticData>
    {
        public string Serialize(BallDiagnosticData d)
        {
            return string.Join(';',
                d.Timestamp.ToString("o", CultureInfo.InvariantCulture),
                d.BallId,
                d.X.ToString("F3", CultureInfo.InvariantCulture),
                d.Y.ToString("F3", CultureInfo.InvariantCulture),
                d.Vx.ToString("F3", CultureInfo.InvariantCulture),
                d.Vy.ToString("F3", CultureInfo.InvariantCulture)
            );
        }
    }
}
