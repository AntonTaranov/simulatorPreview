namespace BlocksBreaker.Simulator
{
    public interface ICollider
    {
        Collision GetCollision(ICollider collider, float deltaTime);
        void ProcessCollision(Collision collision);
        bool CanCollideWith(ICollider collider);
        ICollider GetUsedCollider();
        void Update(float deltaTime);
    }
}
