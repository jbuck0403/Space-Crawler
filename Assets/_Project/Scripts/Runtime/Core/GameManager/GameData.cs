using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Class that holds persistent and temporary game data
/// </summary>
[Serializable]
public class GameData
{
    // Persistent data (saved between runs)
    [Header("Persistent Data")]
    public int totalTalentPoints;

    public List<TalentTreeSaveData> allocatedTalents = new List<TalentTreeSaveData>();

    // public List<string> unlockedAmmoTypes = new List<string>();

    // Statistics
    [Header("Game Statistics")]
    [NonSerialized]
    public int totalEnemiesKilled;

    [NonSerialized]
    public int totalRoomsCleared;

    [NonSerialized]
    public int runsCompleted;

    // Current run data (reset each run)
    [Header("Current Run Data")]
    [NonSerialized]
    public int currentRunTalentPoints;

    [NonSerialized]
    public int currentRunEnemiesKilled;

    [NonSerialized]
    public int currentRunRoomsCleared;

    [NonSerialized]
    public bool isNewGame = true;

    /// <summary>
    /// Reset all temporary data for a new run
    /// </summary>
    public void ResetRunData()
    {
        currentRunTalentPoints = 0;
        currentRunEnemiesKilled = 0;
        currentRunRoomsCleared = 0;
    }

    /// <summary>
    /// Save rewards from current run to persistent data
    /// </summary>
    public void SaveRunRewards()
    {
        // totalTalentPoints += currentRunTalentPoints;
        // totalEnemiesKilled += currentRunEnemiesKilled;
        // totalRoomsCleared += currentRunRoomsCleared;
        // runsCompleted++;

        SaveGameData();
    }

    public void ModifyAllocatedTalents(BaseTalent talent, int pointsAllocated)
    {
        string talentName = talent.ToString();
        foreach (TalentTreeSaveData talentTreeSaveData in allocatedTalents)
        {
            if (talentTreeSaveData.talent == talentName)
            {
                if (pointsAllocated <= 0)
                {
                    allocatedTalents.Remove(talentTreeSaveData);
                }
                else
                {
                    talentTreeSaveData.pointsAllocated = pointsAllocated;
                }
                return;
            }
        }

        TalentTreeSaveData talentSaveData = new TalentTreeSaveData(talent, pointsAllocated);
        allocatedTalents.Add(talentSaveData);
    }

    public void SaveGameData()
    {
        string json = JsonUtility.ToJson(this, true);
        string filePath = Path.Combine(Application.persistentDataPath, "gamedata.json");
        File.WriteAllText(filePath, json);
        Debug.Log($"Game data saved to: {filePath}");
    }

    public static GameData LoadGameData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "gamedata.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            try
            {
                GameData loadedData = JsonUtility.FromJson<GameData>(json);
                Debug.Log($"Game data loaded from: {filePath}");
                return loadedData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading game data: {e.Message}");
                return new GameData();
            }
        }
        else
        {
            Debug.Log("No saved game data found. Creating new game data.");
            return new GameData();
        }
    }
}

[Serializable]
public class TalentTreeSaveData
{
    public string talent;
    public int pointsAllocated;

    public TalentTreeSaveData(BaseTalent baseTalent, int pointsAllocated)
    {
        talent = baseTalent.ToString();
        this.pointsAllocated = pointsAllocated;
    }
}
