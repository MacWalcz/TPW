//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;
using TP.ConcurrentProgramming.Data.Diagnostics;


namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor

        private readonly IDiagnosticSerializer<BallDiagnosticData> _serializer;
        private readonly FileDiagnosticWriter _writer;
        private readonly List<Ball> BallsList = new();
        private bool Disposed = false;

        public DataImplementation()
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ball_diagnostics.log");
            _serializer = new AsciiDiagnosticSerializer();
            _writer = new FileDiagnosticWriter(logPath);
        }

        #endregion ctor

        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<Vector, IBall,object> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            Random random = new Random();
            for (int i = 0; i < numberOfBalls; i++)
            {
                object LockObject = new();
                Vector startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
                Ball newBall = new(startingPosition,startingPosition, 20,LockObject);

                newBall.NewPositionNotification += (sender, position) =>
                {
                    Ball b = (Ball)sender;
                    var data = new BallDiagnosticData
                    (
                        b.Id,
                        DateTime.UtcNow,
                        b.Position.x,
                        b.Position.y,
                        b.Velocity.x,
                        b.Velocity.y
                    );

                    var line = _serializer.Serialize(data);

                    if(!_writer.Enqueue(line))
                    {
                        Debug.WriteLine($"[Diagnostics] bufor pełny, porzucono wpis kulki #{b.Id}");
                    }
                };
                upperLayerHandler(startingPosition, newBall,LockObject);
                BallsList.Add(newBall);
                newBall.Velocity = new Vector((RandomGenerator.NextDouble() - 0.5) * 150, (RandomGenerator.NextDouble() - 0.5) * 150);
                newBall.StartMoving();


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

                    _writer.Dispose();
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


        private Random RandomGenerator = new();


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