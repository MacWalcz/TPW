//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Threading;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicImplementationUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            using (BusinessLogicImplementation newInstance = new(new DataLayerConstructorFixcure()))
            {
                bool newInstanceDisposed = true;
                newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
                Assert.IsFalse(newInstanceDisposed);
            }
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
            DataLayerDisposeFixcure dataLayerFixcure = new DataLayerDisposeFixcure();
            BusinessLogicImplementation newInstance = new(dataLayerFixcure);
            Assert.IsFalse(dataLayerFixcure.Disposed);
            bool newInstanceDisposed = true;
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsFalse(newInstanceDisposed);
            newInstance.Dispose();
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsTrue(newInstanceDisposed);
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
            Assert.IsTrue(dataLayerFixcure.Disposed);
        }

        [TestMethod]
        public void StartTestMethod()
        {
            DataLayerStartFixcure dataLayerFixcure = new();
            using (BusinessLogicImplementation newInstance = new(dataLayerFixcure))
            {
                int called = 0;
                int numberOfBalls2Create = 10;
                newInstance.Start(
                  numberOfBalls2Create,
                  (startingPosition, ball) => { called++; Assert.IsNotNull(startingPosition); Assert.IsNotNull(ball); });
                Assert.AreEqual<int>(1, called);
                Assert.IsTrue(dataLayerFixcure.StartCalled);
                Assert.AreEqual<int>(numberOfBalls2Create, dataLayerFixcure.NumberOfBallseCreated);
            }
        }

        [TestMethod]
        public void TestWallCollision_ReversesXDirection()
        {
      
            var logic = new BusinessLogicImplementation();
            var ball = new DataBallFixture(new Vector(0,100),new Vector(-1,1),20);

    
            logic.CheckCollisions(ball);

            Assert.IsTrue(ball.Velocity.x > 0, "Ball should reverse X direction after wall collision");
        }


        [TestMethod]
        public void TestWallCollision_ReversesYDirection()
        {
           
            var logic = new BusinessLogicImplementation();
            var ball = new DataBallFixture(new Vector(100, 400), new Vector(1, 1), 20);

            logic.CheckCollisions(ball);

          
            Assert.IsTrue(ball.Velocity.y < 0, "Ball should reverse X direction after wall collision");
        }

        [TestMethod]
        public void TestBallCollision_ChangesVelocity()
        {
            var logic = new BusinessLogicImplementation();
            
            var ball1 = new DataBallFixture(new Vector(100, 100), new Vector(10, 0), 20);
            var ball2 = new DataBallFixture(new Vector(105, 100), new Vector(-10, 0), 20);

            logic.balls.Add(ball1);
            logic.balls.Add(ball2); 
            

            var oldVelocity1 = ball1.Velocity;
            var oldVelocity2 = ball2.Velocity;

            logic.CheckCollisions(ball1);


            Assert.AreNotEqual(oldVelocity1.x, ball1.Velocity.x, "Velocity X of ballA should change after collision");
            Assert.AreNotEqual(oldVelocity2.x, ball2.Velocity.x, "Velocity X of ballB should change after collision");

        }
        #region testing instrumentation

        private class DataLayerConstructorFixcure : Data.DataAbstractAPI
        {
            public override void Dispose()
            { }

            public override void Start(int numberOfBalls, Action<Vector, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }
        }

        private class DataLayerDisposeFixcure : Data.DataAbstractAPI
        {
            internal bool Disposed = false;

            public override void Dispose()
            {
                Disposed = true;
            }

            public override void Start(int numberOfBalls, Action<Vector, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }
        }

        private class DataLayerStartFixcure : Data.DataAbstractAPI
        {
            internal bool StartCalled = false;
            internal int NumberOfBallseCreated = -1;

            public override void Dispose()
            { }

            public override void Start(int numberOfBalls, Action<Vector, Data.IBall> upperLayerHandler)
            {
                StartCalled = true;
                NumberOfBallseCreated = numberOfBalls;
                upperLayerHandler(new Vector(0, 0), new DataBallFixture());
            }

           
        }
        private class DataBallFixture : Data.IBall
        {
            public int Id { get => throw new NotImplementedException(); }
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


            public event EventHandler<Vector>? NewPositionNotification = null;

            private Vector _velocity;

            private Vector _position;

            private double _mass;

            public DataBallFixture(Vector initialPosition, Vector initialVelocity, double initialMass) { 
                _velocity = initialVelocity;
                _position = initialPosition;
                Mass = initialMass;
            }

            public DataBallFixture() {

                _velocity = new Vector(0, 0);
                _position = new Vector(0, 0);   
                Mass = 0;   
            }

        }
        #endregion testing instrumentation
    }
}