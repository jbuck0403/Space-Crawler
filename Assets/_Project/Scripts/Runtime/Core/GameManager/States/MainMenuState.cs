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
        // AudioManager.PlayMainMenuMusic();
    }

    public override void UpdateState() { }

    public override void Exit(GameStateType stateTypeToEnter)
    {
        Debug.Log("Exiting Main Menu State");
    }
}
