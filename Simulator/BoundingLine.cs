using UnityEngine;

namespace BlocksBreaker.Simulator
{
    public class BoundingLine : LineData, IChildCollider
    {
        ICollider parent;

        public BoundingLine(ICollider parent, Vector2 startPosition, Vector2 finishPosition) : base(startPosition, finishPosition)
        {
            this.parent = parent;
        }

        public void Move(Vector2 value)
        {
            start += value;
            finish += value;
        }

        public override ICollider GetUsedCollider()
        {
            return GetParent();
        }

        public override void ProcessCollision(Collision collision)
        {
            base.ProcessCollision(collision);

            GetParent().ProcessCollision(collision);
        }

        public override Collision GetCollision(ICollider collider, float deltaTime)
        {
            if (collider is BallData)
            {
                return TryGetCollisionWithCircle(collider as BallData, deltaTime, true);
            }
            return null;
        }

        public ICollider GetParent()
        {
            return parent;
        }
    }
}
