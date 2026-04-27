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
        private readonly object _lock = new object();

        public LogicApi(DataAbstractAPI data)
        {
            _data = data;
        }

        public override void StartSimulation(int ballsCount)
        {
            _data.CreateBalls(ballsCount, 20, 1.0);

            foreach (var ball in _data.GetBalls())
            {
                ball.BallChanged += OnBallChanged;
            }
        }

        public override void StopSimulation()
        {
            _data.StopSimulation();
        }

        private void OnBallChanged(object sender, BallChangedEventArgs e)
        {
            if (e.Ball == null) return;

            lock (_lock)
            {
                CheckWallCollision(e.Ball);
            }
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