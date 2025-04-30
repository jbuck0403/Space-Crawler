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

        if (RoomManager.Instance == null)
        {
            Debug.Log("Creating RoomManager instance");

            RoomManager existingManager = Object.FindObjectOfType<RoomManager>(true);

            if (existingManager != null)
            {
                Debug.Log("Found existing RoomManager - activating it");
                existingManager.gameObject.SetActive(true);
            }
            else
            {
                GameObject roomManagerPrefab = gameManager.roomManagerPrefab;
                if (roomManagerPrefab != null)
                {
                    Object.Instantiate(roomManagerPrefab);
                    Debug.Log("RoomManager instantiated from prefab");
                }
                else
                {
                    GameObject managerObj = new GameObject("RoomManager");
                    managerObj.AddComponent<RoomManager>();
                    Debug.Log("RoomManager created manually");
                }
            }
        }

        InitializeGameplay();
    }

    public override void UpdateState()
    {
        if (
            initializationComplete
            && RoomManager.Instance != null
            && RoomManager.Instance.Player != null
        )
        {
            gameManager.ChangeState(GameStateType.PreRunSetup);
        }
    }

    public override void Exit(GameStateType stateTypeToEnter)
    {
        Debug.Log("Exiting Gameplay Initialization State");
    }

    private void InitializeGameplay()
    {
        if (RoomManager.Instance == null)
        {
            Debug.LogError("Failed to find or create RoomManager");
            return;
        }

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
