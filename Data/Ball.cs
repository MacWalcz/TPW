//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        #region ctor

        public int Id { get; }
        internal Ball(int id, Vector initialPosition, Vector initialVelocity, double initialMass, object Lock)
        {
            Id = id;
            _position = initialPosition;
            Velocity = initialVelocity;
            Mass = initialMass;
            _Lock = Lock;
        }

        #endregion ctor

        #region IBall

        public event EventHandler<Vector>? NewPositionNotification;

        public Vector Velocity
        {
            get
            {

                return _velocity;

            }
            set
            {

                _velocity = value;

            }
        }

        internal (Vector position, Vector velocity) GetSnapshot()
        {
            lock (_Lock)
            {
                return (Position, Velocity);
            }
        }


        public Vector Position
        {
            get
            {

                return _position;

            }
            private set
            {

                _position = value;

            }
        }

        public double Mass { get; init; }


        #endregion IBal 

        #region private

        private readonly object _Lock;

        private Vector _velocity;

        private Vector _position;

        private volatile bool isMoving = true;

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }


        internal void Stop()
        {
            isMoving = false;
        }
        internal void StartMoving()
        {
            new Thread(() =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                double previousTime = stopwatch.Elapsed.TotalSeconds;
                while (isMoving)
                {
                    double currentTime = stopwatch.Elapsed.TotalSeconds;
                    double deltaTime = currentTime - previousTime;
                    previousTime = currentTime;
                    Move(deltaTime);

                }
            }).Start();
        }

        internal void Move(double deltaTime)
        {
            lock (_Lock)
            {
                Position = new Vector(Position.x + Velocity.x * deltaTime,
                                     Position.y + Velocity.y * deltaTime
                                     );
            }
            RaiseNewPositionChangeNotification();
        }

        #endregion private
    }
}