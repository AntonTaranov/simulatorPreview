using System;
using BlocksBreaker.Data;
using BlocksBreaker.Utils;
using UnityEngine;

namespace BlocksBreaker.Simulator
{
    public class BonusData : Circle2D, ICollider
    {
        bool consumed = false;
        bool alive = true;
        float offset;
        
        public BonusData(float radius, float offset) : base(radius)
        {
            this.offset = offset;
        }

        public float GetCollidingRadius()
        {
            return GetRadius() + offset;
        }

        public bool CanCollideWith(ICollider collider)
        {
            return IsAlive && collider is BallData;
        }

        public Collision GetCollision(ICollider collider, float deltaTime)
        {
            if (collider is BallData)
            {
                var time = MathOperations.GetCollisionTimeWithBall(GetPosition(), collider as BallData, deltaTime, GetRadius() * 0.5f);

                if (time <= deltaTime)
                {
                    return new Collision(this, collider, time);
                }
            }
            return null;
        }

        public ICollider GetUsedCollider()
        {
            return this;
        }

        public void ProcessCollision(Collision collision)
        {
            alive = false;
        }

        public bool ConsumeBonus()
        {
            if (!consumed)
            {
                consumed = true;
                return true;
            }
            return false;
        }

        public void Update(float deltaTime) { }

        public override bool IsAlive => alive;
    }
}
