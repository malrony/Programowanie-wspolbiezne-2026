using Presentation.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly PresentationModel _model;
        private int _ballCount;

        public ObservableCollection<BallModel> Balls => _model.Balls;
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public double CanvasWidth
        {
            get => _model.CanvasWidth;
            set { _model.CanvasWidth = value; OnPropertyChanged(); }
        }
        public double CanvasHeight
        {
            get => _model.CanvasHeight;
            set { _model.CanvasHeight = value; OnPropertyChanged(); }
        }

        public int BallCount
        {
            get => _ballCount;
            set { _ballCount = value; OnPropertyChanged(); }
        }

        public MainViewModel(PresentationModel? model = null)
        {
            _model = model ?? new PresentationModel();
            _model.CanvasWidth = 640;
            _model.CanvasHeight = 400;
            StartCommand = new RelayCommand(() => _model.Start(BallCount));
            StopCommand = new RelayCommand(() => _model.Stop());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}