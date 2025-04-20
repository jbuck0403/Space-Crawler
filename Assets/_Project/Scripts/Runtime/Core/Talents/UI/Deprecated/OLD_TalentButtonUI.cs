// using TMPro;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;

// /// <summary>
// /// UI component for a talent button in the talent tree
// /// </summary>
// public class TalentButtonUI
//     : MonoBehaviour,
//         IPointerClickHandler,
//         IPointerEnterHandler,
//         IPointerExitHandler
// {
//     [SerializeField]
//     private BaseTalent talent;

//     [SerializeField]
//     private Image talentIcon;

//     [SerializeField]
//     private TextMeshProUGUI pointsSpentText;

//     [SerializeField]
//     private Color lockedColor = Color.gray;

//     [SerializeField]
//     private Color unlockedColor = Color.white;

//     [SerializeField]
//     private Color maxedColor = Color.yellow;

//     private TalentTreeHandler talentTreeHandler;
//     private TalentTreeUIManager uiManager;

//     public BaseTalent Talent => talent;

//     /// <summary>
//     /// Initialize the talent button with necessary references
//     /// </summary>
//     public void Initialize(TalentTreeHandler handler, TalentTreeUIManager manager)
//     {
//         talentTreeHandler = handler;
//         uiManager = manager;

//         if (talentIcon == null)
//         {
//             talentIcon = GetComponent<Image>();
//         }

//         // Initialize icon if available
//         if (talent != null && talentIcon != null && talent.icon != null)
//         {
//             talentIcon.sprite = talent.icon;
//         }

//         UpdateButton();
//     }

//     /// <summary>
//     /// Set the talent for this button
//     /// </summary>
//     public void SetTalent(BaseTalent newTalent)
//     {
//         talent = newTalent;

//         // Update icon if available
//         if (talent != null && talentIcon != null && talent.icon != null)
//         {
//             talentIcon.sprite = talent.icon;
//         }
//     }

//     /// <summary>
//     /// Update the button visuals to reflect current talent state
//     /// </summary>
//     public void UpdateButton()
//     {
//         if (talent == null || talentTreeHandler == null || pointsSpentText == null)
//             return;

//         BaseTalent runtimeTalent = talentTreeHandler.GetRuntimeTalent(talent);
//         if (runtimeTalent == null)
//         {
//             Debug.LogWarning($"%%% TalentButtonUI: No runtime talent found for {talent.name}");
//             return;
//         }

//         // Update points text
//         pointsSpentText.text =
//             $"{runtimeTalent.pointsDesignated}/{runtimeTalent.maxDesignatedPoints}";

//         // Update button color based on state
//         if (talentIcon != null)
//         {
//             if (runtimeTalent.pointsDesignated <= 0)
//             {
//                 // Check if tier is unlocked
//                 int tierIndex = talentTreeHandler.TalentTreeSO.GetTierIndex(talent);
//                 bool tierUnlocked = talentTreeHandler.IsTierUnlocked(tierIndex);

//                 // Check if prerequisites are met
//                 bool prereqsMet = runtimeTalent.ArePrerequisitesMet(
//                     talentTreeHandler.GetActiveTalents()
//                 );

//                 talentIcon.color = (tierUnlocked && prereqsMet) ? unlockedColor : lockedColor;
//             }
//             else if (runtimeTalent.pointsDesignated >= runtimeTalent.maxDesignatedPoints)
//             {
//                 talentIcon.color = maxedColor;
//             }
//             else
//             {
//                 talentIcon.color = unlockedColor;
//             }
//         }
//     }

//     /// <summary>
//     /// Handle clicks on the talent button
//     /// </summary>
//     public void OnPointerClick(PointerEventData eventData)
//     {
//         if (talent == null || uiManager == null)
//             return;

//         // Left click = add point
//         if (eventData.button == PointerEventData.InputButton.Left)
//         {
//             uiManager.TryUnlockTalent(talent);
//         }
//         // Right click = remove point
//         else if (eventData.button == PointerEventData.InputButton.Right)
//         {
//             uiManager.TryRemoveTalent(talent);
//         }
//     }

//     public void OnPointerEnter(PointerEventData eventData)
//     {
//         // TODO: Display tooltip with talent information
//     }

//     public void OnPointerExit(PointerEventData eventData)
//     {
//         // TODO: Hide tooltip
//     }
// }
