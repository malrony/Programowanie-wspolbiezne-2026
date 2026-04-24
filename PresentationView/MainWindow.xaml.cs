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
    }
}