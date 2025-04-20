using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI component for a talent button in the talent tree
/// </summary>
public class TalentButtonUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image iconImage;

    [SerializeField]
    private TextMeshProUGUI pointsText;

    [SerializeField]
    private Image backgroundImage;

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

        UpdateUI();
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
}
