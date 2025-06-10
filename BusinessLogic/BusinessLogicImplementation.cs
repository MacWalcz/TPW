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
using System.Runtime.CompilerServices;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        #region ctor

        public BusinessLogicImplementation() : this(null)
        { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
        }

        #endregion ctor

        #region BusinessLogicAbstractAPI

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            layerBellow.Start(numberOfBalls, (startingPosition, databall) =>
            {
                Ball newBall = new Ball(databall);

                balls.Add(databall);

                IPosition position = new Position(startingPosition.x, startingPosition.y);
                upperLayerHandler(position, newBall);
            });
        }

        public override void CheckCollisions(Data.IBall ball)
        {
            
                Data.Vector checkedPosition = ball.Position;
                Data.Vector checkedVelocity = ball.Velocity;
                if (checkedPosition.x >= rightBoundary)
                {
                    ball.Velocity = new Data.Vector(-Math.Abs(checkedVelocity.x), checkedVelocity.y);
                }
                else if (checkedPosition.x <= 0)
                {
                    ball.Velocity = new Data.Vector(Math.Abs(checkedVelocity.x), checkedVelocity.y);
                }
                else if (checkedPosition.y >= bottomBoundary)
                {
                    ball.Velocity = new Data.Vector(checkedVelocity.x, -Math.Abs(checkedVelocity.y));
                }
                else if (checkedPosition.y <= 0)
                {
                    ball.Velocity = new Data.Vector(checkedVelocity.x, Math.Abs(checkedVelocity.y));
                }

                

                foreach (Data.IBall other in balls)
                {
                    if (other != ball)
                    {
                        Data.Vector otherPosition = other.Position;
                        Data.Vector otherVelocity = other.Velocity;
                        double dx = checkedPosition.x - otherPosition.x; // obliczanie odległości
                        double dy = checkedPosition.y - otherPosition.y;
                        double distSq = dx * dx + dy * dy; // kwadrat odległości

                        if (distSq < collidsionDistance) // jeżeli odległość jest mniejsza od średnicy kulki to sprawdzamy kolizję
                        {
                            double mA = ball.Mass; // Masy każdej kuli i współczynnik sprężystości (e = 1 = idealnie sprężyste)
                            double mB = other.Mass;
                            double e = 1.0;

                            double dist = Math.Sqrt(dx * dx + dy * dy);

                            if (dist == 0) return;// Jeśli kule są dokładnie na sobie, pomijamy dalsze kroki

                            double nx = dx / dist; // Normalizujemy wektor do osi zderzenia
                            double ny = dy / dist;

                            double rvx = checkedVelocity.x - otherVelocity.x; // Obliczamy komponent prędkości względnej wzdłuż normalnej:
                            double rvy = checkedVelocity.y - otherVelocity.y;
                            double vAlong = rvx * nx + rvy * ny;
                            if (vAlong >= 0) return; // Jeśli komponent >= 0, kule się oddalają, więc brak reakcji

                            double j = -(1 + e) * vAlong / (1.0 / mA + 1.0 / mB); // Obliczamy skalarny impuls j wg wzoru:
                                                                                  //    j = -(1 + e) * (v_rel · n) / (1/mA + 1/mB)
                            double ix = j * nx; // Składowa impulsu osi x
                            double iy = j * ny; // Składowa impulsu osi y


                            ball.Velocity = new Data.Vector(checkedVelocity.x + ix / mA, checkedVelocity.y + iy / mA); // Aktualizujemy prędkości obu kul:
                            other.Velocity = new Data.Vector(otherVelocity.x - ix / mB, otherVelocity.y - iy / mB); // vA' = vA + (j/mA)*n,  vB' = vB - (j/mB)*n

                        }



                    }
                }
            

        }

        #endregion BusinessLogicAbstractAPI

        #region private

        private bool Disposed = false;

        private readonly UnderneathLayerAPI layerBellow;

        internal List<Data.IBall> balls = new List<Data.IBall>();

        private double rightBoundary = GetDimensions.TableWidth - GetDimensions.BallDimension - 6;

        private double bottomBoundary = GetDimensions.TableHeight - GetDimensions.BallDimension - 6;

        private double collidsionDistance = GetDimensions.BallDimension * GetDimensions.BallDimension;


        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}