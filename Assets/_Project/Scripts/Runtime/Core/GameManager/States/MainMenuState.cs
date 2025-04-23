using UnityEngine;

/// <summary>
/// State for the main menu
/// </summary>
public class MainMenuState : GameState
{
    public MainMenuState(GameManager manager)
        : base(manager, GameStateType.MainMenu) { }

    public override void Enter()
    {
        Debug.Log("Entering Main Menu State");
        if (
            RoomManager.Instance is RoomManager roomManager
            && roomManager.Player is GameObject playerObj
        )
        {
            Object.Destroy(playerObj);
        }
        UIManager.ShowMainMenu();
    }

    public override void UpdateState()
    {
        // Main menu state usually doesn't need per-frame updates
    }

    public override void Exit(GameStateType stateTypeToEnter)
    {
        Debug.Log("Exiting Main Menu State");
        // TODO: Hide main menu UI
        // TODO: Clean up any resources
    }

    // Example of UI event handlers
    private void OnNewGameClicked()
    {
        gameManager.StartNewGame();
    }

    private void OnContinueClicked()
    {
        gameManager.ContinueGame();
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
