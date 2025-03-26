using UnityEngine;

/// <summary>
/// Interface defining the contract for providing runtime data to projectiles.
/// Implemented by classes that need to provide dynamic data to projectile behaviors.
/// </summary>
public interface IProjectileDataProvider
{
    /// <summary>
    /// Gets the current target for homing or guided projectiles
    /// </summary>
    Transform GetTarget();
}
