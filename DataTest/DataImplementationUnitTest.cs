using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class DataImplementationUnitTest
    {
        private readonly string _logPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "ball_diagnostics_test.json"
        );

        [TestInitialize]
        public void TestInitialize()
        {
            // Jeśli plik testowy istnieje, usuń go przed każdym testem
            if (File.Exists(_logPath))
            {
                File.Delete(_logPath);
            }
        }

        [TestMethod]
        public void ConstructorTestMethod()
        {
            // Używamy konstruktora z logPath
            using (var newInstance = new DataImplementation(_logPath))
            {
                IEnumerable<IBall>? ballsList = null;
                newInstance.CheckBallsList(x => ballsList = x);
                Assert.IsNotNull(ballsList);

                int numberOfBalls = -1;
                newInstance.CheckNumberOfBalls(x => numberOfBalls = x);
                Assert.AreEqual(0, numberOfBalls);
            }
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
            var newInstance = new DataImplementation(_logPath);
            bool newInstanceDisposed = false;
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsFalse(newInstanceDisposed, "Instancja powinna być nieudostępniona przed Dispose");

            newInstance.Dispose();
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsTrue(newInstanceDisposed, "Instancja powinna być oznaczona jako zwolniona po Dispose");

            IEnumerable<IBall>? ballsList = null;
            newInstance.CheckBallsList(x => ballsList = x);
            Assert.IsNotNull(ballsList);

            newInstance.CheckNumberOfBalls(x => Assert.AreEqual(0, x, "Po Dispose lista powinna pozostać pusta"));

            // Podwójne Dispose powinno rzucić wyjątek
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());

            // Po Dispose nie wolno już Start
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
        }

        [TestMethod]
        public void StartTestMethod()
        {
            using (var newInstance = new DataImplementation(_logPath))
            {
                int callbackCount = 0;
                int expectedCount = 10;

                newInstance.Start(
                  expectedCount,
                  (startingPosition, ball) =>
                  {
                      callbackCount++;
                      Assert.IsTrue(startingPosition.x >= 0);
                      Assert.IsTrue(startingPosition.y >= 0);
                      Assert.IsNotNull(ball);
                  });

                Assert.AreEqual(expectedCount, callbackCount, "Wywołań callbacka powinno być tyle, ile kulek");
                newInstance.CheckNumberOfBalls(x => Assert.AreEqual(expectedCount, x, "Liczba kulek w liście powinna być równa liczbie wywołań"));
            }
        }
    }
}
