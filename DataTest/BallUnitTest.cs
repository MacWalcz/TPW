//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________


namespace TP.ConcurrentProgramming.Data.Test
{
  [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Vector testinVector = new Vector(0.0, 0.0);
            Ball newInstance = new(1, testinVector, testinVector, 1.0, new object()); // Fixed by providing all required arguments
        }

        [TestMethod]
        public void MoveTestMethod()
        {
            Vector initialPosition = new(10.0, 10.0);
            Ball newInstance = new(1, initialPosition, new Vector(0.0, 1.0), 1.0, new object()); // Fixed by providing all required arguments
            Vector curentPosition = new Vector(10.0, 11.0);
            int numberOfCallBackCalled = 0;
            Vector Expected = new(10.0, 11.0);
            newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); curentPosition = position; numberOfCallBackCalled++; };
            newInstance.Move(1.0); // Assuming Move requires a deltaTime argument
            Assert.AreEqual<int>(1, numberOfCallBackCalled);
            Assert.AreEqual<Vector>(Expected, curentPosition);
        }
    }
}