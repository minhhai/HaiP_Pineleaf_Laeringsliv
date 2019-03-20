using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using LaeringslivCore;

/// <summary>
///     Class for handling local generation of cards
/// </summary>
public class GenerateCards : ICardDeckCollectionProvider
{
    const int minorStats = 20;
    const int numOfCards = 39; //TODO: Temp solution
    const int numOfTipsCards = 6;

   
    

    public CardDeckCollection GetCardDeckCollection(int gameID)
    {
        #region Play Cards

        List<PlayCard> playCards = new List<PlayCard>();

        // Generate playing cards
        TextAsset input = Resources.Load<TextAsset>("PlayCards");
        string[] data = input.text.Split(new char[] { '\n' }); // Split each line into its own segment

        for (int i = 1; i < numOfCards + 1; i++)
        {
            // Ignore index 0, as it is the identifier line
            playCards.Add(PlayCardUtility.GeneratePlayCard(data[i]));
        }

        

        #endregion

        #region Feedback Cards

        // Generate Tips cards
        TextAsset testInput = Resources.Load<TextAsset>("TipsCards");
        string[] tipsData = testInput.text.Split(new char[] { '\n' });
        
        List<FeedbackCard> feedbackCards = new List<FeedbackCard>();

        for (int i = 1; i < numOfTipsCards + 1; i++)
        {
            // Ignore index 0, as it is the identifier line
            feedbackCards.Add(PlayCardUtility.GenerateFeedbackCard(tipsData[i]));
        }

        #endregion

        CardDeck cardDeck = new CardDeck
        {
            CardDeckID = 0,
            Name = "Default",
            Description = "Local card deck",
            PlayCards = playCards,
            FeedbackCards = feedbackCards
        };

        List<CardDeck> cardDecks = new List<CardDeck> { cardDeck };

        CardDeckCollection cardDeckCollection = new CardDeckCollection
        {
            GameID = gameID,
            PlayCardDecks = cardDecks
        };

        return cardDeckCollection;
    }
}