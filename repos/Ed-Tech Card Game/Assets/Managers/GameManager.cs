using System.Collections;
using System.Collections.Generic;
using LaeringslivCore;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Game Manager, everything related to general app logic goes through this
/// </summary>
public class GameManager : MonoBehaviour {

    #region Singleton

    public static GameManager Instance;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    #endregion


    private CardWeighing CWManager;

    private bool checkEmptyBadges = false;
    int currentCardIndex = -1;

    private void Start() {

        // Old Super User Method
        if (PlayerStateSaveManager.Instance.LoadThemeID() > 0) {
            AppThemeRefactor.Instance.ChangeTheme(PlayerStateSaveManager.Instance.LoadThemeID());
        }

        CWManager = GetComponent<CardWeighing>();
    }

    GUIManager guiManager => GUIManager.Instance;
    CardManager cardManager => CardManager.Instance;
    Swipe swipeManager => Swipe.Instance;
    PlayerManager playerManager => PlayerManager.Instance;
    GameEventManager gameEventManager => GameEventManager.Instance;

   

    /// <summary>
    /// variables holding the current players scores
    /// </summary>
    #region Player Scores

    int majorStat1, majorStat2, majorStat3, majorStat4;

    // Minor stats for major stat 1
    int minorStat1, minorStat2, minorStat3, minorStat4, minorStat5;
    // Minor stats for major stat 2
    int minorStat6, minorStat7, minorStat8, minorStat9, minorStat10;
    // Minor stats for major stat 3
    int minorStat11, minorStat12, minorStat13, minorStat14, minorStat15;
    // Minor stats for major stat 4
    int minorStat16, minorStat17, minorStat18, minorStat19, minorStat20;

    #endregion

    #region Game logic variables

    public enum GameState {
        Paused,
        Running
    }

    public GameState currentGameState = GameState.Paused;

    public bool superUserActive = false;
    #endregion

    /// <summary>
    /// Currently active Game coaches
    /// </summary>
    public List<ICoach> coaches = new List<ICoach>();
    public List<ICoach> swipeCoaches = new List<ICoach>();


    /// <summary>
    /// Start a new game and draw a card
    /// </summary>
    public void StartGame () {
        // Disable game confirmation panel
        guiManager.ToggleNewGameConfirmationPanel(false);

        // Set Start Card
        currentCardIndex = cardManager.GetStartCardIndex();

        print("Current start index is " + currentCardIndex);

        //Card Weighing methods 
        CWManager.InitValues(cardManager.GetPlayCardList().Count, 0.05f);
        //currentCardIndex = CWManager.DrawCardWeighted(); // Draw weighted random card, use this instead of random starting card if you want

        // Random starting card
        if (currentCardIndex < 0) {
            int randomnum = Random.Range(0, cardManager.GetPlayCardList().Count);
            print("Count is: " + cardManager.GetPlayCardList().Count + " Random Number: " + randomnum);

            currentCardIndex = Random.Range(0, cardManager.GetPlayCardList().Count);
        } else {

        }

        print("Current start index is " + currentCardIndex);

        swipeManager.SetSwipingAllowed(true);
        currentGameState = GameState.Running;
        guiManager.ResetVisualScores();

        GeneratePlayingCard(cardManager.GetCard(currentCardIndex)); 
        

        // Game Time manager method calls
        GameTimeManager.Instance.ResetDayCounter();
        GameTimeManager.Instance.UpdateDisplay();


        playerManager.UpdatePlayerVisualScores();


        // Coach stuff, enable when ready
        //swipeCoaches.Add(new BasicCoach());
        //swipeCoaches.Add(new TutorialCoach());
        //for (int i = 0; i < 20; i++) {
        //    StatBasedCoach coach = new StatBasedCoach();
        //    coach.SetStatID(i);
        //    coaches.Add(coach);
        //}
        
        //InitializeCoaches();

        guiManager.StartCardGame();
    }

    /// <summary>
    /// Starts a new game, if there is an old game stored, ask for confirmation
    /// </summary>
    public void StartNewGame() {

        if (PlayerPrefs.HasKey("CurrentCardID")) {
            guiManager.ToggleNewGameConfirmationPanel(true);
        } else {
            StartGame();
        }
    }

    /// <summary>
    /// Continue an existing game, loading game time and card data
    /// </summary>
    public void ContinueGame() {
        swipeManager.SetSwipingAllowed(true);
        currentGameState = GameState.Running;

        

        // TODO: This is not supposed to be here, but we don't have a save system in place for this just yet.
        CWManager.InitValues(cardManager.GetPlayCardList().Count, 0.05f);


        currentCardIndex = cardManager.GetCardIndexByID(PlayerPrefs.GetInt("CurrentCardID"));
        if (currentCardIndex < 0 ) {
            currentCardIndex = cardManager.GetSubstoryCardIndexByID(PlayerPrefs.GetInt("CurrentCardID"));
            GeneratePlayingCard(cardManager.GetSubstoryCard(currentCardIndex));
        } else {
            GeneratePlayingCard(cardManager.GetCard(currentCardIndex));
        } 
        

        PlayerManager.Instance.LoadPlayerStats();
        GameTimeManager.Instance.LoadDayCounter();
        GameTimeManager.Instance.UpdateDisplay();

        guiManager.StartCardGame();
    }

    /// <summary>
    /// Check if there is an active game session stored. Returns true if number of cards drawn is more than 0
    /// </summary>
    /// <returns></returns>
    public bool CheckIfActiveGame() {
        return (PlayerPrefs.GetInt("CurrentCardID") >= 0);
    }

    /// <summary>
    /// Initialize the visual card from an abstract PlayCard argument
    /// </summary>
    /// <param name="card"></param>
    void GeneratePlayingCard (PlayCard card) {
        cardManager.SetCurrentActiveCard(card);
        int[] randomArray = GameEventManager.Instance.GenerateRandomSwipeDirections();
        guiManager.InitializeCard(card, randomArray);
        PlayerPrefs.SetInt("CurrentCardID", card.CardID);
    }
    

    

    /// <summary>
    /// Get the index of the currently active card
    /// </summary>
    /// <returns></returns>
    public int GetCurrentCardIndex() {
        return currentCardIndex;
    }

    /// <summary>
    /// Get the Card ID of the currently active card
    /// </summary>
    /// <returns></returns>
    public int GetCurrentCardID() {
        return CardManager.Instance.GetPlayCardList()[currentCardIndex].CardID;
    }

    /// <summary>
    /// Proceed to the next card, draws a randomly, weighted card
    /// </summary>
    public void NextCard() {

        // Weighed random
        currentCardIndex = CWManager.DrawCardWeighted();

        //Update weights
        CWManager.UpdateModCardValues();
        
        guiManager.ResetCard();
        PlayCard nextCard = cardManager.GetCard(currentCardIndex);

        print($"New CardID: {nextCard.CardID}");
        GeneratePlayingCard(nextCard);
    }

    //public void ShowTipsCard(int tipsCardIndex) {
    //    guiManager.DisplayTipsCard(cardManager.GetTipsCard(tipsCardIndex));
    //}

    //public void HideTipsCard() {
    //    guiManager.HideTipscard();
    //}


    /// <summary>
    /// Draw a specific card given by card ID
    /// </summary>
    /// <param name="cardID"></param>
    public void DrawSpecificCard(int cardID) {
        guiManager.ResetCard();
        GeneratePlayingCard(cardManager.GetCard(cardManager.GetCardIndexByID(cardID)));
        currentCardIndex = cardManager.GetCardIndexByID(cardID);
        cardManager.SetCurrentActiveCard(cardManager.GetCard(cardManager.GetCardIndexByID(cardID)));
    }

    /// <summary>
    /// Draw a specific Substory card given by card ID
    /// </summary>
    /// <param name="cardID"></param>
    public void DrawSpecificSubstoryCard(int cardID) {
        guiManager.ResetCard();
        GeneratePlayingCard(cardManager.GetSubstoryCard(cardManager.GetSubstoryCardIndexByID(cardID)));
        currentCardIndex = cardManager.GetSubstoryCardIndexByID(cardID);
        cardManager.SetCurrentActiveCard(cardManager.GetSubstoryCard(cardManager.GetSubstoryCardIndexByID(cardID)));
    }
    


    /// Making these separate for now in case we want to add some extra functionality
    #region Menu methods

    public void OpenJournal () {
        guiManager.OpenJournal();
        currentGameState = GameState.Paused;
        swipeManager.SetSwipingAllowed(false);
    }

    public void OpenSettings() {
        guiManager.OpenSettings();
        currentGameState = GameState.Paused;
        swipeManager.SetSwipingAllowed(false);
    }

    public void OpenScore() {
        guiManager.OpenScore();
        currentGameState = GameState.Paused;
        swipeManager.SetSwipingAllowed(false);
    }

    public void OpenTraining() {
        guiManager.OpenTraining();
        currentGameState = GameState.Paused;
        swipeManager.SetSwipingAllowed(false);
    }

    public void OpenCharacters() {
        guiManager.OpenCharacters();
        currentGameState = GameState.Paused;
        swipeManager.SetSwipingAllowed(false);
    }


    
    public void OpenBadges() {
        //Debug
        //if (!checkEmptyBadges) { BadgeManager.Instance.DebugTestBadges(); checkEmptyBadges = true; }


        guiManager.OpenBadges();
        currentGameState = GameState.Paused;
        swipeManager.SetSwipingAllowed(false);
    }

    public void CloseJournal() {
        guiManager.CloseJournal();
        currentGameState = GameState.Running;
        swipeManager.SetSwipingAllowed(true);
    }

    public void CloseSettings() {
        guiManager.CloseSettings();
        currentGameState = GameState.Running;
        swipeManager.SetSwipingAllowed(true);
    }

    public void CloseScore() {
        guiManager.CloseScore();
        currentGameState = GameState.Running;
        swipeManager.SetSwipingAllowed(true);
    }

    public void CloseTraining() {
        guiManager.CloseTraining();
        currentGameState = GameState.Running;
        swipeManager.SetSwipingAllowed(true);
    }

    public void CloseCharacters() {
        guiManager.CloseCharacters();
        currentGameState = GameState.Running;
        swipeManager.SetSwipingAllowed(true);
    }

    public void CloseBadges() {
        guiManager.CloseBadges();
        currentGameState = GameState.Running;
        swipeManager.SetSwipingAllowed(true);
    }
    #endregion



    /// <summary>
    /// Quits game
    /// </summary>
    public void QuitGame () {
        Application.Quit();
    }

    public void ToggleGameSceneMenu() {
        guiManager.ToggleGameSceneMenu();
        EventManager.InspectMenuDropdownElement();
    }

    public void BackToStartMenu () {
        guiManager.BackToMenu();
        ResetAll();
        swipeManager.SetSwipingAllowed(false);
        Server.Instance.ForceSendBundle();
    }

    public void ResetAll() {
        guiManager.ResetAll();
        currentCardIndex = 0;
        playerManager.ResetStats();
        CheckActiveContinueButton();
    }

    public void InspectStat(int statID) {
        guiManager.InspectStat(statID);
        EventManager.InspectStatElement(statID);
    }

    public void CloseStatWindow() {
        guiManager.CloseStatWindow();
    }


    // Basically debug methods
    #region Superuser Methods

    public GameObject superUserCardChoosePanel;
    public Text targetIndexField;
    

    public void LoadSpecificPlayCard() {

        int targetIndex = 0;
        if (int.TryParse(targetIndexField.text, out targetIndex)) {
            targetIndex--;
            if (targetIndex < cardManager.GetNumCards() && targetIndex >= 0) {
                guiManager.ResetCard();
                guiManager.CloseSettings();
                currentGameState = GameState.Running;
                swipeManager.SetSwipingAllowed(true);
                GeneratePlayingCard(cardManager.GetCard(targetIndex));
                currentCardIndex--;
            } else {
                print("Index out of play card range");
            }
        } else {
            print("Not a valid input");
        }
        
        
    }

    public void SetStats(int index) {
        playerManager.SetStat(index, guiManager.SuperUserGetStatInput(index));
        playerManager.SavePlayerStats();
    }

    public void OpenSuperUserSettings() {
        if (superUserActive) {
            guiManager.OpenSettings();
            currentGameState = GameState.Paused;
        }
    }

    public void OpenSuperUserStatChangeWindow() {
        guiManager.OpenStatChangePage();
    }

    public void CloseSuperUserStatChangeWindow() {
        guiManager.CloseStatChangePage();
    }




    #endregion

    #region Various utility methods

    public void CheckActiveContinueButton() {
        guiManager.ToggleContinueActive(CheckIfActiveGame());
    }

    #endregion



    public void ToggleSettingsDebugOptions(bool toggle) {
        if (Server.Instance.GetPlayerID() != "1358") {
            guiManager.ToggleSettingsDebugMenu(toggle);
        }
        
    }


    #region Coach management

    void InitializeCoaches() {
        foreach (ICoach coach in coaches) {
            coach.EnableCoach();
        }
    }

    /// <summary>
    /// Check with the currently assigned coaches if the current action is allowed
    /// </summary>
    public bool CoachReview() {
        bool accepted = true;
        foreach (ICoach coach in coaches ) {
            if (!coach.Review(GetCurrentCardID())) {
                accepted = false;
            }
        }
        return accepted;
    }

    public bool SwipeCoachReview() {
        bool accepted = true;
        foreach (ICoach coach in swipeCoaches) {
            if (!coach.Review(GetCurrentCardID())) {
                accepted = false;
            }
        }
        return accepted;
    }


    public void CoachStartHoldEvent(Swipe.HoldDirection direction) {
        foreach (ICoach coach in coaches) {
            coach.StartEvent(direction);
        }
    }

    public void CoachStopHoldEvent(Swipe.HoldDirection direction) {
        foreach (ICoach coach in coaches) {
            coach.StopEvent(direction);
        }
    }

    #endregion

    #region Badge Management



    #endregion




    #region User ID methods

    public void TriggerChangeUserWindow(bool toggle) {
        guiManager.NewChangeUserText();
        guiManager.ToggleChangeUserWindow(toggle);
    }

    public void ChangeUserID () {
        PlayerPrefs.SetInt("PlayerID", int.Parse(guiManager.GetNewUserIDField()));
        guiManager.NewChangeUserText();
        TriggerChangeUserWindow(false);
    }

    public void TriggerChangeDeckWindow(bool toggle) {
        guiManager.NewDeckIDText();
        guiManager.ToggleChangeDeckWindow(toggle);
    }

    public void ChangeDeckID() {
        PlayerPrefs.SetString("DeckID", guiManager.GetNewDeckIDField());
        TriggerChangeDeckWindow(false);
    }


    #endregion

    #region Utility methods

    private void OnApplicationQuit() {
        BadgeManager.Instance.SaveEarnedBadgeList();
    }

    private void OnApplicationFocus(bool focus) {
        if (!focus) {
            BadgeManager.Instance.SaveEarnedBadgeList();
        }
    }

    private void OnApplicationPause(bool pause) {
        if (pause) {
            BadgeManager.Instance.SaveEarnedBadgeList();
        }
    }

    public void LogOut() {
        guiManager.LogOut();

    }

    public void CheckIfEmptyDeck() {
        if (cardManager.GetPlayCardList().Count < 1 || cardManager.GetPlayCardList() == null) {
            guiManager.ToggleGameButtons(false);
        } else {
            guiManager.ToggleGameButtons(true);
        }
    }

    #endregion
}
