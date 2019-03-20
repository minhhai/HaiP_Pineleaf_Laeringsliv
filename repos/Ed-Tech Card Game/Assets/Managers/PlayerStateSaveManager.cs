using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class handling saving the various player variables between game sessions
/// </summary>
public class PlayerStateSaveManager : MonoBehaviour {

    public static PlayerStateSaveManager Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void SaveState(int currentCardID, int dayCount, int[] minorStats) {
        PlayerPrefs.SetInt("CurrentCard", currentCardID);
        PlayerPrefs.SetInt("DayCount", dayCount);
        for (int i = 0; i < 16; i++) {
            PlayerPrefs.SetInt("MinorStat" + i, minorStats[i]);
        }
    }

    public void SaveCurrentCard(int currentCardID) {
        PlayerPrefs.SetInt("CurrentCard", currentCardID);
    }

    public int LoadCurrentCard() {
        return (PlayerPrefs.GetInt("CurrentCard"));
    }

    public void SaveDayCount(int dayCount) {
        PlayerPrefs.SetInt("DayCount", dayCount);
    }

    public int LoadDayCount() {
        return (PlayerPrefs.GetInt("DayCount"));
    }

    public void SaveMinorStats(int[] minorStats) {
        for (int i = 0; i < 20; i++) {
            PlayerPrefs.SetInt("MinorStat" + i, minorStats[i]);
        }
    }

    public int LoadMinorStat(int i) {
        return PlayerPrefs.GetInt("MinorStat" + i);

    }

    public void SaveThemeID(int i) {
        PlayerPrefs.SetInt("ThemeID", i);
    }

    public int LoadThemeID() {
        return PlayerPrefs.GetInt("ThemeID");
    }

    
    
    public void SaveDeviceID(int i) {
        PlayerPrefs.SetInt("DeviceID", i);
    }

    public int GetDeviceID() {
        return PlayerPrefs.GetInt("DeviceID");
    }

    // Dumb but fast way of generating an user ID that is essentially unique
    public void GenerateRandomDeviceID() {
        int randomVal = Random.Range(0, int.MaxValue);
        PlayerPrefs.SetInt("DeviceID", randomVal);
    }

}
