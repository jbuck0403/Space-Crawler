using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the talent tree UI, dynamically generating and updating talent buttons
/// </summary>
public class TalentTreeUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject talentButtonPrefab;

    [SerializeField]
    private Transform startPoint;

    [SerializeField]
    private RectTransform talentTreeContainer;

    [SerializeField]
    private TextMeshProUGUI availablePointsText;

    [Header("Layout Settings")]
    [SerializeField]
    private float tierVerticalSpacing = 120f;

    [SerializeField]
    private float talentHorizontalSpacing = 100f;

    [SerializeField]
    private bool growUpwards = false;

    private TalentTreeHandler talentTreeHandler;
    private List<TalentButtonUI> talentButtons = new List<TalentButtonUI>();
    private bool isInitialized = false;

    /// <summary>
    /// Initialize with reference to player's talent tree
    /// </summary>
    public void Initialize(TalentTreeHandler playerTalentTree)
    {
        if (playerTalentTree == null)
        {
            Debug.LogError("TalentTreeUIManager: Cannot initialize with null talent tree handler");
            return;
        }

        talentTreeHandler = playerTalentTree;

        // Clear any existing buttons
        ClearTalentButtons();

        // Generate UI elements
        GenerateTalentTreeUI();

        // Update UI
        UpdateUI();

        isInitialized = true;
        Debug.Log("TalentTreeUIManager initialized");
    }

    /// <summary>
    /// Generate the talent tree UI based on the TalentTreeSO
    /// </summary>
    private void GenerateTalentTreeUI()
    {
        if (talentTreeHandler == null || talentTreeHandler.TalentTreeSO == null)
            return;

        TalentTreeSO talentTreeSO = talentTreeHandler.TalentTreeSO;
        int tierCount = talentTreeSO.GetTierCount();

        // Calculate starting position
        Vector3 basePosition = startPoint != null ? startPoint.position : Vector3.zero;

        for (int tierIndex = 0; tierIndex < tierCount; tierIndex++)
        {
            List<BaseTalent> talentsInTier = talentTreeSO.GetTalentsInTier(tierIndex);

            // Skip empty tiers
            if (talentsInTier.Count == 0)
                continue;

            // Calculate vertical position for this tier
            float yOffset = growUpwards
                ? tierIndex * tierVerticalSpacing
                : -tierIndex * tierVerticalSpacing;

            // Calculate width of this tier
            float tierWidth = (talentsInTier.Count - 1) * talentHorizontalSpacing;

            // Generate buttons for each talent in this tier
            for (int talentIndex = 0; talentIndex < talentsInTier.Count; talentIndex++)
            {
                // Calculate position (centered horizontally)
                float xOffset =
                    (talentIndex - (talentsInTier.Count - 1) / 2f) * talentHorizontalSpacing;
                Vector3 buttonPosition = new Vector3(
                    basePosition.x + xOffset,
                    basePosition.y + yOffset,
                    basePosition.z
                );

                // Create button
                CreateTalentButton(talentsInTier[talentIndex], buttonPosition);
            }
        }

        Debug.Log($"TalentTreeUIManager: Generated {talentButtons.Count} talent buttons");
    }

    /// <summary>
    /// Create a single talent button
    /// </summary>
    private void CreateTalentButton(BaseTalent talent, Vector3 position)
    {
        if (talent == null || talentButtonPrefab == null)
            return;

        // Instantiate button prefab
        GameObject buttonObj = Instantiate(talentButtonPrefab, talentTreeContainer);
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();

        // Set position
        if (rectTransform != null)
        {
            rectTransform.position = position;
        }

        // Set button name
        buttonObj.name = $"TalentButton_{talent.talentName}";

        // Initialize button
        TalentButtonUI buttonUI = buttonObj.GetComponent<TalentButtonUI>();
        if (buttonUI != null)
        {
            buttonUI.Initialize(talent, talentTreeHandler, this);
            talentButtons.Add(buttonUI);
        }
    }

    /// <summary>
    /// Clear all generated talent buttons
    /// </summary>
    private void ClearTalentButtons()
    {
        foreach (var button in talentButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }

        talentButtons.Clear();
    }

    /// <summary>
    /// Update all UI elements
    /// </summary>
    public void UpdateUI()
    {
        if (!isInitialized)
            return;

        if (talentTreeHandler == null)
            return;

        // Update available points text
        if (availablePointsText != null)
        {
            availablePointsText.text = $"{talentTreeHandler.AvailablePoints}";
            // $"Available Points: {talentTreeHandler.AvailablePoints}/{talentTreeHandler.TotalPoints}";
        }

        // Update all talent buttons
        foreach (var button in talentButtons)
        {
            if (button != null)
            {
                button.UpdateUI();
            }
        }
    }

    /// <summary>
    /// Try to unlock a talent through the TalentTreeHandler
    /// </summary>
    public void TryUnlockTalent(BaseTalent talent)
    {
        if (talentTreeHandler == null || !talentTreeHandler.IsInitialized)
            return;

        bool success = talentTreeHandler.TryUnlockTalent(talent);

        // Update UI regardless of success
        UpdateUI();
    }

    /// <summary>
    /// Try to remove a talent through the TalentTreeHandler
    /// </summary>
    public void TryRemoveTalent(BaseTalent talent)
    {
        if (talentTreeHandler == null || !talentTreeHandler.IsInitialized)
            return;

        bool success = talentTreeHandler.TryRemoveTalent(talent);

        // Update UI regardless of success
        UpdateUI();
    }

    /// <summary>
    /// Reset all talents
    /// </summary>
    public void ResetAllTalents()
    {
        if (talentTreeHandler == null || !talentTreeHandler.IsInitialized)
            return;

        talentTreeHandler.ResetAllTalents();
        UpdateUI();
    }

    /// <summary>
    /// Show the talent tree UI
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        UpdateUI();
    }

    /// <summary>
    /// Hide the talent tree UI
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
