using System.Collections.Generic;
using BlocksBreaker.Data;
using UnityEngine;

namespace BlocksBreaker.Simulator
{
    public class Cell
    {
        readonly float x;
        readonly float y;
        readonly float width;
        readonly float height;

        public readonly List<ICollider> Colliders;

        public Cell(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            Colliders = new List<ICollider>();
        }

        public bool IsInside(ICollider collider, float deltaTime)
        {
            if (collider is BallData)
            {
                var circle = collider as BallData;
                var circlePosition = circle.GetPosition();
                var circleRadius = circle.GetRadius();
                if (IsCircleInside(circlePosition, circleRadius))
                {
                    return true;
                }
                circlePosition = circlePosition + circle.GetSpeed() * deltaTime;
                return IsCircleInside(circlePosition, circleRadius);
            }
            else if (collider is BlockData)
            {
                var block = collider as BlockData;
                return IsRectangeInside(block.GetPosition(), block.Width,
                         block.Height, block.Offset);
            }
            else if(collider is BonusData)
            {
                BonusData bonus = collider as BonusData;
                return IsCircleInside(bonus.GetPosition(), bonus.GetCollidingRadius());
            }
            return false;
        }

        private bool IsRectangeInside(Vector2 position, float rectangleWidth, float rectangleHeight, float offset = 0)
        {
            // If one rectangle is on left side of other 
            if (x > position.x + rectangleWidth * 0.5f + offset || position.x - rectangleWidth * 0.5f - offset> x + width)
                return false;

            // If one rectangle is above other 
            if (y > position.y + rectangleHeight * 0.5f + offset || position.y - rectangleHeight * 0.5f - offset > y + height)
                return false;

            return true;
        }

        public bool IsCircleInside(Vector2 postition, float radius)
        {

            if (x > postition.x + radius) return false;
            if (x + width < postition.x - radius) return false;
            if (y > postition.y + radius) return false;
            if (y + height < postition.y - radius) return false;
            return true;
        }
    }
}
