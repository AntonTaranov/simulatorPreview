using System;
using UnityEngine;

namespace BlocksBreaker.Simulator
{
    public class FinishLIne : LineData
    {
        public FinishLIne(Vector2 startPosition, Vector2 finishPosition) : base(startPosition, finishPosition)
        {
        }

        public override void ProcessCollision(Collision collision)
        {
            base.ProcessCollision(collision);
            BallData ball = null;
            if (collision.pair.first is BallData)
            {
                ball = collision.pair.first as BallData;
            }
            else if (collision.pair.second is BallData)
            {
                ball = collision.pair.second as BallData;
            }

            if (ball != null)
            {
                ball.Kill();
            }
        }
    }
}
