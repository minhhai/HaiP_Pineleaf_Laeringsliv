using System.Collections;
using System.Collections.Generic;
using System;
using LaeringslivCore;
using UnityEngine;
using System.Globalization;


/// <summary>
/// Class used for handling in-game events (card change, coach interruption, Game end, etc). 
/// Not to be confused with the EventManager class, which is primarily used to organize subscription based events
/// </summary>
public class GameEventManager : MonoBehaviour {

    public static GameEventManager Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }



    private int StoredDirection;

    //TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    /// <summary>
    /// Event for when you transition between two cards. Can be interrupted by events etc
    /// </summary>
    public void CardTransition (int lastCardIndex, int direction) {

        PlayCard currentActiveCard = CardManager.Instance.GetCurrentActiveCard();

        CheckForEarnedBadge(direction); // Check if the player has earned any badges

        // Check for active tips card TODO: Rough, could optimize
        List<FeedbackCard> tipsCardList = CardManager.FeedbackCardList;
        for (int i = 0; i < tipsCardList.Count; i++) {
            if (tipsCardList[i].NextCardID == currentActiveCard.CardID) {
                
                TriggerHintCard(i);
                StoredDirection = direction;
                return;
            }
        }

        // Coach methods
        //if (!GameManager.Instance.CoachReview()) { // Checks with standard coaches if they have something to say, interrupts transmission and hands control over to the relevant coach if so
        //    return;
        //}
        bool followingCard = CheckForNextCard(direction); // Check if the choice made had a specified Next Card



        // Check if there is a card following the current choice, if not, go to next card
        if (!followingCard) {
            DrawNextCard();
        }

    }

    /// <summary>
    /// Draw the next card
    /// </summary>
    public void DrawNextCard() {
        GameTimeManager.Instance.IterateDay();
        GameManager.Instance.NextCard();
    }

    /// <summary>
    /// Draw a specific card and inject it into the current position in the active deck.
    /// </summary>
    /// <param name="cardID"></param>
    public void DrawSpecificCard(int cardID) {
        GameManager.Instance.DrawSpecificCard(cardID);
        PlayCard currentActiveCard = CardManager.Instance.GetCurrentActiveCard();
    }

    /// <summary>
    /// Change deck based on the given ID
    /// </summary>
    /// <param name="deckID"></param>
    public void ChangeToDeck(int deckID) {

    }

    /// <summary>
    /// Force insert an event into the current game
    /// </summary>
    public void InjectEvent() {

    }

    /// <summary>
    /// Replace a NextCardEvent with a transitionary event (Think inserting a help card between two specific cards)
    /// </summary>
    public void InjectTransistoryEvent() {

    }

    /// <summary>
    /// Continue to the next card in queue after an injection event
    /// </summary>
    public void ContinueDrawNextCard() {

        bool nextCard = CheckForNextCard(StoredDirection); // Check if the choice made had a specified Next Card

        if (!nextCard) {
            DrawNextCard();
        }
    }

    /// <summary>
    /// Check if the last choice made specified a specific card ID to jump to
    /// </summary>
    public bool CheckForNextCard(int direction) {
        PlayCardChoice playCardChoice =
           CardManager.Instance.GetCurrentActiveCard().GetSwipeEvents()[randomSwipeDir[(int)direction]];
        print("The current card is: " + CardManager.Instance.GetCurrentActiveCard().CardID);
        if (playCardChoice.NextCardID > 0) {
            print("The Followup card to the choice made is " + playCardChoice.NextCardID);
            //List<PlayCard> playCardList = CardManager.Instance.GetPlayCardList();
            List<PlayCard> substoryCardList = CardManager.Instance.GetSubstoryCardList();
            if (substoryCardList != null) {
                
                for (int i = 0; i < substoryCardList.Count; i++) {
                    if (substoryCardList[i].CardID == playCardChoice.NextCardID) {
                        GameManager.Instance.DrawSpecificSubstoryCard(substoryCardList[i].CardID);
                        return true;
                    }
                }
            }
            // TODO: Checking normal card stack as well for now
            List<PlayCard> playCardList = CardManager.Instance.GetPlayCardList();
            for (int i = 0; i < playCardList.Count; i++) {
                if (playCardList[i].CardID == playCardChoice.NextCardID) {
                    GameManager.Instance.DrawSpecificCard(playCardList[i].CardID);
                    return true;
                } 
            }
            
            print("Couldn't find card id " + playCardChoice.NextCardID + " in the substory card list.");
        }
        return false;
    }

    #region Badge Events

    /// <summary>
    /// Check if you have earned a new badge, if so, inject a badge event card, show video, etc
    /// </summary>
    public void CheckForEarnedBadge(int direction) {

        print("Checking for badge");
        // Check card specific requirements
        PlayCardChoice playCardChoice =
           CardManager.Instance.GetCard(GameManager.Instance.GetCurrentCardIndex()).GetSwipeEvents()[randomSwipeDir[(int)direction]];
        int badgeCardID = playCardChoice.BadgeId;
        if (CardManager.Instance.GetBadgeCardList() != null && badgeCardID > 0) {
            foreach (BadgeCard badgeCard in CardManager.Instance.GetBadgeCardList()) {
                print("This badge ID: " + badgeCardID + " this card's ID: " + badgeCard.BadgeCardId);
                // Check draw specific requirements
                if ( badgeCardID == badgeCard.BadgeCardId) {
                    print("Hi, found a match, BadgeID: " + badgeCardID);
                    AddBadge(badgeCardID);
                }
            }
        }
        
    }

    /// <summary>
    /// Earned a badge, add it to the list, also trigger the GUI popup window
    /// </summary>
    /// <param name="badgeID"></param>
    public void AddBadge(int badgeID) {
        BadgeManager.Instance.AddBadge(badgeID);
        List<BadgeCard> badgecardList = CardManager.Instance.GetBadgeCardList();
        for (int j = 0; j < badgecardList.Count; j++) {
            if (badgeID == badgecardList[j].BadgeCardId) {
                GUIManager.Instance.TriggerBadgePopup(badgecardList[j]);
            }
        }
    }

    #endregion

    /// <summary>
    /// Trigger some sort of event from a coach, could possibly be used to redirect to a Coach training session
    /// </summary>
    public void CoachEvent() {

    }

    /// <summary>
    /// Simple Coach event that only displays a message, just for testing right now
    /// </summary>
    public void CoachMessageEvent(string message) {
        GUIManager.Instance.ResetSwipeBackgrounds();
        GUIManager.Instance.DisplayCoachCard(message);
    }

    /// <summary>
    /// Scrambles the choice directions, but keeps track of the mapping so we can remap the taken choice later
    /// </summary>
    #region Randomize Swipe direction

    public int[] randomSwipeDir = new int[]{ 0, 1, 2 };

    /// <summary>
    /// Generate a new random array of directions
    /// </summary>
    /// <returns></returns>
    public int[] GenerateRandomSwipeDirections () {
        randomSwipeDir = new int[] { 0, 1, 2 };
        for (int i = 0; i < 3; i++) {
            int r = UnityEngine.Random.Range(0, i);
            int tmp = randomSwipeDir[i];
            randomSwipeDir[i] = randomSwipeDir[r];
            randomSwipeDir[r] = tmp;
        }
        return randomSwipeDir;
    }

    #endregion

    /// <summary>
    /// Handle swipe event for card choices
    /// </summary>
    /// <param name="direction"></param>
    public void SwipeEvent(Swipe.HoldDirection direction) {
        PlayCardChoice playCardChoice =
            CardManager.Instance.GetCard(GameManager.Instance.GetCurrentCardIndex()).GetSwipeEvents()[randomSwipeDir[(int)direction]];

        PlayerManager.Instance.ChangeStats(playCardChoice.Score);
        GUIManager.Instance.RunSwipeAnimation((int)direction);

        PlayerManager.Instance.SavePlayerStats();
    }


    /// <summary>
    /// Log swipe event
    /// </summary>
    /// <param name="direction"></param>
    public void HandleChoiceEventLogging(int direction) {
        // Report to server that the player has made a choice

        DateTime dt = DateTime.Now;
        EventLog eventLog = new EventLog {
            Time = dt.ToString("mm/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture),


            // TODO: Change when player profiles exist 
            PlayerID = "0",

            Event = EventLog.EventType.PlayerChoice,

            CardID = GameManager.Instance.GetCurrentCardID(),

            Choice = randomSwipeDir[(int)direction]

        };


        Server.Instance.BundleEvent(eventLog);
        //Server.LogEvent(eventLog);
    }

    /// <summary>
    /// Saves the players state variables locally
    /// </summary>
    public void SaveStatsEvent() {
        PlayerManager.Instance.SavePlayerStats();
    }

    #region Special Case events

    /// <summary>
    /// Display a hint card
    /// </summary>
    /// <param name="feedbackCardID"></param>
    public void TriggerHintCard(int feedbackCardID) {
        GUIManager.Instance.ResetSwipeBackgrounds();
        GUIManager.Instance.DisplayTipsCard(CardManager.Instance.GetTipsCard(feedbackCardID));
    }


    public void CloseHintCard() {
        ContinueDrawNextCard();
        GUIManager.Instance.HideTipsCard();
    }

    public void CloseCoachCard() {
        ContinueDrawNextCard();
        GUIManager.Instance.HideCoachCard();
    }


    public void TriggerBadgeCard(int badgeCardID) {

    }

    /// <summary>
    /// Special case for when a card choice would lead into another card
    /// </summary>
    /// <param name="cardID"></param>
    public void FollowCardEvent(string cardID) {

    }


    /// <summary>
    /// Detect the start of a hold action for a given direction
    /// </summary>
    public void CardHoldStartEvent(Swipe.HoldDirection holdDirection) {

    }

    /// <summary>
    /// Detect the end of a hold action for a given direction, either through reset, or through a swipe action being performed
    /// </summary>
    public void CardHoldStopEvent(Swipe.HoldDirection holdDirection) {

    }


    #endregion

}
