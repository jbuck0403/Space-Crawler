using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that holds persistent and temporary game data
/// </summary>
[System.Serializable]
public class GameData
{
    // Persistent data (saved between runs)
    [Header("Persistent Data")]
    public int totalTalentPoints;
    public Dictionary<string, int> allocatedTalents = new Dictionary<string, int>();
    public List<string> unlockedAmmoTypes = new List<string>();

    // Statistics
    [Header("Game Statistics")]
    public int totalEnemiesKilled;
    public int totalRoomsCleared;
    public int runsCompleted;

    // Current run data (reset each run)
    [Header("Current Run Data")]
    public int currentRunTalentPoints;
    public int currentRunEnemiesKilled;
    public int currentRunRoomsCleared;
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
        totalTalentPoints += currentRunTalentPoints;
        totalEnemiesKilled += currentRunEnemiesKilled;
        totalRoomsCleared += currentRunRoomsCleared;
        runsCompleted++;
    }
}
