using Logic;
using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinessLogicTest
{
    internal class FakeDataApi : DataAbstractAPI
    {
        public bool CreateBallsCalled = false;
        public override int Width => 640;
        public override int Height => 400;
        public override void CreateBalls(int count, int radius, double weight) => CreateBallsCalled = true;
        public override List<IBall> GetBalls() => new List<IBall>();
        public override void StopSimulation() { }
    }

    [TestClass]
    public class LogicTests
    {
        [TestMethod]
        public void TestLogicCallsDataLayer()
        {
            var fakeData = new FakeDataApi();
            var logic = LogicAbstractAPI.CreateAPI(fakeData);

            logic.StartSimulation(5);

            Assert.IsTrue(fakeData.CreateBallsCalled, "Warstwa logiki powinna wywołać CreateBalls w warstwie danych.");
        }
    }
}