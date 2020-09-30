using BlocksBreaker.Utils;
using UnityEngine;
namespace BlocksBreaker.Simulator
{
    /// <summary>
    /// Line data - two points and left side normal.
    /// </summary>
    public class LineData : ICollider
    {
        readonly public Vector2 Normal;

        protected Vector2 start;
        protected Vector2 finish;

        public LineData(Vector2 startPosition, Vector2 finishPosition)
        {
            var direction = finishPosition - startPosition;
            Normal = new Vector2(-direction.y, direction.x).normalized;

            start = startPosition;
            finish = finishPosition;
        }

        public bool CanCollideWith(ICollider collider)
        {
            return collider is BallData;
        }

        public virtual Collision GetCollision(ICollider collider, float deltaTime)
        {
            if (collider is BallData)
            {
                return TryGetCollisionWithCircle(collider as BallData, deltaTime);
            }
            return null;
        }

        protected Collision TryGetCollisionWithCircle(BallData circle, float deltaTime, bool ignoreRadius = false)
        {

            //check speed direction
            if (Vector2.Dot(circle.GetSpeed(), Normal) > 0)
                return null;

            var trajectoryStart = circle.GetPosition();
            var trajectoryFinish = trajectoryStart + circle.GetSpeed() * deltaTime;

            //move line towards circle
            var lineStart = start;
            var lineFinish = finish;
            if (!ignoreRadius)
            {
                var offset = Normal * circle.GetRadius();
                lineStart += offset;
                lineFinish += offset;
            }

            Debug.DrawLine(lineStart, lineFinish, Color.yellow);

            var intersection = new Vector2();

            if (MathOperations.LineSegementsIntersect(trajectoryStart, trajectoryFinish,
                                    lineStart, lineFinish, out intersection, true))
            {
                var toIntersection = intersection - trajectoryStart;
                var circleSpeed = circle.GetSpeed();
                var speedX = circleSpeed.x < 0 ? -circleSpeed.x : circleSpeed.x;
                var speedY = circleSpeed.y < 0 ? -circleSpeed.y : circleSpeed.y;
                var timeToCollision = speedX > speedY ? toIntersection.x / circleSpeed.x :
                                        toIntersection.y / circleSpeed.y;
                return new Collision(this, circle, timeToCollision);
            }
            return null;
        }

        public virtual ICollider GetUsedCollider()
        {
            return this;
        }

        public virtual void ProcessCollision(Collision collision) { }

        public void Update(float deltaTime) { }
    }
}
