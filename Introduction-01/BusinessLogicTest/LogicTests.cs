using Logic;
using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinessLogicTest
{
    internal class FakeDataApi : DataAbstractAPI
    {
        public List<IBall> Balls { get; set; } = new List<IBall>();
        public bool CreateBallsCalled = false;
        public override int Width => 640;
        public override int Height => 400;
        public override void CreateBalls(int count, int radius, double weight) => CreateBallsCalled = true;
        public override List<IBall> GetBalls() => Balls;
        public override void StopSimulation() { }
    }

    internal class FakeBall : IBall
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Diameter { get; set; } = 20;
        public double VX { get; set; }
        public double VY { get; set; }
        public double Weight { get; set; } = 1.0;
        public event EventHandler<BallChangedEventArgs>? BallChanged;

        public void TriggerChange() => BallChanged?.Invoke(this, new BallChangedEventArgs { Ball = this });
    }

    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void TestBallCollisionPhysics()
        {
            // Dwie kule lecące na czołowe zderzenie
            var fakeData = new FakeDataApi();
            var ball1 = new FakeBall { X = 100, Y = 100, VX = 2, VY = 0, Weight = 1.0 };
            var ball2 = new FakeBall { X = 118, Y = 100, VX = -2, VY = 0, Weight = 1.0 };
            fakeData.Balls.Add(ball1);
            fakeData.Balls.Add(ball2);

            var logic = LogicAbstractAPI.CreateAPI(fakeData);

            // Rejestrujemy kule w logice (podpięcie zdarzeń)
            logic.StartSimulation(0); // 0, bo kule dodaliśmy ręcznie do fake

            // Symulujemy ruch ball1, co powinno wywołać sprawdzenie kolizji w logice
            ball1.TriggerChange();

            // Kule powinny odbić się od siebie (zmienić zwrot VX)
            Assert.IsTrue(ball1.VX < 0, "Kula 1 powinna zmienić kierunek na ujemny.");
            Assert.IsTrue(ball2.VX > 0, "Kula 2 powinna zmienić kierunek na dodatni.");
        }

        [TestMethod]
        public void TestWallCollision()
        {
            // Kula uderzająca w lewą ścianę
            var fakeData = new FakeDataApi();
            var ball = new FakeBall { X = -1, Y = 100, VX = -2, VY = 0 };
            fakeData.Balls.Add(ball);
            var logic = LogicAbstractAPI.CreateAPI(fakeData);
            logic.StartSimulation(0);

            ball.TriggerChange();

            Assert.IsTrue(ball.VX > 0, "Kula powinna odbić się od lewej ściany (VX > 0).");
        }
    }
}