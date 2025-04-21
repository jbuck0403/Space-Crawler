using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
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

    private bool roomCompleted = false;
    public bool RoomCompleted => roomCompleted;

    public bool bossDefeated = false;

    private bool enemiesCleared = false;
    private bool roomsSpawned = false;

    // We'll need this flag to track if we've handled this completion already
    private bool roomCompletionHandled = false;

    public GameplayRoomState(GameManager manager)
        : base(manager, GameStateType.GameplayRoom) { }

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

        RoomManager.Instance.PlayerController.Initialize();
    }

    public void SetBossDefeated()
    {
        bossDefeated = true;
    }

    public override void UpdateState()
    {
        // Check for player death
        if (CheckForPlayerDeath())
        {
            HandlePlayerDeath();
            return;
        }

        // Skip if RoomManager isn't ready
        if (roomManager == null)
            return;

        Debug.Log($"#ROOM BOSSDEFEATED: {bossDefeated}");
        // Update game stats if room was just completed
        if (roomManager.IsRoomCompleted && !roomCompletionHandled)
        {
            // Update game data/stats
            gameManager.GameData.currentRunRoomsCleared++;

            roomCompletionHandled = true;
        }

        if (bossDefeated)
        {
            Debug.Log($"#ROOM HANDLEBOSSDEFEATED CALLED");
            HandleBossDefeated();
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

    public void HandleEnemyDefeated(BaseEnemyController enemy)
    {
        // Update enemy count in current room
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            enemiesRemainingInRoom--;

            // Update game data
            gameManager.GameData.currentRunEnemiesKilled++;

            Debug.Log($"#ROOM Enemy defeated. Remaining: {enemiesRemainingInRoom}");

            // Check if all enemies are defeated
            if (enemiesRemainingInRoom <= 0)
            {
                Debug.Log("#ROOM All enemies defeated - marking as cleared");
                enemiesCleared = true;
                CheckRoomCompletion();
            }
        }
    }

    private void HandleRoomCompleted()
    {
        Debug.Log("#ROOM Room completed - unlocking doors");

        // Update game data
        gameManager.GameData.currentRunRoomsCleared++;

        // Unlock doors in the current room
        if (currentRoom != null)
        {
            roomCompleted = true;
            // Mark room as completed
            // TODO: Implement room completion (unlock doors, etc)


            Debug.Log("#ROOM OPENING DOORS WITH ROOMS BEYOND");
            currentRoom.OpenDoorsWithRoomBeyond();

            // Check if this was a boss room
            bool wasBossRoom = false; // TODO: Implement boss room check

            if (wasBossRoom)
            {
                HandleBossDefeated();
            }
        }
    }

    public void MonitorCurrentRoom()
    {
        if (roomManager == null)
            return;

        GameObject roomObj = roomManager.CurrentRoom;
        if (roomObj == null)
            return;

        Room room = roomObj.GetComponent<Room>();
        if (room == null)
            return;

        // Get all enemies in the room
        activeEnemies.Clear();
        activeEnemies.AddRange(room.spawnedEnemies);
        enemiesRemainingInRoom = activeEnemies.Count;

        Debug.Log($"#ROOM Monitoring room with {enemiesRemainingInRoom} enemies");

        Debug.Log(
            $"#ROOM After monitoring - enemiesRemainingInRoom: {enemiesRemainingInRoom}, condition check: {enemiesRemainingInRoom <= 0}"
        );

        // Check if room has no enemies
        if (enemiesRemainingInRoom <= 0)
        // if (enemiesCleared && roomsSpawned)
        {
            Debug.Log(
                $"#ROOM No enemies in room - marking as cleared {enemiesCleared && roomsSpawned}"
            );
            enemiesCleared = true;
            CheckRoomCompletion();
        }
    }

    private void CheckRoomCompletion()
    {
        Debug.Log(
            $"#ROOM CheckRoomCompletion conditions: enemiesCleared={enemiesCleared}, roomsSpawned={roomsSpawned}, !roomCompleted={!roomCompleted}"
        );
        if (enemiesCleared && roomsSpawned && !roomCompleted && currentRoom != null)
        {
            Debug.Log("#ROOM Both conditions met - handling room completion");
            HandleRoomCompleted();
        }
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

    public void OnRoomsSpawned()
    {
        Debug.Log("#ROOM Rooms have been spawned - updating room state");
        roomsSpawned = true;
        CheckRoomCompletion();
    }

    // Reset this flag when player enters a new room
    public void OnRoomChanged()
    {
        roomCompletionHandled = false;
    }
}
