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
            var other = (Ball)otherBall; // Rzutowanie do konkretnej klasy aby uzyskać dostęp do jej pól

            int h1 = RuntimeHelpers.GetHashCode(this); // Aby uniknąć zakleszczenia ustawiamy kolejność blokad na podstawie hashcode'ów
            int h2 = RuntimeHelpers.GetHashCode(other);
            var first = h1 < h2 ? this : other;
            var second = first == this ? other : this;

            lock (first._lock) // Blokujemy oba obiekty przed modyfikacją wspólnych danych.
                lock (second._lock)
                {
                    var posA = this.Position; // Pobieramy pozycje obu kul
                    var posB = other.Position;
                    var velA = this.Velocity;
                    var velB = other.Velocity;

                    double mA = this.Mass; // Masy każdej kuli i współczynnik sprężystości (e = 1 = idealnie sprężyste)
                    double mB = other.Mass;
                    double e = 1.0;

                    double dx = posA.x - posB.x; // Obliczamy wektor od środka B do środka A i jego długość
                    double dy = posA.y - posB.y;
                    double dist = Math.Sqrt(dx * dx + dy * dy);

                    if (dist == 0) return;// Jeśli kule są dokładnie na sobie, pomijamy dalsze kroki

                    double nx = dx / dist; // Normalizujemy wektor do osi zderzenia
                    double ny = dy / dist;

                    double rvx = velA.x - velB.x; // Obliczamy komponent prędkości względnej wzdłuż normalnej:
                    double rvy = velA.y - velB.y;
                    double vAlong = rvx * nx + rvy * ny;
                    if (vAlong >= 0) return; // Jeśli komponent >= 0, kule się oddalają, więc brak reakcji

                    double j = -(1 + e) * vAlong / (1.0 / mA + 1.0 / mB); // Obliczamy skalarny impuls j wg wzoru:
                                                                          //    j = -(1 + e) * (v_rel · n) / (1/mA + 1/mB)
                    double ix = j * nx; // Składowa impulsu osi x
                    double iy = j * ny; // Składowa impulsu osi y

                    velA = new Vector(velA.x + ix / mA, velA.y + iy / mA); // Aktualizujemy prędkości obu kul:
                    velB = new Vector(velB.x - ix / mB, velB.y - iy / mB); // vA' = vA + (j/mA)*n,  vB' = vB - (j/mB)*n
                    this.Velocity = velA; 
                    other.Velocity = velB;

                    this.Move(); // Używamy move() aby zaktualizować pozycję obu kulek
                    other.Move();   
                }

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