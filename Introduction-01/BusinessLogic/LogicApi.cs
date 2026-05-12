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

        public static LogicAbstractAPI CreateAPI(DataAbstractAPI? data = null)
        {
            // Jeśli dane nie zostaną przekazane (np. w testach), tworzymy domyślną warstwę danych
            return new LogicApi(data ?? DataAbstractAPI.CreateAPI(640, 400));
        }
    }

    internal class LogicApi : LogicAbstractAPI
    {
        private readonly DataAbstractAPI _data;
        private readonly object _collisionLock = new object();

        public LogicApi(DataAbstractAPI data)
        {
            _data = data;
        }

        public override void StartSimulation(int ballsCount)
        {
            _data.CreateBalls(ballsCount, 20, 1.0);
            var balls = _data.GetBalls();

            foreach (var ball in _data.GetBalls())
            {
                ball.BallChanged += OnBallChanged;
            }
        }

        public override void StopSimulation()
        {
            _data.StopSimulation();
        }

        private void OnBallChanged(object? sender, BallChangedEventArgs e)
        {
            IBall ball = e.Ball;
            if (ball == null) return;

            // Sekcja krytyczna dla współbieżnych zderzeń
            lock (_collisionLock)
            {
                CheckWallCollision(ball);
                CheckBallCollisions(ball);
            }
        }

        private void CheckBallCollisions(IBall ball)
        {
            foreach (var otherBall in _data.GetBalls())
            {
                if (ball == otherBall) continue;

                double dx = ball.X - otherBall.X;
                double dy = ball.Y - otherBall.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                double minDistance = (ball.Radius / 2.0) + (otherBall.Radius / 2.0);

                if (distance <= minDistance)
                {
                    HandleCollision(ball, otherBall);
                }
            }
        }

        private void HandleCollision(IBall ball1, IBall ball2)
        {
            // Fizyka zderzeń sprężystych 
            double vRelativeX = ball1.VX - ball2.VX;
            double vRelativeY = ball1.VY - ball2.VY;
            double xRelative = ball1.X - ball2.X;
            double yRelative = ball1.Y - ball2.Y;

            // Zapobieganie wielokrotnemu odbiciu (sprawdzenie czy lecą ku sobie)
            if (vRelativeX * xRelative + vRelativeY * yRelative >= 0) return;

            double m1 = ball1.Weight;
            double m2 = ball2.Weight;

            double newVX1 = (ball1.VX * (m1 - m2) + 2 * m2 * ball2.VX) / (m1 + m2);
            double newVY1 = (ball1.VY * (m1 - m2) + 2 * m2 * ball2.VY) / (m1 + m2);
            double newVX2 = (ball2.VX * (m2 - m1) + 2 * m1 * ball1.VX) / (m1 + m2);
            double newVY2 = (ball2.VY * (m2 - m1) + 2 * m1 * ball1.VY) / (m1 + m2);

            ball1.VX = newVX1;
            ball1.VY = newVY1;
            ball2.VX = newVX2;
            ball2.VY = newVY2;
        }

        private void CheckWallCollision(IBall ball)
        {
            if (ball.X <= 0 && ball.VX < 0) ball.VX *= -1;
            if (ball.X + ball.Radius >= _data.Width && ball.VX > 0) ball.VX *= -1;

            if (ball.Y <= 0 && ball.VY < 0) ball.VY *= -1;
            if (ball.Y + ball.Radius >= _data.Height && ball.VY > 0) ball.VY *= -1;
        }

        public override List<IBall> GetBalls() => _data.GetBalls();
    }
}