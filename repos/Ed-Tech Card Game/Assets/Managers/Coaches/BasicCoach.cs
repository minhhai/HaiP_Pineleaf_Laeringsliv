using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BasicCoach : ICoach {

    string name = "BasicCoach";

    public struct TimeEvent {
        public string startTime;
        public string endTime;
    }

    public struct EventCollection {
        public List<TimeEvent> heldLeftEvents;
        public List<TimeEvent> heldRightEvents;
        public List<TimeEvent> heldDownEvents;
    }

    EventCollection thisEventCollection;
    TimeEvent te;

    public void EnableCoach() {
        NewCard();
    }

    public void NewCard() {
        thisEventCollection = new EventCollection();
        thisEventCollection.heldLeftEvents = new List<TimeEvent>();
        thisEventCollection.heldRightEvents = new List<TimeEvent>();
        thisEventCollection.heldDownEvents = new List<TimeEvent>();
    }

    public bool Review(int cardID) {
        return true;
    }

    public bool SwipeReview(int cardID) {
        if (cardID == 0) {
            if (thisEventCollection.heldLeftEvents.Count > 0 && thisEventCollection.heldRightEvents.Count > 0 && thisEventCollection.heldDownEvents.Count > 0) {
                return true;
            } else return false;

        }

        return true;
    }

    

    public void StartEvent(Swipe.HoldDirection direction) {
        if (direction != Swipe.HoldDirection.Up && direction != Swipe.HoldDirection.None) {
            te = new TimeEvent();
            te.startTime = DateTime.Now.ToString();
        }
        
    }

    public void StopEvent(Swipe.HoldDirection direction) {
        
        if (direction != Swipe.HoldDirection.Up && direction != Swipe.HoldDirection.None) {
            te.endTime = DateTime.Now.ToString();
            switch (direction) {
                case (Swipe.HoldDirection.Left):
                    thisEventCollection.heldLeftEvents.Add(te);
                    break;
                case (Swipe.HoldDirection.Right):
                    thisEventCollection.heldRightEvents.Add(te);
                    break;
                case (Swipe.HoldDirection.Down):
                    thisEventCollection.heldDownEvents.Add(te);
                    break;

            }
        }
        
    }

    public string FinalizeCoachResponse () {
        NewCard();
        return GetEventString();
    }

    public string GetEventString() {
        return JsonUtility.ToJson(thisEventCollection);
    }

    public string GetCoachName() {
        return name;
    }


}
