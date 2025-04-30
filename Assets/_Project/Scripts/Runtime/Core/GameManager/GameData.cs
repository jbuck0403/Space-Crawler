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

    public List<WeaponType> unlockedWeaponTypes = new List<WeaponType>();
    public List<AmmoType> unlockedAmmoTypes = new List<AmmoType>();

    // Statistics
    [Header("Game Statistics")]
    [NonSerialized]
    public int totalEnemiesKilled;

    [NonSerialized]
    public int totalRoomsCleared;

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

    private const string SAVE_KEY = "GameData";

    /// <summary>
    /// Reset all temporary data for a new run
    /// </summary>
    public void ResetRunData()
    {
        currentRunTalentPoints = 0;
        currentRunEnemiesKilled = 0;
        currentRunRoomsCleared = 0;
    }

    public void StartNewGame()
    {
        DeleteSaveData();
        isNewGame = true;

        ResetRunData();

        totalEnemiesKilled = 0;
        totalRoomsCleared = 0;
        totalTalentPoints = 0;

        runsCompleted = 0;
        allocatedTalents = new List<TalentTreeSaveData>();
        unlockedWeaponTypes = new List<WeaponType>();
        unlockedAmmoTypes = new List<AmmoType>();
    }

    /// <summary>
    /// Save rewards from current run to persistent data
    /// </summary>
    public void SaveRunRewards()
    {
        if (
            GameManager.Instance != null
            && RoomManager.Instance != null
            && RoomManager.Instance.Player != null
        )
        {
            TalentTreeHandler talentTreeHandler =
                RoomManager.Instance.Player.GetComponent<TalentTreeHandler>();
            if (talentTreeHandler != null)
            {
                // sync points data with TalentTreeHandler
                totalTalentPoints = talentTreeHandler.TotalPoints + currentRunTalentPoints;
                currentRunTalentPoints = 0;

                Debug.Log($"^^^POINTS SYNCED FROM TALENT TREE: {totalTalentPoints}");
            }
        }

        SaveGameData();
    }

    public void AddTalentPoint(int numToAdd = 1)
    {
        currentRunTalentPoints += Mathf.Abs(numToAdd);
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

    public void AddUnlockedWeapon(WeaponType weaponType)
    {
        if (unlockedWeaponTypes.Contains(weaponType))
            return;

        unlockedWeaponTypes.Add(weaponType);
    }

    public void AddUnlockedAmmo(AmmoType ammoType)
    {
        if (unlockedAmmoTypes.Contains(ammoType))
            return;

        unlockedAmmoTypes.Add(ammoType);
    }

    public void DeleteSaveData(string filePath = null)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        DeleteSaveDataWebGL();
#else
        DeleteSaveDataDefault(filePath);
#endif
    }

    private void DeleteSaveDataDefault(string filePath = null)
    {
        if (filePath == null)
        {
            filePath = Path.Combine(Application.persistentDataPath, "gamedata.json");
        }

        File.Delete(filePath);
    }

    private void DeleteSaveDataWebGL()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
        Debug.Log("WebGL: Deleted save data from PlayerPrefs");
    }

    public void SaveGameData()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SaveGameDataWebGL();
#else
        SaveGameDataDefault();
#endif
    }

    private void SaveGameDataDefault()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "gamedata.json");

        string json = JsonUtility.ToJson(this, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"Game data saved to: {filePath}");
    }

    private void SaveGameDataWebGL()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        Debug.Log("WebGL: Game data saved to PlayerPrefs");
    }

    public static GameData LoadGameData()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return LoadGameDataWebGL();
#else
        return LoadGameDataDefault();
#endif
    }

    private static GameData LoadGameDataDefault()
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

    private static GameData LoadGameDataWebGL()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            try
            {
                GameData loadedData = JsonUtility.FromJson<GameData>(json);
                Debug.Log("WebGL: Game data loaded from PlayerPrefs");
                return loadedData;
            }
            catch (Exception e)
            {
                Debug.LogError($"WebGL: Error loading game data: {e.Message}");
                return new GameData();
            }
        }
        else
        {
            Debug.Log("WebGL: No saved game data found. Creating new game data.");
            return new GameData();
        }
    }

    /// <summary>
    /// Checks if save data exists without loading the full file
    /// </summary>
    /// <returns>True if save data exists, false otherwise</returns>
    public static bool HasSaveData()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return HasSaveDataWebGL();
#else
        return HasSaveDataDefault();
#endif
    }

    private static bool HasSaveDataDefault()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "gamedata.json");
        return File.Exists(filePath);
    }

    private static bool HasSaveDataWebGL()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public void IncrementRunsCompleted()
    {
        runsCompleted++;

        AddTalentPoint(2);
        SaveRunRewards();
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
