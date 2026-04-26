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

            api.CreateBalls(ballCount, radius, 1.0);
            var balls = api.GetBalls();

            Assert.HasCount(ballCount, balls);

            foreach (var ball in balls)
            {
                Assert.IsTrue(ball.X >= 0 && ball.X <= width - radius, "Kula X poza zakresem.");
                Assert.IsTrue(ball.Y >= 0 && ball.Y <= height - radius, "Kula Y poza zakresem.");
            }
        }
    }
}
