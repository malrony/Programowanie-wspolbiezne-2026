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

        public int BallCount
        {
            get => _ballCount;
            set { _ballCount = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            _model = new PresentationModel();
            StartCommand = new RelayCommand(() => _model.Start(BallCount));
            StopCommand = new RelayCommand(() => _model.Stop());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}