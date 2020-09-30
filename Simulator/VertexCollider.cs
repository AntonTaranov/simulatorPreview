using BlocksBreaker.Utils;
using UnityEngine;

namespace BlocksBreaker.Simulator
{
    public class VertexCollider : IChildCollider 
    {
        Vector2 position;
        ICollider parent;

        public VertexCollider(Vector2 position, ICollider parent)
        {
            this.position = position;
            this.parent = parent;
        }

        public Vector2 GetNormalTo(Vector2 point)
        {
            return (point - position).normalized;
        }

        public bool CanCollideWith(ICollider collider)
        {
            return collider is BallData;
        }

        public Collision GetCollision(ICollider collider, float deltaTime)
        {
            if (collider is BallData)
            {
                var time = MathOperations.GetCollisionTimeWithBall(position, collider as BallData, deltaTime, 0);

                if (time <= deltaTime)
                {
                    return new Collision(this, collider, time);
                }
            }
            return null;
        }

        public ICollider GetParent()
        {
            return parent;
        }

        public ICollider GetUsedCollider()
        {
            return GetParent();
        }

        public void Move(Vector2 value)
        {
            position += value;
        }

        public void ProcessCollision(Collision collision)
        {
            GetParent().ProcessCollision(collision);
        }

        public void Update(float deltaTime)
        {

        }
    }
}
