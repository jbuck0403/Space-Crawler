using System;

public interface IProjectileBehavior
{
    event Action OnCleanupComplete;
    void Initialize(Projectile projectile, params object[] parameters);
    void Cleanup();
}
