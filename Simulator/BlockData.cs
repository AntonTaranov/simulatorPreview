using System.Collections.Generic;
using BlocksBreaker.Data;
using UnityEngine;
namespace BlocksBreaker.Simulator
{
    public class BlockData : Object2D, ICollider
    {
        public readonly float Height;
        public readonly float Width;

        IChildCollider[] boundingBox;

        public Vector2 TopLeft { get; private set; }
        public Vector2 TopRight { get; private set; }
        public Vector2 BottomLeft { get; private set; }
        public Vector2 BottomRight { get; private set; }

        public float Offset { get; private set; }

        public uint Health { get; private set; }
        public uint maxHealth;

        public BlockData(uint health, float width, float height)
        {
            this.Width = width;
            this.Height = height;
            
            this.Health = health;
            maxHealth = health;
        }

        public void CreateBoundingBoxWithOffset(float value)
        {
            boundingBox = new IChildCollider[8];

            Offset = value;

            BottomLeft = new Vector2(-Width * 0.5f, -Height * 0.5f);
            TopLeft = new Vector2(-Width * 0.5f, Height * 0.5f);
            BottomRight = new Vector2(Width * 0.5f, -Height * 0.5f);
            TopRight = new Vector2(Width * 0.5f, Height * 0.5f);

            var position = GetPosition();
            //lines
            boundingBox[0] = new BoundingLine(this, position + BottomLeft, position + TopLeft);
            boundingBox[0].Move(new Vector2(-value, 0));
            boundingBox[1] = new BoundingLine(this, position + TopLeft, position + TopRight);
            boundingBox[1].Move(new Vector2(0, value));
            boundingBox[2] = new BoundingLine(this, position + TopRight, position + BottomRight);
            boundingBox[2].Move(new Vector2(value, 0));
            boundingBox[3] = new BoundingLine(this, position + BottomRight, position + BottomLeft);
            boundingBox[3].Move(new Vector2(0, - value));
            //vertexes
            boundingBox[4] = new VertexCollider(position + BottomLeft, this);
            boundingBox[5] = new VertexCollider(position + TopLeft, this);
            boundingBox[6] = new VertexCollider(position + BottomRight, this);
            boundingBox[7] = new VertexCollider(position + TopRight, this);
        }

        private void MoveBoundingBox(Vector2 value)
        {
            foreach (var childCollider in boundingBox)
            {
                childCollider.Move(value);
            }
        }

        public override void SetPositionX(float value)
        {
            var lastPosition = GetPosition();
            base.SetPositionX(value);
            MoveBoundingBox(GetPosition() - lastPosition);
        }

        public override void SetPositionY(float value)
        {
            var lastPosition = GetPosition();
            base.SetPositionY(value);
            MoveBoundingBox(GetPosition() - lastPosition);
        }

        public bool CanCollideWith(ICollider collider)
        {
            return IsAlive && collider is BallData;
        }

        public Collision GetCollision(ICollider collider, float deltaTime)
        {
            if (IsAlive && collider is BallData)
            {
                var circle = collider as BallData;
                Collision firstCollision = null;
                foreach(var boundingLine in boundingBox)
                {
                    var collision = boundingLine.GetCollision(circle, deltaTime);
                    if (collision != null)
                    {
                        if (firstCollision == null || firstCollision.Time > collision.Time)
                        {
                            firstCollision = collision;
                        }
                    }
                }
                return firstCollision;
            }
            return null;
        }

        public void ProcessCollision(Collision collision)
        {
            if (IsAlive)
            {
                Health -= 1;
            }
        }

        public void Update(float deltaTime) { }

        public ICollider GetUsedCollider()
        {
            return this;
        }

        public override bool IsAlive => Health > 0;
    }
}
