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
                Task.Run(() => ball.Move(_cts.Token, Width, Height));
            }
        }

        public override List<IBall> GetBalls() => _balls;

        public override void StopSimulation()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }

    internal class Ball : IBall
    {
        private double _x;
        private double _y;

        public double X => _x;
        public double Y => _y;
        public int Radius { get; }
        public double Weight { get; }
        public double VX { get; set; }
        public double VY { get; set; }

        public event EventHandler<BallChangedEventArgs>? BallChanged;

        public Ball(double x, double y, int radius, double weight)
        {
            _x = x;
            _y = y;
            Radius = radius;
            Weight = weight;

            Random rand = new Random();
            VX = rand.NextDouble() * 2 - 1;
            VY = rand.NextDouble() * 2 - 1;
        }

        public async Task Move(CancellationToken token, int width, int height)
        {
            while (!token.IsCancellationRequested)
            {
                _x += VX;
                _y += VY;

                if (_x <= 0 || _x + Radius >= width) VX *= -1;
                if (_y <= 0 || _y + Radius >= height) VY *= -1;

                //if (rand.NextDouble() < 0.05)
                //{
                //    vx += (rand.NextDouble() * 0.2 - 0.1);
                //    vy += (rand.NextDouble() * 0.2 - 0.1);

                //    vx = Math.Clamp(vx, -2, 2);
                //    vy = Math.Clamp(vy, -2, 2);
                //}

                BallChanged?.Invoke(this, new BallChangedEventArgs { Ball = this });
                await Task.Delay(16, token);
            }
        }
    }
}