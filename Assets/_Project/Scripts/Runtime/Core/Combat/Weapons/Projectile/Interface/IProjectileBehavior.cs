public interface IProjectileBehavior
{
    void Initialize(Projectile projectile, object[] behaviorParams);
    void Cleanup();
}
