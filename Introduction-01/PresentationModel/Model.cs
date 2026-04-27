using Logic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Presentation.Model
{
    public class PresentationModel
    {
        private readonly LogicAbstractAPI _logicAPI;
        public ObservableCollection<BallModel> Balls { get; } = new ObservableCollection<BallModel>();

        public double CanvasWidth { get; set; } = 640;
        public double CanvasHeight { get; set; } = 400;

        public PresentationModel(LogicAbstractAPI? logicAPI = null)
        {
            _logicAPI = logicAPI ?? LogicAbstractAPI.CreateAPI();
        }

        public void Start(int ballCount)
        {
            _logicAPI.StartSimulation(ballCount);
            Balls.Clear();

            foreach (var ball in _logicAPI.GetBalls())
            {
                double scaledX = ball.X * (CanvasWidth - ball.Radius) / (640.0 - ball.Radius);
                double scaledY = ball.Y * (CanvasHeight - ball.Radius) / (400.0 - ball.Radius);

                var newBall = new BallModel(scaledX, scaledY, ball.Radius);
                Balls.Add(newBall);

                ball.BallChanged += (s, e) =>
                {
                    if (e.Ball != null)
                    {
                        newBall.X = e.Ball.X * (CanvasWidth - e.Ball.Radius) / (640.0 - e.Ball.Radius);
                        newBall.Y = e.Ball.Y * (CanvasHeight - e.Ball.Radius) / (400.0 - e.Ball.Radius);
                    }
                };
            }
        }

        public void Stop()
        {
            _logicAPI.StopSimulation();
        }
    }

    public class BallModel : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private readonly int _radius;

        public double X
        {
            get => _x;
            set
            {
                if (_x == value) return;
                _x = value;
                OnPropertyChanged();
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (_y == value) return;
                _y = value;
                OnPropertyChanged();
            }
        }

        public int Radius => _radius;

        public BallModel(double x, double y, int radius)
        {
            _x = x;
            _y = y;
            _radius = radius;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}