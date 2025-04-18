using UnityEngine;

/// <summary>
/// State for initializing gameplay (loading the starting room, spawning player)
/// </summary>
public class GameplayInitState : GameState
{
    private bool initializationComplete = false;

    public GameplayInitState(GameManager manager)
        : base(manager, GameStateType.GameplayInit) { }

    public override void Enter()
    {
        Debug.Log("Entering Gameplay Initialization State");
        initializationComplete = false;

        // Find or create RoomManager instance
        if (RoomManager.Instance == null)
        {
            Debug.Log("Creating RoomManager instance");
            // Try to find an existing inactive manager first
            RoomManager existingManager = Object.FindObjectOfType<RoomManager>(true);

            if (existingManager != null)
            {
                Debug.Log("Found existing RoomManager - activating it");
                existingManager.gameObject.SetActive(true);
            }
            else
            {
                // Instantiate new RoomManager if none exists
                GameObject roomManagerPrefab = gameManager.roomManagerPrefab;
                if (roomManagerPrefab != null)
                {
                    Object.Instantiate(roomManagerPrefab);
                    Debug.Log("RoomManager instantiated from prefab");
                }
                else
                {
                    // Create a new GameObject with RoomManager component as fallback
                    GameObject managerObj = new GameObject("RoomManager");
                    managerObj.AddComponent<RoomManager>();
                    Debug.Log("RoomManager created manually");
                }
            }
        }

        // Initialize gameplay
        InitializeGameplay();
    }

    public override void UpdateState()
    {
        // Check if initialization is complete and we can transition to gameplay
        if (
            initializationComplete
            && RoomManager.Instance != null
            && RoomManager.Instance.Player != null
        )
        {
            gameManager.ChangeState(GameStateType.GameplayRoom);
        }
    }

    public override void Exit(GameStateType stateTypeToEnter)
    {
        Debug.Log("Exiting Gameplay Initialization State");

        GameObject player = RoomManager.Instance.Player;

        // Ensure HUD is visible and properly set up
        if (RoomManager.Instance != null && player != null)
        {
            PlayerHUD playerHUD = UIManager.ShowPlayerHUD();
            if (playerHUD != null)
            {
                playerHUD.Initialize(player);
            }
            // TODO: Configure any player-specific UI elements
        }
    }

    private void InitializeGameplay()
    {
        if (RoomManager.Instance == null)
        {
            Debug.LogError("Failed to find or create RoomManager");
            return;
        }

        // Initialize the room system with player
        bool success = RoomManager.Instance.Initialize();

        if (success)
        {
            Debug.Log("Room system initialized successfully");
            initializationComplete = true;

            // Set initial GameData counters
            gameManager.GameData.currentRunEnemiesKilled = 0;
            gameManager.GameData.currentRunRoomsCleared = 0;

            // TODO: Initialize player with selected loadout
            // TODO: Configure any gameplay events/listeners
        }
        else
        {
            Debug.LogError("Failed to initialize room system");
        }
    }
}
