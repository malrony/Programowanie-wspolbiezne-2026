using Logic;

namespace BusinessLogicTest
{
    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void TestBallCollisionPhysics()
        {
            // 1. Arrange: Stworzenie API warstwy logiki
            // Zakładając, że CreateAPI() zwraca instancję klasy implementującej LogicAbstractAPI
            LogicAbstractAPI logic = LogicAbstractAPI.CreateAPI();

            // 2. Act: Wywołanie metody logicznej
            int ballsCount = 2;
            logic.StartSimulation(ballsCount);
            var balls = logic.GetBalls();

            // 3. Assert
            Assert.IsNotNull(balls, "Lista kul nie powinna być nullem");
            Assert.AreEqual(ballsCount, balls.Count, "Liczba kul powinna się zgadzać");

            // Sprawdzenie czy pierwsza kula mieści się w granicach (np. 0-600)
            Assert.IsTrue(balls[0].X >= 0, "Kula X jest poza lewą krawędzią");
        }
    }
}