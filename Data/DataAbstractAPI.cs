//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using Microsoft.Extensions.Logging;
using Serilog.Formatting.Json;
using Serilog;
using TP.ConcurrentProgramming.Data.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        #region Layer Factory

        public static DataAbstractAPI GetDataLayer()
        {
            return modelInstance.Value;
        }

        #endregion Layer Factory

        #region public API

        public abstract void Start(int numberOfBalls, Action<Vector, IBall> upperLayerHandler);

        #endregion public API

        #region IDisposable

        public abstract void Dispose();

        #endregion IDisposable

        #region private

        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() =>
{
    var logPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "ball_diagnostics.json"
    );
    return new DataImplementation(logPath);
});

        #endregion private
    }

    public record Vector(double x, double y);

    public interface IBall
    {
        event EventHandler<Vector> NewPositionNotification;

        Vector Velocity { get; set; }
        Vector Position { get;}
        public int Id { get; }
        double Mass { get; }

    }
}