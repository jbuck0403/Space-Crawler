using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI component for a single talent button
/// </summary>
public class TalentButtonUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private BaseTalent talent;

    [SerializeField]
    private Image talentIcon;

    [SerializeField]
    private Text talentNameText;

    [SerializeField]
    private Text pointsInvestedText;

    [SerializeField]
    private GameObject maxedOutIndicator;

    [SerializeField]
    private GameObject lockedIndicator;

    [Header("Color States")]
    [SerializeField]
    private Color availableColor = Color.white;

    [SerializeField]
    private Color activeColor = Color.green;

    [SerializeField]
    private Color maxedColor = Color.yellow;

    [SerializeField]
    private Color lockedColor = Color.gray;

    private TalentTree talentTree;
    private TalentTreeInputManager inputManager;
    private int tierIndex;

    public BaseTalent Talent => talent;

    public void Initialize(TalentTree tree, TalentTreeInputManager manager, int tier)
    {
        talentTree = tree;
        inputManager = manager;
        tierIndex = tier;

        // Initialize UI elements
        if (talent != null)
        {
            if (talentIcon != null && talent.icon != null)
            {
                talentIcon.sprite = talent.icon;
            }

            if (talentNameText != null)
            {
                talentNameText.text = talent.talentName;
            }
        }

        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (talent == null || talentTree == null)
            return;

        BaseTalent runtimeTalent = talentTree.GetRuntimeTalent(talent);
        if (runtimeTalent == null)
            return;

        // Update points invested text
        if (pointsInvestedText != null)
        {
            pointsInvestedText.text =
                $"{runtimeTalent.pointsDesignated}/{runtimeTalent.maxDesignatedPoints}";
        }

        // Check if talent is locked
        bool isLocked =
            !inputManager.CanActivateTalent(talent, tierIndex)
            && runtimeTalent.pointsDesignated == 0;

        // Check if talent is maxed out
        bool isMaxed = runtimeTalent.pointsDesignated >= runtimeTalent.maxDesignatedPoints;

        // Update visual state
        if (lockedIndicator != null)
        {
            lockedIndicator.SetActive(isLocked);
        }

        if (maxedOutIndicator != null)
        {
            maxedOutIndicator.SetActive(isMaxed);
        }

        // Update icon color
        if (talentIcon != null)
        {
            if (isLocked)
            {
                talentIcon.color = lockedColor;
            }
            else if (isMaxed)
            {
                talentIcon.color = maxedColor;
            }
            else if (runtimeTalent.pointsDesignated > 0)
            {
                talentIcon.color = activeColor;
            }
            else
            {
                talentIcon.color = availableColor;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (talent == null || talentTree == null)
            return;

        // Left click = add point
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (inputManager.CanActivateTalent(talent, tierIndex))
            {
                talentTree.TryUnlockTalent(talent);
                inputManager.UpdateUI();
            }
        }
        // Right click = remove point
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            talentTree.TryRemoveTalent(talent);
            inputManager.UpdateUI();
        }
    }

    private void OnValidate()
    {
        // Try to automatically set up UI elements if not assigned
        if (talentIcon == null)
        {
            talentIcon = GetComponent<Image>();
        }

        if (talentNameText == null)
        {
            talentNameText = GetComponentInChildren<Text>();
        }
    }
}
