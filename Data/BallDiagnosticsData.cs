using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data.Diagnostics
{
    internal record BallDiagnosticData(
        int BallId,
        DateTime Timestamp,
        double X,
        double Y,
        double Vx,
        double Vy
    );
}
