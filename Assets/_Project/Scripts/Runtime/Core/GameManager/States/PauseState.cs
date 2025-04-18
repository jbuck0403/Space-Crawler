using UnityEngine;

/// <summary>
/// State for when the game is paused
/// </summary>
public class PauseState : GameState
{
    public PauseState(GameManager manager)
        : base(manager, GameStateType.Pause) { }

    public override void Enter()
    {
        Debug.Log("Entering Pause State");

        // Pause gameplay
        Time.timeScale = 0f;

        // TODO: Show pause menu
        // TODO: Disable some gameplay systems
    }

    public override void UpdateState()
    {
        // Minimal updates needed when paused
    }

    public override void Exit(GameStateType stateTypeToEnter)
    {
        Debug.Log("Exiting Pause State");

        // Resume gameplay
        Time.timeScale = 1f;

        // TODO: Hide pause menu
        // TODO: Re-enable gameplay systems
    }

    public override void HandleInput()
    {
        // Check for pause button again to unpause
        if (
            Input.GetKeyDown(KeyCode.Escape)
            && (
                gameManager.CurrentStateType == GameStateType.GameplayRoom
                || gameManager.CurrentStateType == GameStateType.Pause
            )
        )
        {
            gameManager.TogglePause();
        }
    }

    // Example of UI event handlers
    private void OnResumeClicked()
    {
        gameManager.TogglePause();
    }

    private void OnSettingsClicked()
    {
        // TODO: Show settings panel
    }

    private void OnMainMenuClicked()
    {
        // Resume time scale before changing state
        Time.timeScale = 1f;

        // Return to main menu, discarding current run progress
        gameManager.ChangeState(GameStateType.MainMenu);
    }
}
