using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// Game Manager singleton that controls the game state machine
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState currentState;

    // dictionary of all available states
    private Dictionary<GameStateType, GameState> states =
        new Dictionary<GameStateType, GameState>();

    // game data that persists between states and runs
    [SerializeField]
    private GameData gameData = new GameData();

    [SerializeField]
    public GameObject roomManagerPrefab;

    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    [SerializeField]
    TalentTreeUIManager talentTreeUIManager;

    [Header("")]
    [SerializeField]
    private int pointsAwardedOnSuccess = 2;

    // Events
    public event Action<GameStateType> OnStateChanged;

    // Public properties
    public GameData GameData => gameData;
    public CinemachineVirtualCamera VirtualCamera => virtualCamera;
    public GameStateType CurrentStateType { get; private set; }
    public GameState CurrentState => currentState;
    public TalentTreeUIManager TalentTreeUIManager => talentTreeUIManager;
    public bool playerDied = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStates();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        playerDied = false;
        ChangeState(GameStateType.MainMenu);
    }

    public static void LoadSavedGameData()
    {
        GameData loadedGameData = GameData.LoadGameData();

        Instance.gameData = loadedGameData;
    }

    public void HandleEnemyDefeated(BaseEnemyController enemy)
    {
        if (currentState.GetGameStateType() == GameStateType.GameplayRoom)
        {
            GameplayRoomState state = (GameplayRoomState)currentState;
            state.HandleEnemyDefeated(enemy);
            RoomManager.Instance.CurrentRoomComponent.RemoveEnemyFromSpawnedList(enemy);
        }
    }

    private void InitializeStates()
    {
        // create all game states
        states.Add(GameStateType.MainMenu, new MainMenuState(this));
        states.Add(GameStateType.GameplayInit, new GameplayInitState(this));
        states.Add(GameStateType.PreRunSetup, new PreRunSetupState(this));
        states.Add(GameStateType.GameplayRoom, new GameplayRoomState(this));
        states.Add(GameStateType.Pause, new PauseState(this));
        states.Add(GameStateType.RunConclusion, new RunConclusionState(this));

        Debug.Log("Game states initialized");
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.HandleInput();
            currentState.UpdateState();
        }
    }

    /// <summary>
    /// Change to a new game state
    /// </summary>
    public void ChangeState(GameStateType newStateType)
    {
        Debug.Log($"Changing state from {CurrentStateType} to {newStateType}");

        // Exit current state
        currentState?.Exit(newStateType);

        // Set and enter new state
        currentState = states[newStateType];
        CurrentStateType = newStateType;
        currentState.Enter();

        // Notify listeners
        OnStateChanged?.Invoke(newStateType);
    }

    public void SetBossDefeated()
    {
        if (currentState.GetGameStateType() == GameStateType.GameplayRoom)
        {
            GameplayRoomState gameState = (GameplayRoomState)currentState;
            if (gameState != null)
            {
                gameState.SetBossDefeated();
            }
        }
    }

    public void IncreaseTalentPoints(int numPoints = 0)
    {
        int pointsToAdd = numPoints;
        if (numPoints <= 0)
        {
            pointsToAdd = pointsAwardedOnSuccess;
        }
        print($"^^^ADDING TALENT POINTS {pointsToAdd}");
        gameData.AddTalentPoint(pointsToAdd);
    }

    /// <summary>
    /// Start a new game run
    /// </summary>
    public void StartNewGame()
    {
        gameData.StartNewGame();

        ChangeState(GameStateType.GameplayInit);
    }

    /// <summary>
    /// Continue a saved game
    /// </summary>
    public void ContinueGame()
    {
        LoadSavedGameData();
        gameData.isNewGame = false;
        ChangeState(GameStateType.GameplayInit);
    }

    public void FinishPreRunSetup()
    {
        ChangeState(GameStateType.GameplayRoom);
    }

    /// <summary>
    /// Complete the current run - success means the boss was defeated
    /// </summary>
    public void CompleteRun(bool success)
    {
        Debug.LogWarning(
            $"!!@ CompleteRun called with success={success}, current state: {CurrentStateType}"
        );

        if (success)
        {
            gameData.IncrementRunsCompleted();
            // gameData.SaveRunRewards();
        }
        else
        {
            playerDied = true;
        }

        ChangeState(GameStateType.RunConclusion);

        Debug.LogWarning(
            $"!!@ After state change in CompleteRun - current state: {CurrentStateType}"
        );
    }

    public void GoToMainMenu()
    {
        ChangeState(GameStateType.MainMenu);
    }

    /// <summary>
    /// Toggle pause state
    /// </summary>
    public void TogglePause()
    {
        if (CurrentStateType == GameStateType.GameplayRoom)
        {
            ChangeState(GameStateType.Pause);
            UIManager.ShowPause();
        }
        else if (CurrentStateType == GameStateType.Pause)
        {
            ChangeState(GameStateType.GameplayRoom);
            UIManager.ShowPlayerHUD(RoomManager.Instance.Player);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
