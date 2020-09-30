using UnityEngine;

namespace BlocksBreaker.Simulator
{
    public interface IChildCollider : ICollider
    {
        void Move(Vector2 value);
        ICollider GetParent();
    }
}
