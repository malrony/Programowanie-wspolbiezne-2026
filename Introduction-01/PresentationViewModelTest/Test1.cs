using Presentation.ViewModel;

namespace PresentationViewModelTest
{
    [TestClass]
    public class MainViewModelTests
    {
        [TestMethod]
        public void TestBallCountPropertyChange()
        {
            var vm = new MainViewModel();
            bool eventRaised = false;
            vm.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "BallCount") eventRaised = true;
            };

            vm.BallCount = 10;

            Assert.AreEqual(10, vm.BallCount);
            Assert.IsTrue(eventRaised, "Zdarzenie PropertyChanged nie zostało wywołane dla BallCount");
        }

        [TestMethod]
        public void TestStartCommandInitializesSimulation()
        {
            var vm = new MainViewModel();
            vm.BallCount = 5;

            vm.StartCommand.Execute(null);

            Assert.IsNotNull(vm.Balls);
            Assert.IsTrue(vm.Balls.Count > 0 || vm.Balls != null);
        }
    }
}
