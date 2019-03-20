using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for handling the in game "Passing of time", ie that as you play through the game, time will pass
/// </summary>
public class GameTimeManager : MonoBehaviour {

    public static GameTimeManager Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }




    [SerializeField]
    private int dayCounter = 0;
    [SerializeField]
    private int weekCounter = 0;
    [SerializeField]
    private int monthCounter = 0;

    public enum Season {
        Spring,
        Summer,
        Fall,
        Winter
    }

    public enum TimeOfDay {
        Morning,
        Afternoon,
        Midday,
        Evening,
        Night
    }

    public enum WeekDay {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday
    }

    public Season currentSeason = Season.Summer;
    public TimeOfDay currentTimeOfDay = TimeOfDay.Morning;
    public WeekDay currentWeekDay = WeekDay.Monday;




    public int GetDayCounter() {
        return dayCounter;
    }

    public void IterateDay() {
        dayCounter++;
        UpdateDisplay();
    }

    public void IterateDays(int n) {
        dayCounter += n;
        UpdateDisplay();
    }

    public void IterateWeek() {
        weekCounter++;
    }

    public void IterateWeeks(int n) {
        weekCounter += n;
    }

    public void IterateMonth() {
        monthCounter++;
    }

    public void IterateMonths(int n) {
        monthCounter += n;
    }

    public void ChangeToSeason (Season newSeason) {
        currentSeason = newSeason;
    }

    public void IterateSeason () {
        if (currentSeason == Season.Winter) {
            currentSeason = Season.Spring;
        } else {
            currentSeason = (Season)((int)currentSeason + 1);
        }
    }

    public void ChangeToTimeOfDay (TimeOfDay newTime) {
        currentTimeOfDay = newTime;
    }

    public void IterateTimeOfDay () {
        if (currentTimeOfDay == TimeOfDay.Night) {
            currentTimeOfDay = TimeOfDay.Morning;
        } else {
            currentTimeOfDay = (TimeOfDay)((int)currentTimeOfDay + 1);
        }
    }

    public void ChangeToWeekDay (WeekDay newWeekDay) {
        currentWeekDay = newWeekDay;
    }

    public void IterateWeekDay () {
        if (currentWeekDay == WeekDay.Friday) {
            currentWeekDay = WeekDay.Monday;
        } else {
            currentWeekDay = (WeekDay)((int)currentWeekDay + 1);
        }
    }

    public void UpdateDisplay() {
        GUIManager.Instance.UpdateTimer(1 + dayCounter % 5, 1 + dayCounter / 5);
        SaveDayCounter();
    }


    public void ResetDayCounter() {
        dayCounter = 0;
        SaveDayCounter();
    }

    public void SaveDayCounter() {
        PlayerStateSaveManager.Instance.SaveDayCount(dayCounter);
    }

    public void LoadDayCounter() {
        dayCounter = PlayerStateSaveManager.Instance.LoadDayCount();
    }

}
