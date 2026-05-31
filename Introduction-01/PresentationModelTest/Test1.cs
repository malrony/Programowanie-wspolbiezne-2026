using Data;
using Logic;
using Presentation.Model;

namespace PresentationModelTest
{
    [TestClass]
    public sealed class Test1
    {
        internal class FakeBall : IBall
        {
            public double X { get; set; }
            public double Y { get; set; }
            public int Diameter => 20;
            public double VX { get; set; }
            public double VY { get; set; }
            public double Weight => 1.0;

            public int Id => throw new NotImplementedException();

            public event EventHandler<BallChangedEventArgs>? BallChanged;

            public void TriggerMove(double newX, double newY)
            {
                X = newX;
                Y = newY;
                BallChanged?.Invoke(this, new BallChangedEventArgs { Ball = this });
            }
        }
        internal class FakeLogicApi : LogicAbstractAPI
        {
            private List<IBall> _balls = new List<IBall>();
            public override void StartSimulation(int count)
            {
                _balls.Clear();
                for (int i = 0; i < count; i++) _balls.Add(new FakeBall());
            }
            public override void StopSimulation() { }
            public override List<IBall> GetBalls() => _balls;
        }

        [TestClass]
        public sealed class PresentationModelTests
        {
            [TestMethod]
            public void TestStart_CreatesBallModels()
            {
                var fakeLogic = new FakeLogicApi();
                var model = new PresentationModel(fakeLogic);

                model.Start(3);

                Assert.HasCount(3, model.Balls);
            }

            [TestMethod]
            public void TestBallChanged_ScalesPositionCorrectly()
            {
                var fakeLogic = new FakeLogicApi();
                var model = new PresentationModel(fakeLogic);
                model.CanvasWidth = 1280;
                model.CanvasHeight = 800;
                model.Start(1);

                var fakeBall = (FakeBall)fakeLogic.GetBalls()[0];

                fakeBall.TriggerMove(620, 380);

                Assert.AreEqual(1260.0, model.Balls[0].X, 0.001, "Skalowanie X nie zadziałało poprawnie.");
                Assert.AreEqual(780.0, model.Balls[0].Y, 0.001, "Skalowanie Y nie zadziałało poprawnie.");
            }
        }

    }
}