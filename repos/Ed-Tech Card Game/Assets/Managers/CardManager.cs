using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using LaeringslivCore;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

/// <summary>
///     Class for keeping track of all the available cards in the game
/// </summary>
public class CardManager : MonoBehaviour
{

    public static CardManager Instance;

    public GameObject LoadingScreen;

    protected  static int targetStackID = 0;

    public void SetTargetStack (int id) {
        targetStackID = id;
    }


    private ICardDeckCollectionProvider _cardDeckCollectionProvider;

    private CardDeckCollection _cardDeckCollection = new CardDeckCollection();
    //private CardDeckCollection[] _cardDeckCollection = new CardDeckCollection[3];

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    //IEnumerator Start()
    //{
    //    string downloadUrl = "https://laeringsliv.azurewebsites.net/CardDeckCollections/DownloadCardDeckCollection?id=1"; // TODO: Using ID 2 for Pro version
    //    UnityWebRequest www = UnityWebRequest.Get(downloadUrl);

    //    yield return www.SendWebRequest();

    //    byte[] json = www.downloadHandler.data;

    //    try {
    //        string outp = Encoding.UTF8.GetString(json);
    //        _cardDeckCollection = JsonUtility.FromJson<CardDeckCollection>(outp);


    //        if (_cardDeckCollection == null) {
    //            throw new Exception("Empty Card Deck");
    //        } else {
    //            SplashScreenLogic.Instance.DeckReady();
    //            CollectionSaveManager.Instance.SaveCollection(1, Encoding.UTF8.GetString(json));
    //        }


    //        // If a connection to the server cannot be established, use the on-device backup files to generate decks.
    //    } catch (Exception e) {
    //        print(e);
    //        //_cardDeckCollectionProvider = new GenerateCards();
    //        //_cardDeckCollection[i-1] = _cardDeckCollectionProvider.GetCardDeckCollection(1);
    //        _cardDeckCollection = JsonUtility.FromJson<CardDeckCollection>(CollectionSaveManager.Instance.LoadCollection(1));
    //        Debug.LogError("Could not get data from server. Using local backup");

    //    }

    //    //TODO Making a dummy to demonstrate:
    //    GUIManager.Instance.SetcardDeckVersionNumber(0 + "." + 75 + "." + 120);
    //    //GUIManager.Instance.SetcardDeckVersionNumber(_cardDeckCollection[0].MainVersion + "." + _cardDeckCollection[0].SubVersion + "." + _cardDeckCollection[0].SubSubVersion);


    //    //for (int i = 1; i < 4; i++) {
    //    //    string downloadUrl = "https://laeringsliv.azurewebsites.net/CardDeckCollections/DownloadCardDeckCollection?id=" + i;
    //    //    UnityWebRequest www = UnityWebRequest.Get(downloadUrl);

    //    //    yield return www.SendWebRequest();

    //    //    byte[] json = www.downloadHandler.data;

    //    //    try {
    //    //        _cardDeckCollection[i-1] = JsonUtility.FromJson<CardDeckCollection>(Encoding.UTF8.GetString(json));


    //    //        if (_cardDeckCollection[i-1] == null) {
    //    //            throw new Exception("Empty Card Deck");
    //    //        } else {
    //    //            SplashScreenLogic.Instance.DeckReady();
    //    //            CollectionSaveManager.Instance.SaveCollection(i, Encoding.UTF8.GetString(json));
    //    //        }


    //    //    // If a connection to the server cannot be established, use the on-device backup files to generate decks.
    //    //    } catch (Exception e) {
    //    //        print(e);
    //    //        //_cardDeckCollectionProvider = new GenerateCards();
    //    //        //_cardDeckCollection[i-1] = _cardDeckCollectionProvider.GetCardDeckCollection(1);
    //    //        _cardDeckCollection[i - 1] = JsonUtility.FromJson<CardDeckCollection>(CollectionSaveManager.Instance.LoadCollection(i));
    //    //        Debug.LogError("Could not get data from server. Using local backup");
    //    //    }
    //    //}
    //    ////TODO Making a dummy to demonstrate:
    //    //GUIManager.Instance.SetcardDeckVersionNumber(0 + "." + 75 + "." + 120);
    //    ////GUIManager.Instance.SetcardDeckVersionNumber(_cardDeckCollection[0].MainVersion + "." + _cardDeckCollection[0].SubVersion + "." + _cardDeckCollection[0].SubSubVersion);

    //}

    public  void TimeoutProcedure() {
        StopAllCoroutines();

        _cardDeckCollection = JsonConvert.DeserializeObject<CardDeckCollection>(CollectionSaveManager.Instance.LoadCollection(1));
        if (_cardDeckCollection != null) {
            Debug.LogError("Could not get data from server. Using local backup");
            LoginProcedure.Instance.UseBackup();
        } else {
            LoginProcedure.Instance.NoBackup();
        }
        



        //for (int i = 1; i < 4; i++) {
        //    _cardDeckCollection[i - 1] = JsonUtility.FromJson<CardDeckCollection>(CollectionSaveManager.Instance.LoadCollection(i));
        //    Debug.LogError("Could not get data from server. Using local backup");
        //}
        //TODO Making a dummy to demonstrate:
        GUIManager.Instance.SetcardDeckVersionNumber(0 + "." + 75 + "." + 120);
        //GUIManager.Instance.SetcardDeckVersionNumber(_cardDeckCollection[0].MainVersion + "." + _cardDeckCollection[0].SubVersion + "." + _cardDeckCollection[0].SubSubVersion);
    }
    
    // List of standard playcards
    public static  List<PlayCard> PlayCardList => Instance._cardDeckCollection.PlayCardDecks[0].PlayCards;

    // List of "Tips" cards, informational cards that can be shown after certain choices
    public static List<FeedbackCard> FeedbackCardList => Instance._cardDeckCollection.PlayCardDecks[0].FeedbackCards;

    public static List<BadgeCard> BadgeCardList => Instance._cardDeckCollection.PlayCardDecks[0].BadgeCards;

    public static List<PlayCard> SubStoryCardList = new List<PlayCard>();

    PlayCard CurrentActiveCard;


    //Check for and separate substory cards into their own deck
    public void SeparateSubStoryCards()
    {
        // To ensure no index shifting, we have to remove elements backwards
        for (int i = PlayCardList.Count - 1; i >= 0; i--)
        {
            if (PlayCardList[i].MinistoryKort)
            {
                SubStoryCardList.Add(PlayCardList[i]);
                PlayCardList.RemoveAt(i);
            }
        }
    }


    public List<PlayCard> GetSubstoryCardList () {
        return SubStoryCardList;
    }

    public List<PlayCard> CustomTrainingList ()
    {

        return PlayCardList;

    }

    public int GetNumCards() {
        return PlayCardList.Count;
    }

    public PlayCard GetCard(int index) {
        return PlayCardList[index];
    }

    public PlayCard GetSubstoryCard(int index) {
        return SubStoryCardList[index];
    }

    public PlayCard GetCardByID(int cardID) {
        foreach(PlayCard pc in PlayCardList) {
            if (pc.CardID == cardID) {
                return pc;
            }
        }
        return null;
    }

    public int GetCardIndexByID(int cardID) {
        for (int i = 0; i < PlayCardList.Count; i++) {
            if (PlayCardList[i].CardID == cardID) {
                return i;
            }
        }
        return -1;
    }

    public int GetSubstoryCardIndexByID(int cardID) {
        for (int i = 0; i < SubStoryCardList.Count; i++) {
            if (SubStoryCardList[i].CardID == cardID) {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Set the currently active card
    /// </summary>
    /// <param name="card"></param>
    public void SetCurrentActiveCard(PlayCard card) {
        CurrentActiveCard = card;
    }

    /// <summary>
    /// Get the currently active card
    /// </summary>
    /// <returns></returns>
    public PlayCard GetCurrentActiveCard() {
        return CurrentActiveCard;
    }

    public FeedbackCard GetTipsCard(int index) {
        return FeedbackCardList[index];
    }

    public Sprite GetCurrentGameLogo() {
        Sprite targetSprite = Resources.Load<Sprite>("images/" + _cardDeckCollection.LogoImage);
        if (targetSprite != null) {
            return targetSprite;
        } else {
            return Resources.Load<Sprite>("images/white-circle");
        }
        
    }


    /// <summary>
    /// New Methods for manually asking for server check
    /// </summary>
    /// <param name="cardID"></param>

    public void GetCardDeckCollection (string cardID) {
        StartCoroutine(GetCardDeckCollectionRoutine(cardID));
    }

    IEnumerator GetCardDeckCollectionRoutine(string deckID) {
        string downloadUrl = "https://laeringsliv.azurewebsites.net/CardDeckCollections/DownloadCardDeckCollection?id=" + deckID; 
        UnityWebRequest www = UnityWebRequest.Get(downloadUrl);

        yield return www.SendWebRequest();

        byte[] json = www.downloadHandler.data;

        try {
            string outp = Encoding.UTF8.GetString(json);
            _cardDeckCollection = JsonConvert.DeserializeObject<CardDeckCollection>(outp);

            if (_cardDeckCollection == null) {
                throw new Exception("Empty Card Deck");
            } else {
                LoginProcedure.Instance.DeckReady();
                LoginProcedure.Instance.SetCardDeckID(deckID);
                CollectionSaveManager.Instance.SaveCollection(1, Encoding.UTF8.GetString(json));
            }
            PlayerPrefs.SetString("DeckID", deckID);
            LoginProcedure.Instance.ConfirmUser();

            // If a connection to the server cannot be established, use the on-device backup files to generate decks.
        } catch (Exception e) {
            print(e);
            //_cardDeckCollectionProvider = new GenerateCards();
            //_cardDeckCollection[i-1] = _cardDeckCollectionProvider.GetCardDeckCollection(1);
            TimeoutProcedure();

        }
    }


    #region Deck Parsing

    /*
     * 
     * In this region we conver the current card deck into normal and special decks based on tags and other stuff 
     * 
     */

    List<PlayCard> NormalDeck = new List<PlayCard>();

    List<PlayCard> FollowCards = new List<PlayCard>();

    /// <summary>
    /// Parse a card deck and extract any special cards that do not belong in the standard deck
    /// </summary>
    /// <param name="cardDeck"></param>
    void ParseDeck(List<PlayCard> cardDeck) {

        for (int i = 0; i < cardDeck.Count; i++) {
            /*
             * if (cardDeck[i].tags.include("FollowCard")) {
             *      FollowCards.Add(cardDeck[i];
             * } else {
             *      NormalDeck.Add(cardDeck[i];
             * }
             */
        }


    }


    public PlayCard GetNormalCard(int index) {
        return NormalDeck[index];
    }


    /// <summary>
    /// Grab and return a follow card based on ID
    /// </summary>
    /// <param name="FollowId"></param>
    /// <returns></returns>
    public PlayCard GetFollowCard(int FollowId) {
        for (int i = 0; i < FollowCards.Count; i++) {
            if (FollowCards[i].CardID == FollowId) {
                return FollowCards[i];
            }
        }
        return null;
    }

    public List<BadgeCard> GetBadgeCardList() {
        return BadgeCardList;
    }

    public List<PlayCard> GetPlayCardList() {
        return PlayCardList;
    }

    public int GetScoreAmounts() {
        return  _cardDeckCollection.MainVersion;
    }

    public string[]GetScoreNames () {
        string[] names= new string[4];
        names[0] = _cardDeckCollection.mainscore1;
        names[1] = _cardDeckCollection.mainscore2;
        names[2] = _cardDeckCollection.mainscore3;
        names[3] = _cardDeckCollection.mainscore4;
        return names;

    }

    public int GetStartCardIndex() {
        return GetCardIndexByID(_cardDeckCollection.StartingCardID);
    }

    #endregion
}
