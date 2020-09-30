using System;
namespace BlocksBreaker.Simulator
{
    public class Collision
    {
        public readonly float Time;
        public readonly Pair<ICollider> pair;
        public Collision(ICollider first, ICollider second, float timeToCollision)
        {
            pair = new Pair<ICollider>(first, second);
            Time = timeToCollision;
        }

        public void Process()
        {
            pair.first.ProcessCollision(this);
            pair.second.ProcessCollision(this);
        }
    }
}
