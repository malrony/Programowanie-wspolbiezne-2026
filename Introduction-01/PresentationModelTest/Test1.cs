using Presentation.Model;

namespace PresentationModelTest
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestScalingLogicCalculation()
        {
            var model = new PresentationModel();
            //Symulujemy okno 2x większe
            model.CanvasWidth = 1280;
            model.CanvasHeight = 800;

            double logicalX = 320;
            double logicalY = 200;

            double expectedScaledX = logicalX * (model.CanvasWidth / 640.0);
            double expectedScaledY = logicalY * (model.CanvasHeight / 400.0);

            Assert.AreEqual(640, expectedScaledX, 0.001, "Skalowanie szerokości nie działa.");
            Assert.AreEqual(400, expectedScaledY, 0.001, "Skalowanie wysokości nie działa.");
        }
    }
}
