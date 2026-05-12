using Data;

namespace DataTest
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestCreateBallsPositionAndCount()
        {
            int width = 640;
            int height = 400;
            var api = DataAbstractAPI.CreateAPI(width, height);
            int ballCount = 10;
            int radius = 20;
            double weight = 1.0;

            api.CreateBalls(ballCount, radius, weight);
            var balls = api.GetBalls();

            Assert.HasCount(ballCount, balls);

            foreach (var ball in balls)
            {
                Assert.IsTrue(ball.X >= 0 && ball.X <= width - radius, "Kula X poza zakresem.");
                Assert.IsTrue(ball.Y >= 0 && ball.Y <= height - radius, "Kula Y poza zakresem.");

                // Sprawdzenie czy cechy kul (masa, promień) są poprawnie przypisane
                Assert.AreEqual(radius, ball.Radius, "Promień kuli jest nieprawidłowy.");
                Assert.AreEqual(weight, ball.Weight, "Masa kuli jest nieprawidłowa.");
            }
        }

        [TestMethod]
        public async Task TestBallsMovement()
        {
            var api = DataAbstractAPI.CreateAPI(640, 400);
            api.CreateBalls(1, 20, 1.0);
            var ball = api.GetBalls().First();

            double initialX = ball.X;
            double initialY = ball.Y;

            await Task.Delay(100);

            // Sprawdzamy czy kula się poruszyła (czy VX/VY działają)
            Assert.AreNotEqual(initialX, ball.X, "Pozycja X kuli nie zmieniła się po czasie.");
            Assert.AreNotEqual(initialY, ball.Y, "Pozycja Y kuli nie zmieniła się po czasie.");
        }

        [TestMethod]
        public void TestStopSimulationClearsBalls()
        {
            var api = DataAbstractAPI.CreateAPI(640, 400);
            api.CreateBalls(5, 20, 1.0);

            api.StopSimulation();
            api.CreateBalls(2, 20, 1.0);
            Assert.AreEqual(2, api.GetBalls().Count, "StopSimulation lub ponowne CreateBalls nie wyczyściło starej listy.");
        }
    }
}

