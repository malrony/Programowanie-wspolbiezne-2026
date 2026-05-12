using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    public abstract class DataAbstractAPI
    {
        public abstract void CreateBalls(int count, int radius, double weight);
        public abstract List<IBall> GetBalls();
        public abstract void StopSimulation();
        public abstract int Width { get; }
        public abstract int Height { get; }

        public static DataAbstractAPI CreateAPI(int width, int height)
        {
            return new DataApi(width, height);
        }
    }

    public interface IBall
    {
        double X { get; }
        double Y { get; }
        int Radius { get; }
        double VX { get; set; }
        double VY { get; set; }
        double Weight { get; }
        event EventHandler<BallChangedEventArgs>? BallChanged;
    }

    public class BallChangedEventArgs : EventArgs
    {
        public IBall? Ball { get; set; }
    }

    internal class DataApi : DataAbstractAPI
    {
        private readonly List<IBall> _balls = new List<IBall>();
        private CancellationTokenSource? _cts;
        public override int Width { get; }
        public override int Height { get; }

        public DataApi(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override void CreateBalls(int count, int radius, double weight)
        {
            StopSimulation();
            _balls.Clear();
            _cts = new CancellationTokenSource();
            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                var ball = new Ball(
                    rand.Next(0, Width - radius),
                    rand.Next(0, Height - radius),
                    radius,
                    weight
                );

                _balls.Add(ball);

                // Move nie przyjmuje już Width i Height, bo za ściany odpowiada Logika
                // Task.Run zapewnia, że każda kula porusza się współbieżnie
                Task.Run(() => ball.Move(_cts.Token));
            }
        }

        public override List<IBall> GetBalls() => new List<IBall>(_balls);

        public override void StopSimulation()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
    }

    internal class Ball : IBall
    {
        private double _x;
        private double _y;
        private double _vx;
        private double _vy;
        private readonly object _lock = new object(); // Sekcja krytyczna kuli

        public double X { get { lock (_lock) return _x; } }
        public double Y { get { lock (_lock) return _y; } }
        public int Radius { get; }
        public double Weight { get; }

        public double VX
        {
            get { lock (_lock) return _vx; }
            set { lock (_lock) _vx = value; }
        }
        public double VY
        {
            get { lock (_lock) return _vy; }
            set { lock (_lock) _vy = value; }
        }

        public event EventHandler<BallChangedEventArgs>? BallChanged;

        public Ball(double x, double y, int radius, double weight)
        {
            _x = x;
            _y = y;
            Radius = radius;
            Weight = weight;

            Random rand = new Random();
            _vx = rand.NextDouble() * 4 - 2;
            _vy = rand.NextDouble() * 4 - 2;
        }

        public async Task Move(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // Sekcja krytyczna: aktualizacja pozycji
                lock (_lock)
                {
                    _x += _vx;
                    _y += _vy;
                }

                // Powiadomienie logiki i modelu o zmianie
                BallChanged?.Invoke(this, new BallChangedEventArgs { Ball = this });

                try
                {
                    await Task.Delay(16, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
    }
}