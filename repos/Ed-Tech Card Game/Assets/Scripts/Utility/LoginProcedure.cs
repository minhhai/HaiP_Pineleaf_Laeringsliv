using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class used for login logic
/// </summary>
public class LoginProcedure : MonoBehaviour {
    
    #region Singleton

    public static LoginProcedure Instance;

    #endregion

    #region Public variables 
    public Text LoadingDeckText;
    public Button StartButton;

    public Text TitleText;
    public InputField PlayerIDField;
    public InputField CardDeckIDField;
    public GameObject LoggedInUserPanel;
    public Text LoggedInUserText;

    public float timeoutTimer = 10.0f;

    #endregion

    #region Private variables

    bool registeredUser = false;

    bool deckReady = false;

    string cardDeckID;



    #endregion

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }


        SetFieldValues();

    }

    /// <summary>
    /// Loads presets and initializes field values
    /// </summary>
    public void SetFieldValues() {
        if (PlayerPrefs.GetInt("PlayerID") != 0) {
            PlayerIDField.text = PlayerPrefs.GetInt("PlayerID").ToString();
            registeredUser = true;
            TitleText.text = "Innlogget med Bruker ID: ";
        } else {

        }
        if (PlayerPrefs.GetString("DeckID") != "") {
            CardDeckIDField.text = PlayerPrefs.GetString("DeckID");
        }
    }

    
    public void SetCardDeckID(string ID) {
        cardDeckID = ID;
    }

    /// <summary>
    /// Confirm user and deck, last step in the login procedure before going to main menu
    /// </summary>
    public void ConfirmUser () {

        if (PlayerIDField.text != "") {
            Server.Instance.SetPlayerID(PlayerIDField.text);
            if (deckReady) {
                PlayerPrefs.SetInt("PlayerID", int.Parse(PlayerIDField.text));
                
                
            } else {
                LoadingDeckText.text = "Fant ingen gyldig spill, " +
               "vennligst prøv igjen.";
                StartButton.interactable = true;
                return;
            }
        } else {
            LoadingDeckText.text = "Vennligst sett en spiller ID.";
            StartButton.interactable = true;
            return;
        }


        // Check if user is supposed to have Debug methods
        CheckForSuperUser();

        if (cardDeckID != null) {
            Server.Instance.SetGameID(int.Parse(cardDeckID));
            Server.Instance.SetOrganizationID(cardDeckID);
        }

        int deviceID;
        if (PlayerPrefs.HasKey("DeviceID")) {
            deviceID = PlayerStateSaveManager.Instance.GetDeviceID();
        } else {
            PlayerStateSaveManager.Instance.GenerateRandomDeviceID();
            deviceID = PlayerStateSaveManager.Instance.GetDeviceID();
        }

        // Initialize unique Device ID 
        Server.Instance.SetDeviceID(deviceID);


        GameManager.Instance.CheckIfEmptyDeck();
        CardManager.Instance.SeparateSubStoryCards();
        GameManager.Instance.CheckActiveContinueButton();
        GUIManager.Instance.NewChangeUserText();
        GUIManager.Instance.NewDeckIDText();
        GUIManager.Instance.SetScoreVariables(CardManager.Instance.GetScoreAmounts(), CardManager.Instance.GetScoreNames());

        StartCoroutine(Stagger(2f));

        // Initialize scoring system based on current game information
        GUIManager.Instance.SetScoreSystem();
    }

    /// <summary>
    /// Simple stagger method, wait for t sseconds before proceeding to main menu (Mostly just to let people read system messages)
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    IEnumerator Stagger(float t) {
        yield return new WaitForSeconds(t);
        GUIManager.Instance.ToMainMenu();
        LoadingDeckText.text = "";
        StartButton.interactable = true;
        gameObject.SetActive(false);
    }

    

    

    private void StartTimeoutTimer() {
        StartCoroutine((TimeOutProcedure()));
    }

    public IEnumerator TimeOutProcedure () {
        float t = timeoutTimer;
        while (t > 0) {
            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        ConfirmUser();

    }

    /// <summary>
    /// New method, uses both the User ID and the Deck ID
    /// </summary>
    public void GetDeck () {
        CardManager.Instance.GetCardDeckCollection(CardDeckIDField.text);
        StartTimeoutTimer();
        StartButton.interactable = false;
    }

    #region Deck confirmation methods

    /// <summary>
    /// Received deck from server, confirm game
    /// </summary>
    public void DeckReady() {
        deckReady = true;
        LoadingDeckText.text = "Spill klart!";
        //BadgeManager.Instance.SetBadges(); // TODO: Temporary solution, need to look into making more dynamic badges later
        LoadBadges();
        StopAllCoroutines();
        ConfirmUser();
    }

    /// <summary>
    /// Did not recieve deck from server, but has local backup, confirm game
    /// </summary>
    public void UseBackup() {
        deckReady = true;
        LoadingDeckText.text = "Ingen kontakt med server, " +
            "bruker backup";
        LoadBadges();
        ConfirmUser();
    }


    /// <summary>
    /// Did not recieve deck from server, and has no local backup, reject game
    /// </summary>
    public void NoBackup () {
        LoadingDeckText.text = "Ingen kontakt med server og ingen backup, " +
                "vennligst vent ett minutt og prøv igjen.";
        StartButton.interactable = true;
    }

    #endregion

    /// <summary>
    /// Load badges and instantiate them in the badge menu
    /// </summary>
    public void LoadBadges() {
        BadgeManager.Instance.InitializeEarnedBadges();
    }

    #region Super user Stuff

    /// <summary>
    ///  Debug method, activate super user tools for the correct ID
    /// </summary>
    void CheckForSuperUser(){
        if (Server.Instance.GetPlayerID() == "1358" ) {
            GUIManager.Instance.ToggleSettingsDebugMenu(true);

        } else {
            GUIManager.Instance.ToggleSettingsDebugMenu(false);

        }
    }

    #endregion

}
