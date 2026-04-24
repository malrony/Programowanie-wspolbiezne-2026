using Data;
using System;
using System.Collections.Generic;

namespace Logic
{
    public abstract class LogicAbstractAPI
    {
        public abstract void StartSimulation(int ballsCount);
        public abstract void StopSimulation();
        public abstract List<IBall> GetBalls();

        public static LogicAbstractAPI CreateAPI(DataAbstractAPI data = null)
        {
            return new LogicApi(data ?? DataAbstractAPI.CreateAPI(640, 400));
        }
    }

    internal class LogicApi : LogicAbstractAPI
    {
        private readonly DataAbstractAPI _data;
        private readonly object _lock = new object();

        public LogicApi(DataAbstractAPI data)
        {
            _data = data;
        }

        public override void StartSimulation(int ballsCount)
        {
            _data.CreateBalls(ballsCount, 20, 1.0);
        }

        public override void StopSimulation()
        {
            _data.StopSimulation();
        }

        private void OnBallChanged(object sender, BallChangedEventArgs e)
        {
            IBall ball = e.Ball;

            lock (_lock)
            {
                CheckWallCollision(ball);
                CheckBallCollision(ball);
            }
        }

        private void CheckWallCollision(IBall ball)
        {
            if (ball.X <= 0 && ball.VX < 0) ball.VX *= -1;
            if (ball.X + ball.Radius >= _data.Width && ball.VX > 0) ball.VX *= -1;

            if (ball.Y <= 0 && ball.VY < 0) ball.VY *= -1;
            if (ball.Y + ball.Radius >= _data.Height && ball.VY > 0) ball.VY *= -1;
        }

        private void CheckBallCollision(IBall ball)
        {
            foreach (var other in _data.GetBalls())
            {
                if (ball == other) continue;

                double dx = (other.X + other.Radius / 2.0) - (ball.X + ball.Radius / 2.0);
                double dy = (other.Y + other.Radius / 2.0) - (ball.Y + ball.Radius / 2.0);
                double distance = Math.Sqrt(dx * dx + dy * dy);
                double minDistance = (ball.Radius + other.Radius) / 2.0;

                if (distance <= minDistance)
                {
                    double vdx = other.VX - ball.VX;
                    double vdy = other.VY - ball.VY;
                    double dotProduct = dx * vdx + dy * vdy;

                    if (dotProduct < 0)
                    {
                        double m1 = ball.Weight;
                        double m2 = other.Weight;

                        double v1x = ball.VX;
                        double v1y = ball.VY;
                        double v2x = other.VX;
                        double v2y = other.VY;

                        ball.VX = (v1x * (m1 - m2) + (2 * m2 * v2x)) / (m1 + m2);
                        ball.VY = (v1y * (m1 - m2) + (2 * m2 * v2y)) / (m1 + m2);

                        other.VX = (v2x * (m2 - m1) + (2 * m1 * v1x)) / (m1 + m2);
                        other.VY = (v2y * (m2 - m1) + (2 * m1 * v1y)) / (m1 + m2);
                    }
                }
            }
        }

        public override List<IBall> GetBalls() => _data.GetBalls();
    }
}