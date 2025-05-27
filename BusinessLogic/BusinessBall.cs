//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using BisAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {

        internal static ConcurrentBag<Ball> Balls = new(); // Kolekcja do przechowywania wszystkich kul
        private readonly Data.IBall _inner; // Referencja do obiektu kulki
        private static readonly ConcurrentDictionary<(int, int), bool> InCollision = new ConcurrentDictionary<(int, int), bool>(); // Dictionary do przechowywania kul w kolizji
        private readonly int _hash;
        private Position _currentPosition;

        internal Ball(Data.IBall ball, object Lock)
        {
            _inner = ball; // Inicjalizacja referencji do obiektu kulki
            _hash = RuntimeHelpers.GetHashCode(this); // Obliczanie hasha kulki do identyfikacji w kolizji
            Balls.Add(this); // Dodanie kulki do kolekcji
            _lock = Lock;
            ball.NewPositionNotification += RaisePositionChangeEvent;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private
        private object _lock;
        private void RaisePositionChangeEvent(object? sender, Data.Vector e)
        {

            _currentPosition = new Position(e.x, e.y);

            NewPositionNotification?.Invoke(this, _currentPosition);
            if (sender is Data.IBall ball)
            {

                double rightBoundary = BisAPI.GetDimensions.TableWidth - BisAPI.GetDimensions.BallDimension - 6;
                double bottomBoundary = BisAPI.GetDimensions.TableHeight - BisAPI.GetDimensions.BallDimension - 6;

                double x = ball.Velocity.x;
                double y = ball.Velocity.y;
                if (e.x >= rightBoundary)
                {
                    lock (_lock)
                    {
                        ball.Velocity = new Data.Vector(-Math.Abs(x), y);
                    }

                }
                else if (e.x <=0)
                {
                    lock (_lock)
                    {
                        ball.Velocity = new Data.Vector(Math.Abs(x), y);
                    }
                }
                else if (e.y >= bottomBoundary)
                {
                    lock (_lock)
                    {
                        ball.Velocity = new Data.Vector(x, -Math.Abs(y));
                    }
                }
                else if (e.y <= 0)
                {
                    lock (_lock)
                    {   
                        ball.Velocity = new Data.Vector(x, Math.Abs(y));
                    }
                }

                double diameter = BisAPI.GetDimensions.BallDimension; // średnica kulki
                double collidsionDistance = diameter * diameter; // odległość kolizji

                Parallel.ForEach(Balls, other => // iteracja po wszystkich kulach
                    {
                        if (ReferenceEquals(this, other)) return; // jeżeli to ta sama kulka to pomijamy

                        if (other._currentPosition is null) return; // Jeżeli pozycja kulki jest null to pomijamy


                        int h1 = RuntimeHelpers.GetHashCode(this); // Aby uniknąć zakleszczenia ustawiamy kolejność blokad na podstawie hashcode'ów
                        int h2 = RuntimeHelpers.GetHashCode(other);
                        Ball first = h1 < h2 ? this : other;
                        Ball second = first == this ? other : this;
                        lock (first._lock)
                        {
                            lock (second._lock)
                            {
                                var key = _hash < other._hash ? (_hash, other._hash) : (other._hash, _hash); // tworzenie klucza do Dictionary
                                double dx = e.x - other._currentPosition.x; // obliczanie odległości
                                double dy = e.y - other._currentPosition.y;
                                double distSq = dx * dx + dy * dy; // kwadrat odległości

                                if (distSq < collidsionDistance) // jeżeli odległość jest mniejsza od średnicy kulki to sprawdzamy kolizję
                                {
                                    if (InCollision.TryAdd(key, true)) // dodajemy do Dictionary
                                    {

                                        ContactBall(this, other);
                                    }
                                }
                                else
                                {
                                    InCollision.TryRemove(key, out _); // jeżeli kulki się nie stykają to usuwamy z Dictionary
                                }
                            }
                        }
                    });
            }


        }
        private void ContactBall(Ball firstBall, Ball otherBall)
        {




            var posA = firstBall._inner.Position; // Pobieramy pozycje obu kul
            var posB = otherBall._inner.Position;
            var velA = firstBall._inner.Velocity;
            var velB = otherBall._inner.Velocity;

            double mA = firstBall._inner.Mass; // Masy każdej kuli i współczynnik sprężystości (e = 1 = idealnie sprężyste)
            double mB = otherBall._inner.Mass;
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

            velA = new Data.Vector(velA.x + ix / mA, velA.y + iy / mA); // Aktualizujemy prędkości obu kul:
            velB = new Data.Vector(velB.x - ix / mB, velB.y - iy / mB); // vA' = vA + (j/mA)*n,  vB' = vB - (j/mB)*n
            firstBall._inner.Velocity = velA;
            otherBall._inner.Velocity = velB;


        }



        #endregion private
    }
}