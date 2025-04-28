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
        Debug.LogWarning("RunConclusionState.Enter - About to show RunConclusion UI");

        UIManager.ShowRunConclusion(!gameManager.playerDied);
        gameManager.playerDied = false;

        Debug.LogWarning("RunConclusionState.Enter - Called UIManager.ShowRunConclusion");

        // Make sure game is not paused
        Time.timeScale = 1f;
    }

    public override void UpdateState() { }

    public override void Exit(GameStateType stateTypeToEnter)
    {
        Debug.Log("Exiting Run Conclusion State");
    }

    // Example of UI event handlers
    private void OnContinueClicked()
    {
        // Go back to talent selection for the next run
        // gameManager.ChangeState(GameStateType.GameplayInit);
    }

    private void OnMainMenuClicked()
    {
        // Return to main menu
        gameManager.ChangeState(GameStateType.MainMenu);
    }
}
