using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI component for a talent button in the talent tree
/// </summary>
public class TalentButtonUI
    : MonoBehaviour,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler
{
    [SerializeField]
    private Image iconImage;

    [SerializeField]
    private TextMeshProUGUI pointsText;

    [SerializeField]
    private Image backgroundImage;

    [SerializeField]
    private GameObject tooltip;

    [SerializeField]
    private TextMeshProUGUI tooltipDescription;

    [SerializeField]
    private TextMeshProUGUI tooltipTalentName;

    [SerializeField]
    private Color lockedColor = Color.gray;

    [SerializeField]
    private Color availableColor = Color.white;

    [SerializeField]
    private Color maxedColor = Color.yellow;

    private BaseTalent talent;
    private TalentTreeHandler talentTreeHandler;
    private TalentTreeUIManager uiManager;

    /// <summary>
    /// Initialize the button with required references
    /// </summary>
    public void Initialize(
        BaseTalent talent,
        TalentTreeHandler handler,
        TalentTreeUIManager manager
    )
    {
        this.talent = talent;
        talentTreeHandler = handler;
        uiManager = manager;

        if (talent != null && talent.icon != null)
        {
            iconImage.sprite = talent.icon;
        }

        InitTooltip();
        UpdateUI();
    }

    private void InitTooltip()
    {
        tooltipTalentName.text = talent.talentName;
        tooltipDescription.text = $"{talent.description}";

        // Resize tooltip to fit the text
        ResizeTooltipToFitText();

        tooltip.SetActive(false);
    }

    private void ResizeTooltipToFitText()
    {
        if (tooltip == null || tooltipDescription == null || tooltipTalentName == null)
            return;

        // Get the RectTransform
        RectTransform tooltipRect = tooltip.GetComponent<RectTransform>();

        // Force mesh updates to ensure accurate preferred values
        tooltipTalentName.ForceMeshUpdate();
        tooltipDescription.ForceMeshUpdate();

        // Get preferred sizes for both text components
        Vector2 nameSize = tooltipTalentName.GetPreferredValues();
        Vector2 descSize = tooltipDescription.GetPreferredValues();

        // Determine width (use the wider of the two)
        float width = Mathf.Max(nameSize.x, descSize.x);

        // Vertical margin between name and description
        float verticalMargin = 10f; // Adjust as needed

        // Calculate total height (name + margin + description)
        float height = nameSize.y + verticalMargin + descSize.y;

        // Add padding to the overall container
        float paddingX = 20f; // Horizontal padding
        float paddingY = 20f; // Vertical padding

        // Set tooltip size
        tooltipRect.sizeDelta = new Vector2(width + paddingX, height + paddingY);
    }

    /// <summary>
    /// Update the button's visual state
    /// </summary>
    public void UpdateUI()
    {
        if (talent == null || talentTreeHandler == null)
            return;

        // Get runtime instance
        BaseTalent runtimeTalent = talentTreeHandler.GetRuntimeTalent(talent);
        if (runtimeTalent == null)
            return;

        // Update points text
        if (pointsText != null)
        {
            pointsText.text =
                $"{runtimeTalent.pointsDesignated}/{runtimeTalent.maxDesignatedPoints}";
            Debug.Log(
                $"^^^ UI UPDATE: {runtimeTalent.talentName} points={runtimeTalent.pointsDesignated}/{runtimeTalent.maxDesignatedPoints}"
            );
        }

        // Update visual state
        if (backgroundImage != null)
        {
            if (runtimeTalent.pointsDesignated <= 0)
            {
                // Check if tier is unlocked
                int tierIndex = talentTreeHandler.TalentTreeSO.GetTierIndex(talent);
                bool tierUnlocked = talentTreeHandler.IsTierUnlocked(tierIndex);

                // Check if prerequisites are met
                bool prereqsMet = runtimeTalent.ArePrerequisitesMet(
                    talentTreeHandler.GetActiveTalents()
                );

                // Talent is locked if tier is locked or prerequisites aren't met
                backgroundImage.color = (tierUnlocked && prereqsMet) ? availableColor : lockedColor;
            }
            else if (runtimeTalent.pointsDesignated >= runtimeTalent.maxDesignatedPoints)
            {
                // Talent is maxed out
                backgroundImage.color = maxedColor;
            }
            else
            {
                // Talent has points but not maxed
                backgroundImage.color = availableColor;
            }
        }
    }

    /// <summary>
    /// Handle pointer clicks on the button
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (talent == null || uiManager == null)
            return;

        // Left click = add point
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            uiManager.TryUnlockTalent(talent);
        }
        // Right click = remove point
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            uiManager.TryRemoveTalent(talent);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ResizeTooltipToFitText();
        tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }
}
