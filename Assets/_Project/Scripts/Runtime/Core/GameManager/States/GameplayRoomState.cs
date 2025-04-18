using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State for active gameplay in rooms
/// </summary>
public class GameplayRoomState : GameState
{
    // References to key components
    private RoomManager roomManager;
    private GameObject player;
    private HealthSystem playerHealth;

    // Room tracking
    private Room currentRoom;
    private int enemiesRemainingInRoom;
    private List<BaseEnemyController> activeEnemies = new List<BaseEnemyController>();

    // Event subscriptions
    private bool eventsSubscribed = false;

    public GameplayRoomState(GameManager manager)
        : base(manager) { }

    public override void Enter()
    {
        Debug.Log("Entering Gameplay Room State");

        // Ensure game is running at normal speed
        Time.timeScale = 1f;

        // Get references to key objects
        InitReferences();

        // Enable player controls
        if (player != null)
        {
            // TODO: Enable player input/controls
        }

        // Subscribe to events
        SubscribeToEvents();

        // Start monitoring current room
        MonitorCurrentRoom();
    }

    public override void UpdateState()
    {
        // Check for player death
        if (CheckForPlayerDeath())
        {
            HandlePlayerDeath();
            return;
        }

        // Check for room completion
        if (currentRoom != null && enemiesRemainingInRoom <= 0)
        {
            HandleRoomCompleted();
        }

        // Update enemy count if needed
        if (roomManager != null && roomManager.CurrentRoom != currentRoom)
        {
            MonitorCurrentRoom();
        }
    }

    public override void Exit(GameStateType stateTypeToEnter)
    {
        Debug.Log("Exiting Gameplay Room State");

        // Unsubscribe from events
        UnsubscribeFromEvents();

        // If transitioning to conclusion, clean up gameplay elements
        if (stateTypeToEnter == GameStateType.RunConclusion)
        {
            // TODO: Clean up any gameplay-specific resources
        }
        // If pausing, just disable player input but keep everything visible
        else if (stateTypeToEnter == GameStateType.Pause)
        {
            // TODO: Disable player input while paused
        }
    }

    public override void HandleInput()
    {
        // Check for pause button press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.TogglePause();
        }

        // All other gameplay input handled by the player controller
    }

    private void InitReferences()
    {
        roomManager = RoomManager.Instance;

        if (roomManager != null)
        {
            player = roomManager.Player;

            if (player != null)
            {
                playerHealth = player.GetComponent<HealthSystem>();
            }
        }

        if (roomManager == null || player == null)
        {
            Debug.LogError("Failed to initialize GameplayRoomState - missing required components");
        }
    }

    private bool CheckForPlayerDeath()
    {
        if (playerHealth != null)
        {
            return playerHealth.IsDead;
        }
        return false;
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("Player has died - ending run");

        // Complete run with failure
        gameManager.CompleteRun(false);
    }

    private void HandleBossDefeated()
    {
        Debug.Log("Boss defeated - successful run!");

        // Complete run with success
        gameManager.CompleteRun(true);
    }

    private void HandleEnemyDefeated(BaseEnemyController enemy)
    {
        // Update enemy count in current room
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            enemiesRemainingInRoom--;

            // Update game data
            gameManager.GameData.currentRunEnemiesKilled++;

            Debug.Log($"Enemy defeated. Remaining: {enemiesRemainingInRoom}");
        }
    }

    private void HandleRoomCompleted()
    {
        Debug.Log("Room completed - unlocking doors");

        // Update game data
        gameManager.GameData.currentRunRoomsCleared++;

        // Unlock doors in the current room
        if (currentRoom != null)
        {
            // Mark room as completed
            // TODO: Implement room completion (unlock doors, etc)

            // Check if this was a boss room
            bool wasBossRoom = false; // TODO: Implement boss room check

            if (wasBossRoom)
            {
                HandleBossDefeated();
            }
        }
    }

    private void MonitorCurrentRoom()
    {
        if (roomManager == null)
            return;

        GameObject roomObj = roomManager.CurrentRoom;
        if (roomObj == null)
            return;

        Room room = roomObj.GetComponent<Room>();
        if (room == null)
            return;

        // Update current room reference
        currentRoom = room;

        // Get all enemies in the room
        activeEnemies.Clear();
        activeEnemies.AddRange(room.spawnedEnemies);
        enemiesRemainingInRoom = activeEnemies.Count;

        Debug.Log($"Monitoring new room with {enemiesRemainingInRoom} enemies");
    }

    private void SubscribeToEvents()
    {
        if (eventsSubscribed)
            return;

        // Subscribe to enemy death events
        // TODO: Subscribe to OnEnemyDefeated event

        eventsSubscribed = true;
    }

    private void UnsubscribeFromEvents()
    {
        if (!eventsSubscribed)
            return;

        // Unsubscribe from events
        // TODO: Unsubscribe from all events

        eventsSubscribed = false;
    }
}
