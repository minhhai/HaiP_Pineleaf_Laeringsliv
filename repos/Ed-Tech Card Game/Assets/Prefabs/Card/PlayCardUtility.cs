using System;
using System.Collections.Generic;
using LaeringslivCore;
using UnityEngine;


public static class PlayCardUtility
{
    public static void SetCardValues(this PlayCard playCard, string[] cardValues)
    {
        playCard.CardID = int.Parse(cardValues[0]);                                         // Card ID
        playCard.Title = cardValues[1];                                                     // Card Character
        playCard.ImageName = cardValues[2];                                                 // Image ID
        playCard.Text = cardValues[3];                                                      // Card Text
        string[] leftSwipeValues = GenerateSubArray(cardValues, 6, 22);                     // Choice 1 (Left Swipe)
        string[] rightswipeValues = GenerateSubArray(cardValues, 22 + 6, 22);               // Choice 2 (Right Swipe)
        string[] downSwipeValues = GenerateSubArray(cardValues, (22 * 2) + 6, 22);          // Choice 3 (Down Swipe)
        
        playCard.GenerateCardChoices(leftSwipeValues, rightswipeValues, downSwipeValues);
    }

    public static void InitValues(this PlayCardChoice playCardChoice, string _eventText, string[] eventValues, string _nextCard)
    {
        playCardChoice.Score = new FeatureVector();
        playCardChoice.Text = _eventText;
        
        int nextCard;
        if (int.TryParse(_nextCard, out nextCard))
        {
            playCardChoice.NextCardID = int.Parse(_nextCard);
        }
        else
        {
            playCardChoice.NextCardID = -1;
        }
        

        for (int i = 0; i < 20; i++)
        {
            int scoreValue;
            if (!int.TryParse(eventValues[i], out scoreValue))
            {
                if (string.IsNullOrWhiteSpace(eventValues[i])) scoreValue = 0;
                else Debug.LogWarning("Could not parse the value");
            }
            
            playCardChoice.Score[i] = scoreValue;
        }
    }

    public static PlayCardChoice[] GetSwipeEvents(this PlayCard playCard)
    {
        return new[] { playCard.Choice1, playCard.Choice2, playCard.Choice3 };
    }
    
    public static void GenerateCardChoices(this PlayCard playCard, string[] leftSwipeValues, string[] rightSwipeValues, string[] downSwipeValues)
    {
        playCard.Choice1 = new PlayCardChoice();
        playCard.Choice1.InitValues(leftSwipeValues[0], GenerateSubArray(leftSwipeValues, 1, 20), leftSwipeValues[21]);

        playCard.Choice2 = new PlayCardChoice();
        playCard.Choice2.InitValues(rightSwipeValues[0], GenerateSubArray(rightSwipeValues, 1, 20), rightSwipeValues[21]);

        playCard.Choice3 = new PlayCardChoice();
        if (downSwipeValues[0] != "")
        {
            playCard.Choice3.InitValues(downSwipeValues[0], GenerateSubArray(downSwipeValues, 1, 20), downSwipeValues[21]);
        }
    }

    public static string[] GenerateSubArray(string[] data, int fromIndex, int length)
    {

        string[] newArray = new string[length];
        Array.Copy(data, fromIndex, newArray, 0, length);
        return newArray;
    }

    public static void SetValues(this FeedbackCard feedbackCard, string[] cardData)
    {
        feedbackCard.CardID = int.Parse(cardData[0]);
        feedbackCard.NextCardID = int.Parse(cardData[1]);
        feedbackCard.Text = cardData[2];
    }

    public static PlayCard GeneratePlayCard(string cardData)
    {
        PlayCard newCard = new PlayCard();
        //newCard.Choices = new List<PlayCardChoice>(new PlayCardChoice[3]);
        string[] cardInformation = cardData.Split(',');

        newCard.SetCardValues(cardInformation);

        return newCard;
    }
    
    public static FeedbackCard GenerateFeedbackCard(string feedbackCardData)
    {
        FeedbackCard newFeedbackCard = new FeedbackCard();
        string[] cardInformation = feedbackCardData.Split(',');

        newFeedbackCard.SetValues(cardInformation);

        return newFeedbackCard;
    }
}