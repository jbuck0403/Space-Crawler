using System;
using UnityEngine;

/// <summary>
/// Base class for projectile behaviors that handles common functionality
/// </summary>
public abstract class BaseProjectileBehavior : MonoBehaviour, IProjectileBehavior
{
    protected Projectile projectile;

    public event Action OnCleanupComplete;

    /// <summary>
    /// Explicit interface implementation that delegates to the type-specific implementation
    /// </summary>
    void IProjectileBehavior.Initialize(Projectile projectile, params object[] parameters)
    {
        this.projectile = projectile;

        // allow derived classes to handle their specific parameter requirements
        InitializeFromParams(parameters);
    }

    /// <summary>
    /// Abstract method that derived classes must implement to handle their specific parameters
    /// </summary>
    protected abstract void InitializeFromParams(object[] parameters);

    /// <summary>
    /// Clean up resources and handle component removal
    /// </summary>
    public virtual void Cleanup()
    {
        Destroy(this);
    }

    protected void OnDestroy()
    {
        OnCleanupComplete?.Invoke();
    }
}
