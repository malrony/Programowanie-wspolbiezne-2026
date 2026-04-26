using System.Windows;
using Presentation.ViewModel;

namespace PresentationView
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel(); 
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.DataContext is MainViewModel vm)
            {
                vm.CanvasWidth = BallsControl.ActualWidth;
                vm.CanvasHeight = BallsControl.ActualHeight;
            }
        }
    }
}