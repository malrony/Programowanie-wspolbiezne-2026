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
            Assert.IsTrue(eventRaised, "ViewModel nie powiadomił Widoku o zmianie liczby kulek.");
        }

        [TestMethod]
        public void TestStartCommandInitializesSimulation()
        {
            var vm = new MainViewModel();
            vm.BallCount = 5;

            vm.StartCommand.Execute(null);

            Assert.IsNotNull(vm.Balls);
            Assert.HasCount(5, vm.Balls);
        }
    }
}
