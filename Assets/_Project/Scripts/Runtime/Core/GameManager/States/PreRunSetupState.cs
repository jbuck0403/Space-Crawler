using UnityEngine;

/// <summary>
/// State for pre-run setup (talent points and loadout)
/// </summary>
public class PreRunSetupState : GameState
{
    [SerializeField]
    private GameObject talentTreeUIPrefab;

    private GameObject player;
    private TalentTreeHandler playerTalentTree;
    private TalentTreeUIManager talentUI;

    public PreRunSetupState(GameManager manager)
        : base(manager, GameStateType.PreRunSetup) { }

    public override void Enter()
    {
        Debug.Log("Entering Pre-Run Setup State");
        talentUI = GameManager.Instance.TalentTreeUIManager;

        // Find or initialize player
        InitializePlayer();

        // Initialize talent tree
        InitializeTalentTree();

        // Show talent tree UI
        ShowTalentTreeUI();

        LoadTalentTree();

        UIManager.Instance.TalentTreeUIManager.UpdateUI();
    }

    private void LoadTalentTree()
    {
        playerTalentTree.LoadSavedTalents();
    }

    private void InitializePlayer()
    {
        Debug.Log("WEAPONHUD: PreRunSetupState.InitializePlayer called");
        player = RoomManager.Instance.Player;

        if (player == null)
        {
            Debug.LogError("WEAPONHUD: PreRunSetupState - Could not find player!");
            return;
        }

        Debug.Log($"WEAPONHUD: Found player: {player.name}");

        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
        if (weaponHandler != null)
        {
            Debug.Log("WEAPONHUD: Found WeaponHandler on player, initializing HUD");
            UIManager.Instance.InitializeWeaponHUD(weaponHandler);

            Debug.Log(
                $"WEAPONHUD: WeaponHandler has {weaponHandler.WeaponInstances.Count} weapon instances"
            );
            foreach (var weapon in weaponHandler.WeaponInstances)
            {
                Debug.Log($"WEAPONHUD: WeaponHandler has weapon of type {weapon.weaponType}");
            }
        }
        else
        {
            Debug.LogError("WEAPONHUD: Player does not have WeaponHandler component!");
        }
        Debug.Log("WEAPONHUD: InitializePlayer complete");
    }

    private void InitializeTalentTree()
    {
        if (player == null)
            return;

        // Get or add TalentTreeHandler component
        playerTalentTree = player.GetComponent<TalentTreeHandler>();

        if (playerTalentTree == null)
        {
            Debug.LogError("PreRunSetupState: Player does not have a TalentTreeHandler component!");
            return;
        }

        if (!playerTalentTree.IsInitialized)
        {
            playerTalentTree.Initialize();

            Debug.Log("PreRunSetupState: Initialized talent tree and added starting points");
        }
        else
        {
            Debug.Log("PreRunSetupState: Talent tree already initialized");
        }

        talentUI.UpdateUI();
    }

    private void ShowTalentTreeUI()
    {
        if (playerTalentTree == null)
            return;

        talentUI = GameManager.Instance.TalentTreeUIManager;

        talentUI.Initialize(playerTalentTree);
        UIManager.ShowPreRunSetup();

        Debug.Log("PreRunSetupState: Showing talent tree UI");
    }

    public override void UpdateState()
    {
        // Check for start game input
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            OnStartGameClicked();
        }

        // Check for back to main menu input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackToMainMenuClicked();
        }
    }

    public override void Exit(GameStateType stateTypeToEnter)
    {
        Debug.Log("Exiting Pre-Run Setup State");
        GameManager.Instance.GameData.SaveRunRewards();

        HideTalentUI();
        InitPlayerHUD();
    }

    private void HideTalentUI()
    {
        // Hide talent UI
        if (talentUI != null)
        {
            talentUI.Hide();
        }

        // Save talent configuration
        if (playerTalentTree != null && playerTalentTree.IsInitialized)
        {
            // saving TBI
            Debug.Log("PreRunSetupState: Saving talent configuration");
        }
    }

    private void InitPlayerHUD()
    {
        Debug.Log("Exiting Gameplay Initialization State");

        if (RoomManager.Instance != null && player != null)
        {
            UIManager.ShowPlayerHUD(player);
        }
    }

    // UI event handlers
    private void OnStartGameClicked()
    {
        Debug.Log("PreRunSetupState: Starting game");
        gameManager.ChangeState(GameStateType.GameplayRoom);
    }

    private void OnBackToMainMenuClicked()
    {
        Debug.Log("PreRunSetupState: Returning to main menu");
        gameManager.ChangeState(GameStateType.MainMenu);
    }
}
