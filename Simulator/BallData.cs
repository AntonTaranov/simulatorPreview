using System;
using BlocksBreaker.Data;
using UnityEngine;
namespace BlocksBreaker.Simulator
{
    public class BallData : Circle2D, ICollider
    {
        Vector2 speed;

        bool alive;

        public BallData(float radius) : base(radius)
        {
            alive = true;
        }

        public void SetSpeed(Vector2 value)
        {
            speed = value;
        }

        public Vector2 GetSpeed()
        {
            return speed;
        }

        public void HitWithNormal(Vector2 normal)
        {
            var projection = Vector2.Dot(speed, normal) * normal * 2;
            speed -= projection;
        }

        public void UpdatePosition(float deltaTime)
        {
            var newPosition = GetPosition() + deltaTime * speed;
            SetPositionX(newPosition.x);
            SetPositionY(newPosition.y);
        }

        public void InvertSpeed(bool x, bool y)
        {
            speed.x = x ? -speed.x : speed.x;
            speed.y = y ? -speed.y : speed.y;
        }

        private void HitWithLine(ICollider collider)
        {
            if(collider is LineData)
            {
                HitWithNormal((collider as LineData).Normal);
            }
            else if (collider is VertexCollider)
            {
                HitWithNormal((collider as VertexCollider).GetNormalTo(GetPosition()));   
            }
        }

        public Collision GetCollision(ICollider collider, float deltaTime)
        {
            if (collider is LineData || collider is BlockData || collider is BonusData)
            {
                return collider.GetCollision(this, deltaTime);
            }
            return null;
        }

        public void ProcessCollision(Collision collision)
        {
            UpdatePosition(collision.Time);
            HitWithLine(collision.pair.first != this ?
                              collision.pair.first : collision.pair.second);
        }

        public bool CanCollideWith(ICollider collider)
        {
            return collider is LineData || collider is BlockData || collider is BonusData;
        }

        public void Update(float deltaTime)
        {
            if (alive)
            {
                UpdatePosition(deltaTime);
            }
        }

        public override bool IsAlive => alive;

        public void Kill()
        {
            alive = false;
        }

        public ICollider GetUsedCollider()
        {
            return this;
        }
    }
}
