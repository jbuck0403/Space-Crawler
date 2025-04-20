// using System;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// /// <summary>
// /// Handles user interaction with the talent tree UI
// /// </summary>
// public class TalentTreeInputManager : MonoBehaviour
// {
//     [Header("UI References")]
//     [SerializeField]
//     private TextMeshProUGUI availablePointsText;

//     private TalentTreeHandler talentTreeHandler;

//     private void Awake()
//     {
//         Debug.Log("%%% TalentTreeInputManager: Awake");

//         if (talentTreeHandler == null)
//         {
//             talentTreeHandler = GetComponent<TalentTreeHandler>();
//         }

//         if (talentTreeHandler == null)
//         {
//             Debug.LogError("%%% TalentTreeInputManager: TalentTreeHandler component is missing!");
//             enabled = false;
//             return;
//         }

//         // Initialize all talent buttons
//         Debug.Log("%%% TalentTreeInputManager: Initializing talent buttons");
//         var buttons = FindObjectsOfType<TalentButtonUI>();
//         foreach (var button in buttons)
//         {
//             button.Initialize(talentTreeHandler, this);
//             Debug.Log(
//                 $"%%% TalentTreeInputManager: Initialized button for talent {button.Talent.name}"
//             );
//         }

//         // Initial UI update
//         UpdateUI();
//         Debug.Log("%%% TalentTreeInputManager: Initial UI update completed");
//     }

//     /// <summary>
//     /// Update all UI elements
//     /// </summary>
//     public void UpdateUI()
//     {
//         Debug.Log("%%% TalentTreeInputManager: Updating UI");

//         // Update available points text
//         if (availablePointsText != null)
//         {
//             availablePointsText.text =
//                 $"Available Points: {talentTreeHandler.AvailablePoints}/{talentTreeHandler.TotalPoints}";
//             Debug.Log(
//                 $"%%% TalentTreeInputManager: Updated points text: {talentTreeHandler.AvailablePoints}/{talentTreeHandler.TotalPoints}"
//             );
//         }

//         // Update all talent buttons
//         var buttons = FindObjectsOfType<TalentButtonUI>();
//         foreach (var button in buttons)
//         {
//             button.UpdateButton();
//         }
//     }

//     /// <summary>
//     /// Try to unlock a talent through the TalentTreeHandler
//     /// </summary>
//     public bool TryUnlockTalent(BaseTalent talent)
//     {
//         Debug.Log($"%%% TalentTreeInputManager: Relaying unlock request for {talent.name}");
//         bool result = talentTreeHandler.TryUnlockTalent(talent);
//         UpdateUI();
//         return result;
//     }

//     /// <summary>
//     /// Try to remove a talent through the TalentTreeHandler
//     /// </summary>
//     public bool TryRemoveTalent(BaseTalent talent)
//     {
//         Debug.Log($"%%% TalentTreeInputManager: Relaying remove request for {talent.name}");
//         bool result = talentTreeHandler.TryRemoveTalent(talent);
//         UpdateUI();
//         return result;
//     }

//     /// <summary>
//     /// Reset all talents through the TalentTreeHandler
//     /// </summary>
//     public void ResetAllTalents()
//     {
//         Debug.Log("%%% TalentTreeInputManager: Relaying reset request");
//         talentTreeHandler.ResetAllTalents();
//         UpdateUI();
//     }
// }
