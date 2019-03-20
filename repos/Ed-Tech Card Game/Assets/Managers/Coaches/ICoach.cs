using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICoach{

    void EnableCoach();

    bool Review(int cardID);

    bool SwipeReview(int cardID);

    void StartEvent(Swipe.HoldDirection direction);

    void StopEvent(Swipe.HoldDirection direction);

    string GetCoachName();
}
