using System.Collections.Generic;
using UnityEngine;
namespace BlocksBreaker.Simulator
{
    public class BallsSpawner
    {
        readonly Vector2 position;
        readonly Vector2 startSpeedDirection;
        readonly float radius;
        readonly float startSpeed;

        float delay;
        uint numBalls;
        float time;

        public BallsSpawner(float radius, Vector2 startPosition, Vector2 direction, uint numBalls, float startSpeed)
        {
            position = startPosition;

            startSpeedDirection = direction;

            this.numBalls = numBalls;
            this.radius = radius;
            this.startSpeed = startSpeed;
        }

        public void SetDelay(float delay)
        {
            this.delay = delay;
            //start immediately
            time = delay;
        }

        public BallData[] Update(float deltaTime)
        {
            time += deltaTime;
            var result = new List<BallData>();
            while (time > delay)
            {
                time -= delay;
                if (numBalls > 0)
                {
                    var newBall = new BallData(radius);
                    newBall.SetPositionX(position.x);
                    newBall.SetPositionY(position.y);
                    newBall.SetSpeed(startSpeedDirection * startSpeed);
                    newBall.UpdatePosition(time);
                    result.Add(newBall);
                }

                numBalls--;
            }
            return result.ToArray();
        }

        public bool IsFinished
        {
            get => numBalls == 0;
        }
    }
}
