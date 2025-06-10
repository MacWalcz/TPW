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
              

        #region Test instrumentation

        private class DataBallFixture : Data.IBall
        {
            public int Id { get; }
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