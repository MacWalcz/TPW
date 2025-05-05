//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        #region ctor

        internal Ball(Vector initialPosition, Vector initialVelocity, double initialMass)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
            Mass = initialMass;
            velocityLength = Math.Sqrt(Velocity.x * Velocity.x + Velocity.y * Velocity.y);
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }

        public double Mass { get; init; }

        public void ContactX()
        {
            double x = Velocity.x; double y = Velocity.y;
            lock (_lock)
            {
                Velocity = new Vector(-x, y);
                Move();
            }
        }

        public void ContactY()
        {
            double x = Velocity.x; double y = Velocity.y;
            lock (_lock)
            {
                Velocity = new Vector(x, -y);
                Move();
            }
        }

        public void ContactBall(IBall otherBall)
        {

        }


        #endregion IBal 

        #region private

        private readonly object _lock = new();


        private Vector Position;

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
            lock (_lock)
            {
                Position = new Vector(Position.x + Velocity.x / velocityLength, Position.y + Velocity.y / velocityLength);
            }
            RaiseNewPositionChangeNotification();
        }

        #endregion private
    }
}