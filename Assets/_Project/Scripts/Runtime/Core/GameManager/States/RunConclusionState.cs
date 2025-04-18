using UnityEngine;

/// <summary>
/// State for the end of a run (success or failure)
/// </summary>
public class RunConclusionState : GameState
{
    public RunConclusionState(GameManager manager)
        : base(manager, GameStateType.RunConclusion) { }

    public override void Enter()
    {
        Debug.Log("Entering Run Conclusion State");

        // TODO: Show run conclusion UI
        // TODO: Display rewards if successful
        // TODO: Show run statistics

        // Make sure game is not paused
        Time.timeScale = 1f;
    }

    public override void UpdateState()
    {
        // Minimal updates needed in this state
    }

    public override void Exit(GameStateType stateTypeToEnter)
    {
        Debug.Log("Exiting Run Conclusion State");

        // TODO: Hide run conclusion UI
    }

    // Example of UI event handlers
    private void OnContinueClicked()
    {
        // Go back to talent selection for the next run
        gameManager.ChangeState(GameStateType.PreRunSetup);
    }

    private void OnMainMenuClicked()
    {
        // Return to main menu
        gameManager.ChangeState(GameStateType.MainMenu);
    }
}
