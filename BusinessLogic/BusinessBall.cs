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
using System.Numerics;
using System.Runtime.CompilerServices;
using BisAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {

        private static ConcurrentBag<Ball> Balls = new(); // Kolekcja do przechowywania wszystkich kul
        private readonly Data.IBall _inner; // Referencja do obiektu kulki
        private static readonly ConcurrentDictionary<(int, int), bool> InCollision = new ConcurrentDictionary<(int, int), bool>(); // Dictionary do przechowywania kul w kolizji
        private readonly int _hash;
        private Position _currentPosition;
        public Position CurrentPosition => _currentPosition; // Właściwość do pobierania aktualnej pozycji kulki
        public Ball(Data.IBall ball)
        {
            _inner = ball; // Inicjalizacja referencji do obiektu kulki
            _hash = RuntimeHelpers.GetHashCode(this); // Obliczanie hasha kulki do identyfikacji w kolizji
            Balls.Add(this); // Dodanie kulki do kolekcji
            ball.NewPositionNotification += RaisePositionChangeEvent;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion IBall

        #region private

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
            _currentPosition = new Position(e.x, e.y);

            NewPositionNotification?.Invoke(this, _currentPosition);
            if (sender is Data.IBall ball)
            {

                double rightBoundary = BisAPI.GetDimensions.TableWidth - BisAPI.GetDimensions.BallDimension - 6;
                double bottomBoundary = BisAPI.GetDimensions.TableHeight - BisAPI.GetDimensions.BallDimension - 6;
                

                if (e.x >= rightBoundary || e.x <= 0)
                {
                    ball.ContactX();

                }
                else if (e.y >= bottomBoundary || e.y <= 0)
                {
                    ball.ContactY();
                }

                double diameter = BisAPI.GetDimensions.BallDimension; // średnica kulki
                double collidsionDistance = diameter * diameter; // odległość kolizji

                Parallel.ForEach(Balls, other => // iteracja po wszystkich kulach
                    {
                        if (ReferenceEquals(this, other)) return; // jeżeli to ta sama kulka to pomijamy

                        if (other.CurrentPosition is null) return; // Jeżeli pozycja kulki jest null to pomijamy

                        var key = _hash < other._hash ? (_hash, other._hash) : (other._hash, _hash); // tworzenie klucza do Dictionary
                        double dx = e.x - other.CurrentPosition.x; // obliczanie odległości
                        double dy = e.y - other.CurrentPosition.y; 
                        double distSq = dx * dx + dy * dy; // kwadrat odległości

                        if (distSq < collidsionDistance) // jeżeli odległość jest mniejsza od średnicy kulki to sprawdzamy kolizję
                        {
                            if (InCollision.TryAdd(key, true)) // dodajemy do Dictionary
                            {
                                _inner.ContactBall(other._inner); //
                            }
                        }
                        else
                        {
                            InCollision.TryRemove(key, out _); // jeżeli kulki się nie stykają to usuwamy z Dictionary
                        }
                    });
            }

        }

        #endregion private
    }
}