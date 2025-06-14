﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using TP.ConcurrentProgramming.Data.Diagnostics;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;


namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI, IDisposable
    {
        #region ctor

        private readonly Serilog.ILogger _log;
        private readonly List<Ball> BallsList = new();
        private bool Disposed = false;
        private Random RandomGenerator = new();


        public DataImplementation(string logPath)
        {
            // Konfiguracja Serilog – tylko plik, JSONFormatter
            _log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    formatter: new JsonFormatter(renderMessage: true),
                    path: logPath,
                    rollingInterval: RollingInterval.Day
                )
                .CreateLogger();
        }

        #endregion ctor

        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<Vector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            Random random = new Random();
            for (int i = 0; i < numberOfBalls; i++)
            {
                int id = i + 1;
                Vector startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
                Ball newBall = new(id, startingPosition, startingPosition, 20);

                newBall.NewPositionNotification += (sender, pos) =>
                {
                    var b = (Ball)sender;
                    var data = new BallDiagnosticData(
                        b.Id,
                        DateTime.UtcNow,
                        pos.x, pos.y,
                        b.Velocity.x, b.Velocity.y
                    );

                    _log.Debug("{@BallDiagnosticData}", data);
                };
                upperLayerHandler(startingPosition, newBall);
                BallsList.Add(newBall);
                newBall.Velocity = new Vector((RandomGenerator.NextDouble() - 0.5) * 150, (RandomGenerator.NextDouble() - 0.5) * 150);



            }
            foreach (Ball ball in BallsList)
            {
                ball.StartMoving();
            }


        }
        #endregion DataAbstractAPI

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    foreach (var ball in BallsList)
                    {
                        ball.Stop();
                    }
                    BallsList.Clear();
                }
                Disposed = true;
            }
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        //private bool disposedValue;

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}