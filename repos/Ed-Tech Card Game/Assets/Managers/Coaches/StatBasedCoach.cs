using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBasedCoach : ICoach {


    string coachName = "Stat Coach";

    int statID; // The ID of the stat this coach is responsible for:

    List<string> givenStatLowMessages = new List<string>(); // List of possible responses the coach can give if the given stat is too low

    bool givenWarning = false;


    public void EnableCoach() {

    }

    public void InitializeStatCoach(int _statID, List<string> messages) {
        statID = _statID;
        givenStatLowMessages = messages;
    }

    public bool Review(int cardID) {

        //TODO: For debug purposes
        //statID = 4;

        int substatValue = PlayerManager.Instance.GetSubStat(statID);

        if (substatValue < 25) {
            if (substatValue < 10) {
                GameEventManager.Instance.CoachMessageEvent("Substat: " + PlayerManager.Instance.GetMinorName(statID) + " is very low \n Do you want to start a training session?");
                Debug.Log("Substat: " + statID + " is very low.");
                return false;
            } else if (!givenWarning) {
                GameEventManager.Instance.CoachMessageEvent("Substat: " + PlayerManager.Instance.GetMinorName(statID) + " is getting low \n You should keep that in mind");
                Debug.Log("Substat: " + statID + " is low.");
                givenWarning = true;
                return false;
            }
        }

        if (substatValue > 50) {
            givenWarning = false;
        }

        
        return true;
    }

    public void SetStatID (int newStatID) {
        statID = newStatID;
    }

    public bool SwipeReview(int cardID) {
        return true;
    }

    public void StartEvent(Swipe.HoldDirection direction) {

    }

    public void StopEvent(Swipe.HoldDirection direction) {

    }

    public string GetCoachName() {
        return coachName;
    }

    public string GetRandomStatLowMessage() {
        return givenStatLowMessages[Random.Range(0, givenStatLowMessages.Count)];
    }
}
 