using UnityEngine;

/// <summary>
/// Abstract base class for all game states
/// </summary>
public abstract class GameState
{
    protected GameManager gameManager;

    public GameState(GameManager manager)
    {
        gameManager = manager;
    }

    /// <summary>
    /// Called when entering this state
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// Called every frame while in this state
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    /// Called when exiting this state
    /// </summary>
    public abstract void Exit(GameStateType stateTypeToEnter);

    /// <summary>
    /// Handle input specific to this state
    /// </summary>
    public virtual void HandleInput()
    {
        // Default implementation does nothing
        // Override in concrete states as needed
    }
}
