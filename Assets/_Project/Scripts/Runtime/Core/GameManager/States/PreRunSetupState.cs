using UnityEngine;

/// <summary>
/// State for pre-run setup (talent points and loadout)
/// </summary>
public class PreRunSetupState : GameState
{
    public PreRunSetupState(GameManager manager)
        : base(manager) { }

    public override void Enter()
    {
        Debug.Log("Entering Pre-Run Setup State");

        // TODO: Show talent tree and loadout UI
        // TODO: Load player data (talent points, unlocked items)
        // TODO: Initialize talent tree with current allocation
    }

    public override void UpdateState()
    {
        // Typically doesn't need per-frame updates beyond UI interactions
    }

    public override void Exit(GameStateType stateTypeToEnter)
    {
        Debug.Log("Exiting Pre-Run Setup State");

        // TODO: Hide talent and loadout UI
        // TODO: Save talent selections
        // TODO: Save ammo type selections
    }

    // Example of UI event handlers
    private void OnStartGameClicked()
    {
        // Transition to the gameplay initialization state
        gameManager.ChangeState(GameStateType.GameplayInit);
    }

    private void OnBackToMainMenuClicked()
    {
        // Return to the main menu
        gameManager.ChangeState(GameStateType.MainMenu);
    }
}
