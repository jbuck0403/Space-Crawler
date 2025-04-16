using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Simplified UI component for a talent button
/// </summary>
public class TalentButtonUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private BaseTalent talent;

    private Image talentIcon;

    [SerializeField]
    private TextMeshProUGUI pointsSpentText;

    private TalentTreeHandler talentTreeHandler;
    private TalentTreeInputManager talentUIManager;

    public BaseTalent Talent => talent;

    public void Initialize(TalentTreeHandler handler, TalentTreeInputManager manager)
    {
        talentTreeHandler = handler;
        talentUIManager = manager;

        if (talentIcon == null)
        {
            talentIcon = GetComponent<Image>();
        }

        // Initialize icon if available
        if (talent != null && talentIcon != null && talent.icon != null)
        {
            talentIcon.sprite = talent.icon;
        }

        UpdateButton();
    }

    public void UpdateButton()
    {
        if (talent == null || talentTreeHandler == null || pointsSpentText == null)
            return;

        BaseTalent runtimeTalent = talentTreeHandler.GetRuntimeTalent(talent);
        if (runtimeTalent == null)
        {
            Debug.LogWarning($"%%% TalentButtonUI: No runtime talent found for {talent.name}");
            return;
        }

        pointsSpentText.text =
            $"{runtimeTalent.pointsDesignated}/{runtimeTalent.maxDesignatedPoints}";

        Debug.Log(
            $"%%% TalentButtonUI: Updated UI for {talent.name}: {runtimeTalent.pointsDesignated}/{runtimeTalent.maxDesignatedPoints}"
        );
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (talent == null || talentTreeHandler == null)
            return;

        // Left click = add point
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"%%% TalentButtonUI: Left click on {talent.name}");
            bool result = talentUIManager.TryUnlockTalent(talent);
            Debug.Log($"%%% TalentButtonUI: TryUnlockTalent result: {result}");
        }
        // Right click = remove point
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log($"%%% TalentButtonUI: Right click on {talent.name}");
            bool result = talentUIManager.TryRemoveTalent(talent);
            Debug.Log($"%%% TalentButtonUI: TryRemoveTalent result: {result}");
        }
    }
}
