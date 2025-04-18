using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Manages UI panels and transitions between different UI states
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeManager();
    }
    #endregion

    [Header("Canvas Reference")]
    [SerializeField]
    private Canvas primaryCanvas;

    [Header("UI Panels")]
    [SerializeField]
    private GameObject gameplayUIPanel;

    [SerializeField]
    private GameObject mainMenuPanel;

    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    private GameObject runConclusionPanel;

    [SerializeField]
    private GameObject preRunSetupPanel;

    [SerializeField]
    private PlayerHUD playerHUDComponent;

    // Dictionary to store all panels for easier access
    private Dictionary<PanelType, GameObject> panels = new Dictionary<PanelType, GameObject>();

    // Current active panel
    private PanelType currentPanel = PanelType.None;

    public bool playerHUDInitialized = false;

    // Panel types enum
    public enum PanelType
    {
        None,
        PlayerHUD,
        MainMenu,
        Pause,
        RunConclusion,
        PreRunSetup
    }

    /// <summary>
    /// Initialize the UI Manager
    /// </summary>
    private void InitializeManager()
    {
        if (gameplayUIPanel != null)
            panels.Add(PanelType.PlayerHUD, gameplayUIPanel);
        if (mainMenuPanel != null)
            panels.Add(PanelType.MainMenu, mainMenuPanel);
        if (pausePanel != null)
            panels.Add(PanelType.Pause, pausePanel);
        if (runConclusionPanel != null)
            panels.Add(PanelType.RunConclusion, runConclusionPanel);
        if (preRunSetupPanel != null)
            panels.Add(PanelType.PreRunSetup, preRunSetupPanel);

        // Hide all panels initially
        foreach (var panel in panels.Values)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        // Ensure canvas is active
        if (primaryCanvas != null)
        {
            primaryCanvas.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Primary Canvas not assigned in UIManager!");
        }
    }

    /// <summary>
    /// Show a specific panel and hide all others
    /// </summary>
    /// <param name="panelType">The panel to show</param>
    private void ShowPanel(PanelType panelType)
    {
        // Don't do anything if the panel is already active
        if (currentPanel == panelType)
            return;

        // Hide all panels
        foreach (var panel in panels.Values)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        // Show the requested panel if it exists
        if (panels.TryGetValue(panelType, out GameObject targetPanel) && targetPanel != null)
        {
            targetPanel.SetActive(true);
            currentPanel = panelType;
            Debug.Log($"Showing panel: {panelType}");
        }
        else
        {
            Debug.LogWarning($"Panel {panelType} not found or not assigned!");
            currentPanel = PanelType.None;
        }
    }

    /// <summary>
    /// Get the currently active panel
    /// </summary>
    public static PanelType GetCurrentPanel()
    {
        return Instance != null ? Instance.currentPanel : PanelType.None;
    }

    #region Static Panel Methods
    public static PlayerHUD ShowPlayerHUD()
    {
        if (Instance != null)
        {
            Instance.ShowPanel(PanelType.PlayerHUD);
            if (Instance.playerHUDComponent != null)
            {
                return Instance.playerHUDComponent;
            }
        }

        return null;
    }

    /// <summary>
    /// Show the Main Menu panel
    /// </summary>
    public static void ShowMainMenu()
    {
        if (Instance != null)
            Instance.ShowPanel(PanelType.MainMenu);
    }

    /// <summary>
    /// Show the Pause panel
    /// </summary>
    public static void ShowPause()
    {
        if (Instance != null)
            Instance.ShowPanel(PanelType.Pause);
    }

    /// <summary>
    /// Show the Run Conclusion panel
    /// </summary>
    public static void ShowRunConclusion()
    {
        if (Instance != null)
            Instance.ShowPanel(PanelType.RunConclusion);
    }

    /// <summary>
    /// Show the Pre-Run Setup panel
    /// </summary>
    public static void ShowPreRunSetup()
    {
        if (Instance != null)
            Instance.ShowPanel(PanelType.PreRunSetup);
    }

    /// <summary>
    /// Hide all panels
    /// </summary>
    public static void HideAllPanels()
    {
        if (Instance != null)
            Instance.ShowPanel(PanelType.None);
    }
    #endregion
}
