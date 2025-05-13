//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Runtime.CompilerServices;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        #region ctor

        internal Ball(Vector initialPosition, Vector initialVelocity, double initialMass)
        {
            _position = initialPosition;
            Velocity = initialVelocity;
            Mass = initialMass;
            velocityLength = Math.Sqrt(Velocity.x * Velocity.x + Velocity.y * Velocity.y);
        }

        #endregion ctor

        #region IBall

        public event EventHandler<Vector>? NewPositionNotification;

        public Vector Velocity
        {
            get
            {
                lock (_velocityLock)
                {
                    return _velocity;
                }
            }
            set
            {
                lock (_velocityLock)
                {
                    _velocity = value;
                }
            }
        }

        public Vector Position
        {
            get
            {
                lock (_positionLock)
                {
                    return _position;
                }
            }
            private set
            {
                lock (_positionLock)
                {
                    _position = value;
                }
            }
        }

        public double Mass { get; init; }


        #endregion IBal 

        #region private

        private readonly object _velocityLock = new();

        private readonly object _positionLock = new();

        private Vector _velocity;

        private Vector _position;

        private double velocityLength;

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
                while (isMoving)
                {
                    Move();
                    int delay = 500 / (int)velocityLength;
                    Thread.Sleep(delay);
                }
            }).Start();
        }

        internal void Move()
        {

            Position = new Vector(Position.x + Velocity.x / velocityLength, Position.y + Velocity.y / velocityLength);
            RaiseNewPositionChangeNotification();
        }

        #endregion private
    }
}