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
using System.Reflection;
using System.Xml.Serialization;

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
            var data = new DataBallFixture();
            var lockObject = new object(); 
            var biz = new Ball(data, lockObject); 

            data.NewMove(10, 50);

            data.Velocity = new Data.Vector(-3, 5);

            data.NewMove(0, 100);

            Assert.AreEqual(3, data.Velocity.x, "X powinno się odwrócić");
            Assert.AreEqual(5, data.Velocity.y, "Y powinno pozostać bez zmian");
        }

        [TestMethod]
        public void WhenYAtOrBelowZero_ShouldCallDataContactY()
        {
            var data = new DataBallFixture();
            var lockObject = new object(); 
            var biz = new Ball(data, lockObject); 

            data.NewMove(50, 50);

            data.Velocity = new Data.Vector(4, -7);

            data.NewMove(100, 0);

            Assert.AreEqual(4, data.Velocity.x, "X powinno pozostać bez zmian");
            Assert.AreEqual(7, data.Velocity.y, "Y powinno się odwrócić");
        }

        [TestMethod]
        public void CollisionTest()
        {
            var ball1 = new DataBallFixture();
            var ball2 = new DataBallFixture();

            var lockObject1 = new object(); 
            var lockObject2 = new object();

            ball1.Position = new Data.Vector(49, 50);
            ball1.Velocity = new Data.Vector(1, 0);

            ball2.Position = new Data.Vector(51, 50);
            ball2.Velocity = new Data.Vector(-1, 0);

            var businessBall = new Ball(ball1, lockObject1); 
            var businessBall2 = new Ball(ball2, lockObject2);

            Ball.Balls.Clear();
            Ball.Balls.Add(businessBall);
            Ball.Balls.Add(businessBall2);

            ball1.NewMove(50, 50);
            ball2.NewMove(51, 50);
            ball1.NewMove(50, 50);

            Assert.AreNotEqual(1, ball1.Velocity.x, "Ball1 should change direction");
            Assert.AreNotEqual(-1, ball2.Velocity.x, "Ball2 should change direction");
        }


        #region Test instrumentation

        private class DataBallFixture : Data.IBall
        {
            public Data.Vector Velocity { get; set; } = new Data.Vector(0, 0);
            public double Mass { get; init; } = 1.0;

            public Data.Vector Position { get; set; }

            public event EventHandler<Data.Vector>? NewPositionNotification;

            public void NewMove(double x, double y)
                => NewPositionNotification?.Invoke(this, new Data.Vector(x, y));

           

        }
        

        #endregion testing instrumentation
    }
}