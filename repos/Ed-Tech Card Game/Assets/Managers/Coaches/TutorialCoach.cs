using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCoach : ICoach {


    string coachName = "TutorialCoach";

    public enum Button {
        Stat1Button,
        Stat2Button,
        Stat3Button,
        Stat4Button,
        DropdownMenuButton
    }

    private bool checkedStat1Button = false, checkedStat2Button = false, checkedStat3Button = false, checkedStat4Button = false, checkedMenuDropDown = false;
    

    public void EnableCoach() {
        EventManager.inspectStatElement += InspectStatButtonCheck;
        EventManager.inspectDropdownMenuElement += DropdownMenuButtonCheck;
    }

    public bool SwipeReview(int cardID) {
        if (!checkedStat1Button) {
            Debug.Log("Please press the first stat button at least once.");
            return false;
        } else if (!checkedStat2Button) {
            Debug.Log("Please press the second stat button at least once.");
            return false;
        } else if (!checkedStat3Button) {
            Debug.Log("Please press the third stat button at least once.");
            return false;
        } else if (!checkedStat4Button) {
            Debug.Log("Please press the fourth stat button at least once.");
            return false;
        } else if (!checkedMenuDropDown) {
            Debug.Log("Please press the menu dropdown button at least once.");
            return false;
        }

        else return true;
    }

    public bool Review(int cardID) {
        return true;
    }


    public void StartEvent(Swipe.HoldDirection direction) {

    }

    public void StopEvent(Swipe.HoldDirection direction) {

    }

    public void InspectStatButtonCheck(int button) {
        Debug.Log(button);
        switch ((Button)button) {
            case (Button.Stat1Button):
                checkedStat1Button = true;
                break;
            case (Button.Stat2Button):
                checkedStat2Button = true;
                break;
            case (Button.Stat3Button):
                checkedStat3Button = true;
                break;
            case (Button.Stat4Button):
                checkedStat4Button = true;
                break;
            default:

                break;
        }
    }

    public void DropdownMenuButtonCheck() {
        checkedMenuDropDown = true;
    }

    public string GetCoachName () {
        return coachName;
    }


}
