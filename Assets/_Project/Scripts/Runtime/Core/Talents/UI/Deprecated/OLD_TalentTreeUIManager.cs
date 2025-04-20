// using System;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// /// <summary>
// /// Manages the talent tree UI, including dynamic generation of talent buttons
// /// </summary>
// public class TalentTreeUIManager : MonoBehaviour
// {
//     [Header("UI References")]
//     [SerializeField]
//     private TextMeshProUGUI availablePointsText;

//     [SerializeField]
//     private GameObject talentButtonPrefab;

//     [SerializeField]
//     private Transform startPoint;

//     [SerializeField]
//     private RectTransform talentTreeContainer;

//     [Header("Layout Settings")]
//     [SerializeField]
//     private float tierVerticalSpacing = 100f;

//     [SerializeField]
//     private float talentHorizontalSpacing = 100f;

//     [SerializeField]
//     private bool growUpwards = true;

//     private TalentTreeHandler talentTreeHandler;
//     private List<TalentButtonUI> generatedButtons = new List<TalentButtonUI>();

//     private void Awake()
//     {
//         Debug.Log("%%% TalentTreeUIManager: Awake");
//     }

//     /// <summary>
//     /// Initialize the talent tree UI with a reference to a TalentTreeHandler
//     /// </summary>
//     public void Initialize(TalentTreeHandler handler)
//     {
//         talentTreeHandler = handler;

//         if (talentTreeHandler == null)
//         {
//             Debug.LogError("%%% TalentTreeUIManager: No TalentTreeHandler provided!");
//             return;
//         }

//         // Clear any existing talent buttons
//         ClearTalentButtons();

//         // Generate the talent tree UI
//         GenerateTalentTreeUI(talentTreeHandler.TalentTreeSO);

//         // Update UI
//         UpdateUI();
//     }

//     /// <summary>
//     /// Generate talent tree UI from the provided TalentTreeSO
//     /// </summary>
//     private void GenerateTalentTreeUI(TalentTreeSO talentTreeSO)
//     {
//         if (talentTreeSO == null || talentButtonPrefab == null || talentTreeContainer == null)
//         {
//             Debug.LogError(
//                 "%%% TalentTreeUIManager: Missing required references for UI generation"
//             );
//             return;
//         }

//         // Calculate starting Y position based on growth direction
//         int tierCount = talentTreeSO.GetTierCount();
//         float startY = startPoint ? startPoint.position.y : 0f;
//         if (!growUpwards)
//         {
//             startY += (tierCount - 1) * tierVerticalSpacing;
//         }

//         // Generate tier by tier
//         for (int tier = 0; tier < tierCount; tier++)
//         {
//             List<BaseTalent> talentsInTier = talentTreeSO.GetTalentsInTier(tier);

//             // Skip empty tiers
//             if (talentsInTier.Count == 0)
//                 continue;

//             // Calculate tier Y position
//             float tierY = startY + (growUpwards ? tier : -tier) * tierVerticalSpacing;

//             // Calculate horizontal spacing to center the talents in this tier
//             float tierWidth = (talentsInTier.Count - 1) * talentHorizontalSpacing;
//             float startX = (startPoint ? startPoint.position.x : 0f) - tierWidth / 2f;

//             // Generate buttons for each talent in this tier
//             for (int i = 0; i < talentsInTier.Count; i++)
//             {
//                 float xPos = startX + (i * talentHorizontalSpacing);
//                 CreateTalentButton(talentsInTier[i], new Vector2(xPos, tierY));
//             }
//         }

//         Debug.Log($"%%% TalentTreeUIManager: Generated {generatedButtons.Count} talent buttons");
//     }

//     /// <summary>
//     /// Create a talent button at the specified position
//     /// </summary>
//     private void CreateTalentButton(BaseTalent talent, Vector2 position)
//     {
//         if (talent == null || talentButtonPrefab == null)
//             return;

//         // Instantiate a new button from the prefab
//         GameObject buttonObj = Instantiate(talentButtonPrefab, talentTreeContainer);

//         // Position the button
//         RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
//         if (rectTransform)
//         {
//             rectTransform.anchoredPosition = position;
//         }

//         // Initialize the button with talent data
//         TalentButtonUI buttonUI = buttonObj.GetComponent<TalentButtonUI>();
//         if (buttonUI)
//         {
//             buttonUI.SetTalent(talent);
//             buttonUI.Initialize(talentTreeHandler, this);
//             generatedButtons.Add(buttonUI);

//             Debug.Log($"%%% TalentTreeUIManager: Created button for talent {talent.name}");
//         }
//     }

//     /// <summary>
//     /// Clear all generated talent buttons
//     /// </summary>
//     private void ClearTalentButtons()
//     {
//         foreach (var button in generatedButtons)
//         {
//             if (button != null && button.gameObject != null)
//             {
//                 Destroy(button.gameObject);
//             }
//         }

//         generatedButtons.Clear();
//     }

//     /// <summary>
//     /// Update all UI elements
//     /// </summary>
//     public void UpdateUI()
//     {
//         if (talentTreeHandler == null)
//             return;

//         // Update available points text
//         if (availablePointsText != null)
//         {
//             availablePointsText.text =
//                 $"Available Points: {talentTreeHandler.AvailablePoints}/{talentTreeHandler.TotalPoints}";
//         }

//         // Update all talent buttons
//         foreach (var button in generatedButtons)
//         {
//             if (button != null)
//             {
//                 button.UpdateButton();
//             }
//         }
//     }

//     /// <summary>
//     /// Try to unlock a talent through the TalentTreeHandler
//     /// </summary>
//     public bool TryUnlockTalent(BaseTalent talent)
//     {
//         if (talentTreeHandler == null)
//             return false;

//         bool result = talentTreeHandler.TryUnlockTalent(talent);

//         // Update UI after change
//         UpdateUI();

//         return result;
//     }

//     /// <summary>
//     /// Try to remove a talent through the TalentTreeHandler
//     /// </summary>
//     public bool TryRemoveTalent(BaseTalent talent)
//     {
//         if (talentTreeHandler == null)
//             return false;

//         bool result = talentTreeHandler.TryRemoveTalent(talent);

//         // Update UI after change
//         UpdateUI();

//         return result;
//     }

//     /// <summary>
//     /// Reset all talents through the TalentTreeHandler
//     /// </summary>
//     public void ResetAllTalents()
//     {
//         if (talentTreeHandler == null)
//             return;

//         talentTreeHandler.ResetAllTalents();

//         // Update UI after reset
//         UpdateUI();
//     }

//     /// <summary>
//     /// Show the talent tree UI
//     /// </summary>
//     public void Show()
//     {
//         gameObject.SetActive(true);
//         UpdateUI();
//     }

//     /// <summary>
//     /// Hide the talent tree UI
//     /// </summary>
//     public void Hide()
//     {
//         gameObject.SetActive(false);
//     }
// }
