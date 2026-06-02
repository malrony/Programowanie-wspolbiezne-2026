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

            CheckWallCollision(ball);
            CheckBallCollisions(ball);
        }

        private void CheckBallCollisions(IBall ball)
        {
            foreach (var otherBall in _data.GetBalls())
            {
                if (ball == otherBall) continue;

                double dx = ball.X - otherBall.X;
                double dy = ball.Y - otherBall.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                double minDistance = (ball.Diameter / 2.0) + (otherBall.Diameter / 2.0) - 1.5;

                if (distance <= minDistance)
                {
                    
                    IBall firstLock = ball.Id < otherBall.Id ? ball : otherBall;
                    IBall secondLock = ball.Id < otherBall.Id ? otherBall : ball;

                    lock (firstLock)
                    {
                        lock (secondLock)
                        {
                           
                            dx = ball.X - otherBall.X;
                            dy = ball.Y - otherBall.Y;
                            distance = Math.Sqrt(dx * dx + dy * dy);

                            if (distance <= minDistance)
                            {
                               
                                HandleCollision(ball, otherBall);
                            }
                        }
                    }
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

            double apporachIndicator = vRelativeX * xRelative + vRelativeY * yRelative;
            if (apporachIndicator >= 0) return; // Kule się oddalają

            double distSquared = xRelative * xRelative + yRelative * yRelative;
            if (distSquared == 0) return;

            double m1 = ball1.Weight;
            double m2 = ball2.Weight;

            double impulseFactor = apporachIndicator / (distSquared * (m1 + m2));

            ball1.VX -= 2 * m2 * impulseFactor * xRelative;
            ball1.VY -= 2 * m2 * impulseFactor * yRelative;
            ball2.VX += 2 * m1 * impulseFactor * xRelative;
            ball2.VY += 2 * m1 * impulseFactor * yRelative;
        }

        private void CheckWallCollision(IBall ball)
        {
            lock (ball)
            {
                if (ball.X <= 0 && ball.VX < 0) ball.VX *= -1;
                if (ball.X + ball.Diameter >= _data.Width && ball.VX > 0) ball.VX *= -1;

                if (ball.Y <= 0 && ball.VY < 0) ball.VY *= -1;
                if (ball.Y + ball.Diameter >= _data.Height && ball.VY > 0) ball.VY *= -1;
            }
        }
        public override List<IBall> GetBalls() => _data.GetBalls();
    }
}