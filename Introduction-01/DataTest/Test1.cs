using Data;

namespace DataTest
{
    [TestClass]
    public sealed class Test1
    {
        private DataAbstractAPI _api;
        [TestInitialize]
        public void Setup()
        {
            // Wykonuje się automatycznie przed każdym testem.
            _api = DataAbstractAPI.CreateAPI(640, 400);
        }

        [TestCleanup]
        public void Teardown()
        {
            // Wykonuje się automatycznie po każdym teście.
            _api?.StopSimulation();
        }
        [TestMethod]
        public void TestCreateBallsPositionAndCount()
        {
            int width = 640;
            int height = 400;
            int ballCount = 10;
            int radius = 20;
            double weight = 1.0;

            _api.CreateBalls(ballCount, radius, weight);
            var balls = _api.GetBalls();

            Assert.HasCount(ballCount, balls);

            foreach (var ball in balls)
            {
                Assert.IsTrue(ball.X >= 0 && ball.X <= width - radius, "Kula X poza zakresem.");
                Assert.IsTrue(ball.Y >= 0 && ball.Y <= height - radius, "Kula Y poza zakresem.");

                // Sprawdzenie czy masa, promień są poprawnie przypisane
                Assert.AreEqual(radius, ball.Diameter, "Promień kuli jest nieprawidłowy.");
                Assert.AreEqual(weight, ball.Weight, "Masa kuli jest nieprawidłowa.");
            }
        }

        [TestMethod]
        public async Task TestBallsMovement_RealTime()
        {
            _api.CreateBalls(1, 20, 1.0);
            var ball = _api.GetBalls()[0];

            double initialX = ball.X;
            double initialY = ball.Y;

            // Używamy await Task.Delay, aby symulacja w tle mogła zadziałać
            await Task.Delay(100);

            Assert.IsTrue(ball.X != initialX || ball.Y != initialY, "Kula nie poruszyła się w czasie rzeczywistym!");
        }

        [TestMethod]
        public void TestStopSimulationClearsBalls()
        {
            _api.CreateBalls(5, 20, 1.0);

            _api.StopSimulation();
            _api.CreateBalls(2, 20, 1.0);
            Assert.AreEqual(2, _api.GetBalls().Count, "StopSimulation lub ponowne CreateBalls nie wyczyściło starej listy.");
        }
    }
}

