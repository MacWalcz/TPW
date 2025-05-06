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
using System.Reflection;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BallUnitTest
    {

        [TestInitialize]
        public void Init()
        {
            var ballsField = typeof(Ball).GetField("Balls", BindingFlags.NonPublic | BindingFlags.Static);
            var bag = (ConcurrentBag<Ball>)ballsField.GetValue(null)!;
            while (bag.TryTake(out _));
        }
        [TestMethod]
        public void WhenXAtOrBelowZero_ShouldCallDataContactX()
        {
            // Arrange
            var data = new DataBallFixture();
            var biz = new Ball(data);

            data.NewMove(10, 50);

            data.Velocity = new VectorFixture(-3, 5);

            // Act
            data.NewMove(0, 100);  

            // Assert
            Assert.IsTrue(data.ContactXCalled, "ContactX() powinno zostać wywołane w warstwie danych");
            Assert.AreEqual(3, data.Velocity.x, "X powinno się odwrócić");
            Assert.AreEqual(5, data.Velocity.y, "Y powinno pozostać bez zmian");
        }

        [TestMethod]
        public void WhenYAtOrBelowZero_ShouldCallDataContactY()
        {
            var data = new DataBallFixture();
            var biz = new Ball(data);

            data.NewMove(50, 50);

            data.Velocity = new VectorFixture(4, -7);

            data.NewMove(100, 0);  

            Assert.IsTrue(data.ContactYCalled, "ContactY() powinno zostać wywołane w warstwie danych");
            Assert.AreEqual(4, data.Velocity.x, "X powinno pozostać bez zmian");
            Assert.AreEqual(7, data.Velocity.y, "Y powinno się odwrócić");
        }

        [TestMethod]
        public void WhenTwoBallsOverlap_ShouldCallDataContactBallOnceOnA()
        {
            var dataA = new DataBallFixture();
            var dataB = new DataBallFixture();
            var bA = new Ball(dataA);
            var bB = new Ball(dataB);

            dataA.NewMove(0, 0);
            dataB.NewMove(15, 0);

            ClearCollisionDictionary();

            dataA.ResetContactCount();
            dataB.ResetContactCount();

            dataA.Velocity = new VectorFixture(5, 0);
            dataB.Velocity = new VectorFixture(0, 0);
            dataA.NewMove(1, 0);

            Assert.AreEqual(1, dataA.ContactBallCallCount);
            Assert.AreEqual(0, dataB.ContactBallCallCount);
        }

        private void ClearCollisionDictionary()
        {
            var dictField = typeof(Ball).GetField("InCollision", BindingFlags.NonPublic | BindingFlags.Static);
            var dict = (ConcurrentDictionary<(int, int), bool>)dictField.GetValue(null)!;
            dict.Clear();
        }


        #region Test instrumentation

        private class DataBallFixture : Data.IBall
        {
            public Data.IVector Velocity { get; set; } = new VectorFixture(0, 0);
            public double Mass { get; init; } = 1.0;

            public event EventHandler<Data.IVector>? NewPositionNotification;

            public bool ContactXCalled { get; private set; }
            public bool ContactYCalled { get; private set; }
            public int ContactBallCallCount { get; private set; }

            public void NewMove(double x, double y)
                => NewPositionNotification?.Invoke(this, new VectorFixture(x, y));

            void Data.IBall.ContactX()
            {
                ContactXCalled = true;
                Velocity = new VectorFixture(-Velocity.x, Velocity.y);
            }

            void Data.IBall.ContactY()
            {
                ContactYCalled = true;
                Velocity = new VectorFixture(Velocity.x, -Velocity.y);
            }

            void Data.IBall.ContactBall(Data.IBall other)
            {
                ContactBallCallCount++;
                Velocity = new VectorFixture(-Velocity.x, -Velocity.y);
            }
            public void ResetContactCount() => ContactBallCallCount = 0;

        }
        private class VectorFixture : Data.IVector
        {
            internal VectorFixture(double X, double Y)
            {
                x = X; y = Y;
            }

            public double x { get; init; }
            public double y { get; init; }
        }

        #endregion testing instrumentation
    }
}