using System.Collections;
using LaeringslivCore;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager handling all GUI elements and visual events
/// </summary>
public class GUIManager : MonoBehaviour {

    #region Singleton

    public static GUIManager Instance;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    #endregion

    #region UI element references 

    // UI Top level elements
    [SerializeField]
    private GameObject MainMenuPanel;
    [SerializeField]
    private GameObject GameWindowPanel;
    [SerializeField]
    private GameObject GameMenuPanel;

    public GameObject gameSceneMenuPanel;

    [SerializeField]
    private GameObject LoginscreenPanel;

    [SerializeField]
    private GameObject StatInspectPanel;

    [SerializeField]
    private GameObject NewGameConfirmationPanel;

    [SerializeField]
    private GameObject JournalWindowPanel;
    [SerializeField]
    private GameObject SettingsWindowPanel;
    [SerializeField]
    private GameObject TrainingWindowPanel;
    [SerializeField]
    private GameObject ScoreWindowPanel;
    [SerializeField]
    private GameObject CharactersWindowPanel;
    [SerializeField]
    private GameObject BadgesWindowPanel;

    // Main Menu window elements

    [SerializeField]
    private Button startGameButton;
    [SerializeField]
    private Button continueGamebutton;

    [SerializeField]
    private Image mainmenuLogo;

    [SerializeField]
    private GameObject EmptyCardDeckErrorMessage;

    // Game Window elements

    // Top row:
    [SerializeField]
    private GameObject MajorStat1Object;
    [SerializeField]
    private GameObject MajorStat2Object;
    [SerializeField]
    private GameObject MajorStat3Object;
    [SerializeField]
    private GameObject MajorStat4Object;
    private string[] StatNames = new string[4];
    [SerializeField]
    private Text[] StatValues;

    [SerializeField]
    private GameObject MajorStat1FillObject;
    [SerializeField]
    private GameObject MajorStat2FillObject;
    [SerializeField]
    private GameObject MajorStat3FillObject;
    [SerializeField]
    private GameObject MajorStat4FillObject;

    [SerializeField]
    private Color statOriginalColor;

    private Color stat1FillColor = new Color32(0x3d, 0xb1, 0xff, 0xff);
    private Color stat2FillColor = new Color32(0x40, 0xff, 0x3d, 0xff);
    private Color stat3FillColor = new Color32(0xff, 0x4a, 0x3d, 0xff);
    private Color stat4FillColor = new Color32(0xff, 0xbe, 0x3d, 0xff);



    [SerializeField]
    private GameObject SettingsObject;


    [SerializeField]
    private GameObject playCardObject;

    [SerializeField]
    private GameObject tipsCardObject;

    [SerializeField]
    private GameObject coachCardObject;




    [Header("Transforms")]
    public float swipeSpeed = 1.0f;
    public Transform CardStartTransform;
    public Transform CardLeftUITransform;
    public Transform CardRightUITransform;
    public Transform CardDownUITransform;

    public Transform CardLeftHoldTransform;
    public Transform CardRightHoldTransform;
    public Transform CardDownHoldTransform;

    public GameObject leftSwipeTextObject, rightSwipeTextObject, downSwipeTextObject;

    private Color textBoxColor, textBoxTextColor;
    
    #endregion

    
    private void Start() {
        textBoxColor = leftSwipeTextObject.GetComponent<Image>().color;
        textBoxTextColor = leftSwipeTextObject.GetComponentInChildren<Text>().color;
    }


    /// <summary>
    /// Toggle the confirmation window that pops up if you try to start a new game while an old one is still saved
    /// </summary>
    /// <param name="toggle"></param>
    public void ToggleNewGameConfirmationPanel (bool toggle) {
        NewGameConfirmationPanel.SetActive(toggle);
    }

    /// <summary>
    /// Enables and disables the relevant windows for starting a new game
    /// </summary>
    public void StartCardGame() {
        MainMenuPanel.SetActive(false);
        GameWindowPanel.SetActive(true);

        GameMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Initialize a new card from given values
    /// </summary>
    /// <param name="card"></param>
    /// <param name="randomArray"></param>
    public void InitializeCard(PlayCard card, int[] randomArray)
    {
        playCardObject.GetComponent<InSceneCard>().
            SetCardValues(card.ImageName, 
            card.Text, 
            new string[] { card.Choice1.Text, card.Choice2.Text, card.Choice3.Text }, 
            card.Title,
            randomArray);

    }

    /// <summary>
    /// Enable and display a given Tips card
    /// </summary>
    /// <param name="feedbackCard"></param>
    public void DisplayTipsCard(FeedbackCard feedbackCard)
    {
        tipsCardObject.GetComponent<InSceneTipsCard>().SetText(feedbackCard.Text);
        tipsCardObject.SetActive(true);
        Swipe.Instance.SetSwipingAllowed(false);
    }

    /// <summary>
    /// Hide said Tips card
    /// </summary>
    public void HideTipsCard() {
        tipsCardObject.SetActive(false);
        Swipe.Instance.SetSwipingAllowed(true);
    }

    // TODO: Convert this to use specialized buttons for starting various training sessions and etc
    /// <summary>
    /// Display a Coach card, which is a special kind of card capable of diverting the game onto a side story or training session
    /// </summary>
    /// <param name="message"></param>
    public void DisplayCoachCard(string message) {
        coachCardObject.GetComponent<InSceneCoachCard>().SetText(message);
        coachCardObject.SetActive(true);
        Swipe.Instance.SetSwipingAllowed(false);
    }

    /// <summary>
    /// Hide Coach card
    /// </summary>
    public void HideCoachCard() {
        coachCardObject.SetActive(false);
        Swipe.Instance.SetSwipingAllowed(true);
    }

    


    /// <summary>
    /// Starts the swipe animation
    /// </summary>
    /// <param name="swipeDirection"></param>
    public void RunSwipeAnimation(int swipeDirection) {
        StartCoroutine(SwipeEventAnimation(swipeDirection));
    }

    /// <summary>
    /// Animation for sliding the card off the screen
    /// </summary>
    /// <returns></returns>
    IEnumerator SwipeEventAnimation(int swipeDirection) {

        Vector3 currentPosition = playCardObject.transform.position;
        float p = 0;
        Transform newTransform = CardStartTransform;
        switch (swipeDirection) {
            case 0:
                newTransform = CardLeftUITransform;
                break;
            case 1:
                newTransform = CardRightUITransform;
                break;
            case 2:
                newTransform = CardDownUITransform;
                break;
            default:
                break;

        }
        while (p < 1) {
            Vector3 newPosition = Vector3.Lerp(currentPosition, newTransform.position, p);
            playCardObject.transform.position = newPosition;
            p += Time.deltaTime * swipeSpeed;
            yield return new WaitForEndOfFrame();
        }
        Swipe.Instance.SetSliding(false);
        GameEventManager.Instance.CardTransition(GameManager.Instance.GetCurrentCardIndex(), swipeDirection);
    }

    /// <summary>
    /// Reset Card transforms back to defaults
    /// </summary>
    public void ResetCard() {
        playCardObject.transform.position = CardStartTransform.position;
        playCardObject.transform.rotation = CardStartTransform.rotation;
        leftSwipeTextObject.SetActive(false);
        rightSwipeTextObject.SetActive(false);
        downSwipeTextObject.SetActive(false);
    }

    /// <summary>
    /// Disable the swipe text backgrounds that appear when you swipe a card in either direction
    /// </summary>
    public void ResetSwipeBackgrounds() {
        leftSwipeTextObject.SetActive(false);
        rightSwipeTextObject.SetActive(false);
        downSwipeTextObject.SetActive(false);
    }

    /// <summary>
    /// Method for handling the holding of cards in the various directions
    /// </summary>
    /// <param name="directionID"></param>
    public void HoldCard(int directionID) { // 0 = Left, 1 = Right, 2 = down
        switch (directionID) {
            case (0):
                playCardObject.transform.position = CardLeftHoldTransform.position;
                playCardObject.transform.rotation = CardLeftHoldTransform.rotation;
                ShowSwipeText(0);
                leftSwipeTextObject.GetComponent<Image>().color = new Color(textBoxColor.r, textBoxColor.g, textBoxColor.b, 0.85f);
                leftSwipeTextObject.GetComponentInChildren<Text>().color = new Color(textBoxTextColor.r, textBoxTextColor.g, textBoxTextColor.b, 0.85f);
                break;
            case (1):
                playCardObject.transform.position = CardRightHoldTransform.position;
                playCardObject.transform.rotation = CardRightHoldTransform.rotation;
                ShowSwipeText(1);
                rightSwipeTextObject.GetComponent<Image>().color = new Color(textBoxColor.r, textBoxColor.g, textBoxColor.b, 0.85f);
                rightSwipeTextObject.GetComponentInChildren<Text>().color = new Color(textBoxTextColor.r, textBoxTextColor.g, textBoxTextColor.b, 0.85f);
                break;
            case (2):
                playCardObject.transform.position = CardDownHoldTransform.position;
                playCardObject.transform.rotation = CardDownHoldTransform.rotation;
                ShowSwipeText(2);
                downSwipeTextObject.GetComponent<Image>().color = new Color(textBoxColor.r, textBoxColor.g, textBoxColor.b, 0.85f);
                downSwipeTextObject.GetComponentInChildren<Text>().color = new Color(textBoxTextColor.r, textBoxTextColor.g, textBoxTextColor.b, 0.85f);
                break;
            case (-1): // Reset
                break;
            default:
                break;
        }
    }

    void ShowSwipeText(int directionID) {
        switch (directionID) {
            case (0):
                leftSwipeTextObject.SetActive(true);
                rightSwipeTextObject.SetActive(false);
                downSwipeTextObject.SetActive(false);
                break;
            case (1):
                leftSwipeTextObject.SetActive(false);
                rightSwipeTextObject.SetActive(true);
                downSwipeTextObject.SetActive(false);
                break;
            case (2):
                leftSwipeTextObject.SetActive(false);
                rightSwipeTextObject.SetActive(false);
                downSwipeTextObject.SetActive(true);
                break;
            case (-1):
                leftSwipeTextObject.SetActive(false);
                rightSwipeTextObject.SetActive(false);
                downSwipeTextObject.SetActive(false);
                break;
            default:
                leftSwipeTextObject.SetActive(false);
                rightSwipeTextObject.SetActive(false);
                downSwipeTextObject.SetActive(false);
                break;
        }
    }


    #region New Swipe logic
    /*
    public void SwipeInText(int swipeID) {
        Vector3 newPosition = swipeTextHoldPostion.position;
        Scene3_leftSwipeTextObject.transform.position = leftTextOriginalPosition.position;
        Scene3_rightSwipeTextObject.transform.position = rightTextOriginalPosition.position;
        Scene3_downSwipeTextObject.transform.position = downTextOriginalPosition.position;
        switch (swipeID) {
            case (0): // Swipe left object right
                Scene3_leftSwipeTextObject.transform.position = newPosition;
                break;
            case (1): // Swipe right object left
                Scene3_rightSwipeTextObject.transform.position = newPosition;
                break;
            case (2):
                Scene3_downSwipeTextObject.transform.position = newPosition;
                break;
            default:

                break;
        }
    }

    public void SwipeOutText() {
        Scene3_leftSwipeTextObject.transform.position = leftTextOriginalPosition.position;
        Scene3_rightSwipeTextObject.transform.position = rightTextOriginalPosition.position;
        Scene3_downSwipeTextObject.transform.position = downTextOriginalPosition.position;
    }

    // Lock the text to the input field of the card
    void lockText(int textID) {
        switch (textID) {
            case (0):
                Scene3_leftSwipeTextObject.transform.position = swipeTextHoldPostion.position;
                break;
            case (1):
                Scene3_rightSwipeTextObject.transform.position = swipeTextHoldPostion.position;
                break;
            case (2):
                Scene3_downSwipeTextObject.transform.position = swipeTextHoldPostion.position;
                break;
            default:

                break;
        }
    }
    */



    #endregion
    

    //TODO: Testing
    public void DragCard(float x, float y) {
        //if (Mathf.Abs(x) > Mathf.Abs(y)) {
        //    if (x > 0) {
        //        playCardObject.transform.position = Vector3.Lerp(CardStartTransform.position, CardRightHoldTransform.position, x / deadzoneMagnitude);
        //        playCardObject.transform.rotation = Quaternion.Slerp(CardStartTransform.rotation, CardRightHoldTransform.rotation, x / deadzoneMagnitude);
        //    } else {
        //        playCardObject.transform.position = Vector3.Lerp(CardStartTransform.position, CardLeftHoldTransform.position, Mathf.Abs(x) / deadzoneMagnitude);
        //        playCardObject.transform.rotation = Quaternion.Slerp(CardStartTransform.rotation, CardLeftHoldTransform.rotation, Mathf.Abs(x) / deadzoneMagnitude);
        //    }
        //} else if (y < 0) {
        //    playCardObject.transform.position = Vector3.Lerp(CardStartTransform.position, CardDownHoldTransform.position, Mathf.Abs(y) / deadzoneMagnitude);
        //    playCardObject.transform.rotation = Quaternion.Slerp(CardStartTransform.rotation, CardDownHoldTransform.rotation, Mathf.Abs(y) / deadzoneMagnitude);
        //}

        float deadzoneMagnitude = Swipe.Instance.GetDeadZoneThreshold();

        Vector3 xLerp = Vector3.zero;
        if (x >= 0) {
            xLerp = Vector3.Lerp(CardStartTransform.position, CardRightHoldTransform.position, x / deadzoneMagnitude);
        } else {
            xLerp = Vector3.Lerp(CardStartTransform.position, CardLeftHoldTransform.position, Mathf.Abs(x) / deadzoneMagnitude);
        }
        if (y < 0) {
            playCardObject.transform.position = Vector3.Lerp(xLerp, CardDownHoldTransform.position, Mathf.Abs(y) / deadzoneMagnitude);
        } else {
            playCardObject.transform.position = xLerp;
        }
        
        

        if (Mathf.Abs(x) > Mathf.Abs(y)) {
            if (x > 0) {
                //playCardObject.transform.position = Vector3.Lerp(CardStartTransform.position, CardRightHoldTransform.position, x / deadzoneMagnitude);
                playCardObject.transform.rotation = Quaternion.Slerp(CardStartTransform.rotation, CardRightHoldTransform.rotation, x / deadzoneMagnitude);
                float alphaP = 0f;
                if (Mathf.Abs(x) > 100f) {
                    alphaP = (Mathf.Abs(x)) / deadzoneMagnitude;
                    if (alphaP > 0.85f) alphaP = 0.85f;
                } 
                
                rightSwipeTextObject.SetActive(true);
                leftSwipeTextObject.SetActive(false);
                downSwipeTextObject.SetActive(false);
                rightSwipeTextObject.GetComponent<Image>().color = new Color(textBoxColor.r, textBoxColor.g, textBoxColor.b, alphaP);
                rightSwipeTextObject.GetComponentInChildren<Text>().color = new Color(textBoxTextColor.r, textBoxTextColor.g, textBoxTextColor.b, alphaP);
            } else {
                //playCardObject.transform.position = Vector3.Lerp(CardStartTransform.position, CardLeftHoldTransform.position, Mathf.Abs(x) / deadzoneMagnitude);
                playCardObject.transform.rotation = Quaternion.Slerp(CardStartTransform.rotation, CardLeftHoldTransform.rotation, Mathf.Abs(x) / deadzoneMagnitude);

                float alphaP = 0f;
                if (Mathf.Abs(x) > 100f) {
                    alphaP = (Mathf.Abs(x)) / deadzoneMagnitude;
                    if (alphaP > 0.85f) alphaP = 0.85f;
                }

                rightSwipeTextObject.SetActive(false);
                leftSwipeTextObject.SetActive(true);
                downSwipeTextObject.SetActive(false);
                leftSwipeTextObject.GetComponent<Image>().color = new Color(textBoxColor.r, textBoxColor.g, textBoxColor.b, alphaP);
                leftSwipeTextObject.GetComponentInChildren<Text>().color = new Color(textBoxTextColor.r, textBoxTextColor.g, textBoxTextColor.b, alphaP);
            }
        } else if (y < 0) {
            //playCardObject.transform.position = Vector3.Lerp(CardStartTransform.position, CardDownHoldTransform.position, Mathf.Abs(y) / deadzoneMagnitude);
            playCardObject.transform.rotation = Quaternion.Slerp(CardStartTransform.rotation, CardDownHoldTransform.rotation, Mathf.Abs(y) / deadzoneMagnitude);

            float alphaP = 0f;
            if (Mathf.Abs(y) > 100f) {
                alphaP = (Mathf.Abs(y)) / deadzoneMagnitude;
                if (alphaP > 0.85f) alphaP = 0.85f;
            }
            rightSwipeTextObject.SetActive(false);
            leftSwipeTextObject.SetActive(false);
            downSwipeTextObject.SetActive(true);
            downSwipeTextObject.GetComponent<Image>().color = new Color(textBoxColor.r, textBoxColor.g, textBoxColor.b, alphaP);
            downSwipeTextObject.GetComponentInChildren<Text>().color = new Color(textBoxTextColor.r, textBoxTextColor.g, textBoxTextColor.b, alphaP);
        }
    }







    // Temporary stat change flash methods

    public void HandleStatChangeGraphics(int statIndex, int statChange, int newStatValue) {
        float newFillpercent = (float)newStatValue / 500f;
        switch (statIndex) {
            case (0):
                StatValues[0].text = PlayerManager.Instance.GetMainStat(0).ToString();
                StartCoroutine(ChangeFillPercent(MajorStat1FillObject, newFillpercent, stat1FillColor));
                if (statChange > 0) {
                    StartCoroutine(FlashStatColor(MajorStat1FillObject, Color.green));
                } else if (statChange < 0) {
                    StartCoroutine(FlashStatColor(MajorStat1FillObject, Color.red));
                }
                break;
            case (1):
                StatValues[1].text = PlayerManager.Instance.GetMainStat(1).ToString();
                StartCoroutine(ChangeFillPercent(MajorStat2FillObject, newFillpercent, stat2FillColor));
                if (statChange > 0) {
                    StartCoroutine(FlashStatColor(MajorStat2FillObject, Color.green));
                } else if (statChange < 0) {
                    StartCoroutine(FlashStatColor(MajorStat2FillObject, Color.red));
                }
                break;
            case (2):
                StatValues[2].text = PlayerManager.Instance.GetMainStat(2).ToString();
                StartCoroutine(ChangeFillPercent(MajorStat3FillObject, newFillpercent, stat3FillColor));
                if (statChange > 0) {
                    StartCoroutine(FlashStatColor(MajorStat3FillObject, Color.green));
                } else if (statChange < 0) {
                    StartCoroutine(FlashStatColor(MajorStat3FillObject, Color.red));
                }
                break;
            case (3):
                StatValues[3].text = PlayerManager.Instance.GetMainStat(3).ToString();
                StartCoroutine(ChangeFillPercent(MajorStat4FillObject, newFillpercent, stat4FillColor));
                if (statChange > 0) {
                    StartCoroutine(FlashStatColor(MajorStat4FillObject, Color.green));
                } else if (statChange < 0) {
                    StartCoroutine(FlashStatColor(MajorStat4FillObject, Color.red));
                }
                break;
            default:
                break;
        }


    }

    IEnumerator FlashStatColor(GameObject statElement, Color _color) {
        Image statElementImage = statElement.GetComponent<Image>();
        statElementImage.color = _color;
        yield return new WaitForSeconds(0.5f);
        statElementImage.color = statOriginalColor;
    }


    IEnumerator ChangeFillPercent(GameObject fillImage, float targetFillPercent, Color statColor) {
        Image fillElement = fillImage.GetComponent<Image>();
        float startFillPercent = fillElement.fillAmount;
        float p = 0;
        while (p < 1.0f) {
            fillElement.fillAmount = Mathf.Lerp(startFillPercent, targetFillPercent, p);

            p += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        fillElement.fillAmount = targetFillPercent;
    }


    // Comment: Putting these in separate methods in case we want to expand on them later
    #region Menu Methods

    public void OpenJournal() {

        JournalWindowPanel.SetActive(true);
        gameSceneMenuPanel.SetActive(false);
    }

    public void OpenSettings() {
        SettingsWindowPanel.SetActive(true);
        gameSceneMenuPanel.SetActive(false);
    }

    public void OpenCharacters() {
        CharactersWindowPanel.SetActive(true);
        gameSceneMenuPanel.SetActive(false);
    }

    public void OpenScore () {
        ScoreWindowPanel.SetActive(true);
        gameSceneMenuPanel.SetActive(false);
    }

    public void OpenTraining() {
        TrainingWindowPanel.SetActive(true);
        gameSceneMenuPanel.SetActive(false);
    }

    public void OpenBadges() {
        BadgesWindowPanel.SetActive(true);
        gameSceneMenuPanel.SetActive(false);
    }

    public void CloseJournal() {

        JournalWindowPanel.SetActive(false);
    }

    public void CloseSettings() {
        SettingsWindowPanel.SetActive(false);
    }

    public void CloseCharacters() {
        CharactersWindowPanel.SetActive(false);
    }

    public void CloseScore() {
        ScoreWindowPanel.SetActive(false);
    }

    public void CloseTraining() {
        TrainingWindowPanel.SetActive(false);
    }

    public void CloseBadges() {
        BadgesWindowPanel.SetActive(false);
    }

    bool menuIsSliding = false;


    //* Warning: Messy Methods ahead!! *//

    public void ToggleGameSceneMenu() {
        if (!menuIsSliding) StartCoroutine(SlideGameSceneMenu());
        
        //gameSceneMenuPanel.SetActive(!gameSceneMenuPanel.activeSelf);
    }


    public Transform SlideMenuOutPosition, SlideMenuInPosition;

    float slideSpeed = 4.0f;

    IEnumerator SlideGameSceneMenu() {
        menuIsSliding = true;
        float p = 0;
        if (!gameSceneMenuPanel.activeSelf) { // Set active and slide in

            Swipe.Instance.SetSwipingAllowed(false);

            gameSceneMenuPanel.SetActive(true);
            while (p < 1) {
                gameSceneMenuPanel.transform.position = Vector3.Lerp(SlideMenuOutPosition.position, SlideMenuInPosition.position, p);
                p += Time.deltaTime * slideSpeed;
                yield return new WaitForEndOfFrame();
            }
            gameSceneMenuPanel.transform.position = SlideMenuInPosition.position;
        } else { // slide out and set inactive
            while (p < 1) {
                gameSceneMenuPanel.transform.position = Vector3.Lerp(SlideMenuInPosition.position, SlideMenuOutPosition.position, p);
                p += Time.deltaTime * slideSpeed;
                yield return new WaitForEndOfFrame();
            }
            gameSceneMenuPanel.transform.position = SlideMenuInPosition.position;
            gameSceneMenuPanel.SetActive(false);

            Swipe.Instance.SetSwipingAllowed(true);

        }
        menuIsSliding = false;
    }

    public void BackToMenu() {
        GameWindowPanel.SetActive(false);
        GameMenuPanel.SetActive(false);
        JournalWindowPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void ToMainMenu() {
        mainmenuLogo.sprite = CardManager.Instance.GetCurrentGameLogo();
        MainMenuPanel.SetActive(true);
    }

    public void ResetAll() {
        gameSceneMenuPanel.SetActive(false);
    }
    #endregion



    #region Inspect Stat methods

    public Image InspectStatBGImage, InspectStatFillImage;
    public Text InspectStatTitle;
    public Text InspectStatNumber;

    public void InspectStat (int statID) {
        if (!StatInspectPanel.activeSelf) {
            StatInspectPanel.SetActive(true);
            Swipe.Instance.SetSwipingAllowed(false);
            
        }
        switch (statID) {
                case (0):
                    InspectStatTitle.text = StatNames[0];
                    InspectStatBGImage.sprite = MajorStat1FillObject.GetComponent<Image>().sprite;
                    InspectStatFillImage.sprite = MajorStat1FillObject.GetComponent<Image>().sprite;
                    InspectStatFillImage.fillAmount = (float) PlayerManager.Instance.GetMainStat(0)/ 500f;
                    InspectStatNumber.text = (PlayerManager.Instance.GetMainStat(0).ToString());
                break;
                case (1):
                    InspectStatTitle.text = StatNames[1];
                    InspectStatBGImage.sprite = MajorStat2FillObject.GetComponent<Image>().sprite;
                    InspectStatFillImage.sprite = MajorStat2FillObject.GetComponent<Image>().sprite;
                    InspectStatFillImage.fillAmount = (float)PlayerManager.Instance.GetMainStat(1) / 500f;
                    InspectStatNumber.text = (PlayerManager.Instance.GetMainStat(1).ToString());
                break;
                case (2):
                    InspectStatTitle.text = StatNames[2];
                    InspectStatBGImage.sprite = MajorStat3FillObject.GetComponent<Image>().sprite;
                    InspectStatFillImage.sprite = MajorStat3FillObject.GetComponent<Image>().sprite;
                    InspectStatFillImage.fillAmount = (float)PlayerManager.Instance.GetMainStat(2) / 500f;
                    InspectStatNumber.text = (PlayerManager.Instance.GetMainStat(2).ToString());
                break;
                case (3):
                    InspectStatTitle.text = StatNames[3];
                    InspectStatBGImage.sprite = MajorStat4FillObject.GetComponent<Image>().sprite;
                    InspectStatFillImage.sprite = MajorStat4FillObject.GetComponent<Image>().sprite;
                    InspectStatFillImage.fillAmount = (float)PlayerManager.Instance.GetMainStat(3) / 500f;
                    InspectStatNumber.text = (PlayerManager.Instance.GetMainStat(3).ToString());
                break;
            }
    }

    public void CloseStatWindow() {
        StatInspectPanel.SetActive(false);
        Swipe.Instance.SetSwipingAllowed(true);
    }


    #endregion


    #region Super User stuff

    [Header("Super User Stuff")]

    [SerializeField]
    private GameObject SuperUserOptionsWindow;

    [SerializeField]
    private GameObject SuperUserStatChangeWindow;

    public Image SuperUserMainStat1Fill;
    public Image SuperUserMainStat2Fill;
    public Image SuperUserMainStat3Fill;
    public Image SuperUserMainStat4Fill;
    public Text[] SuperUserMainStatsPercents;
    public InputField[] SuperuserStatInputs;

    public int SuperUserGetStatInput(int index) {
        int returnValue = int.Parse(SuperuserStatInputs[index].text);
        if (returnValue > 100) {
            returnValue = 100;
            SuperuserStatInputs[index].text = "100";
        }
        return returnValue;
    }

    public void SuperUserHandleStatChangeGraphics(int statIndex, int statChange, int newStatValue) {
        UpdateStatChangePercent(statIndex);
        float newFillpercent = (float)newStatValue / 400f;
        switch (statIndex) {
            case (0):
                StatValues[0].text = PlayerManager.Instance.GetMainStat(0).ToString();
                StartCoroutine(ChangeFillPercent(SuperUserMainStat1Fill.gameObject, newFillpercent, stat1FillColor));
                if (statChange > 0) {
                    StartCoroutine(FlashStatColor(SuperUserMainStat1Fill.gameObject, Color.green));
                } else {
                    StartCoroutine(FlashStatColor(SuperUserMainStat1Fill.gameObject, Color.red));
                }
                break;
            case (1):
                StatValues[0].text = PlayerManager.Instance.GetMainStat(1).ToString();
                StartCoroutine(ChangeFillPercent(SuperUserMainStat2Fill.gameObject, newFillpercent, stat2FillColor));
                if (statChange > 0) {
                    StartCoroutine(FlashStatColor(SuperUserMainStat2Fill.gameObject, Color.green));
                } else {
                    StartCoroutine(FlashStatColor(SuperUserMainStat2Fill.gameObject, Color.red));
                }
                break;
            case (2):
                StatValues[0].text = PlayerManager.Instance.GetMainStat(2).ToString();
                StartCoroutine(ChangeFillPercent(SuperUserMainStat3Fill.gameObject, newFillpercent, stat3FillColor));
                if (statChange > 0) {
                    StartCoroutine(FlashStatColor(SuperUserMainStat3Fill.gameObject, Color.green));
                } else {
                    StartCoroutine(FlashStatColor(SuperUserMainStat3Fill.gameObject, Color.red));
                }
                break;
            case (3):
                StatValues[0].text = PlayerManager.Instance.GetMainStat(3).ToString();
                StartCoroutine(ChangeFillPercent(SuperUserMainStat4Fill.gameObject, newFillpercent, stat4FillColor));
                if (statChange > 0) {
                    StartCoroutine(FlashStatColor(SuperUserMainStat4Fill.gameObject, Color.green));
                } else {
                    StartCoroutine(FlashStatColor(SuperUserMainStat4Fill.gameObject, Color.red));
                }
                break;
            default:
                break;
        }


    }

    public void OpenStatChangePage () {
        SuperUserStatChangeWindow.SetActive(true);
        UpdateStatInputFieldWindowValues();
        for (int i = 0; i < 4; i++) {
            UpdateStatChangePercent(0);
            UpdateStatChangePercent(1);
            UpdateStatChangePercent(2);
            UpdateStatChangePercent(3);
        }
    }

    public void CloseStatChangePage() {
        SuperUserStatChangeWindow.SetActive(false);
        UpdateSuperUserChanges();
    }

    public void UpdateStatInputFieldWindowValues() {
        SuperUserMainStat1Fill.fillAmount = PlayerManager.Instance.GetMainStatPercent(0);
        SuperUserMainStat2Fill.fillAmount = PlayerManager.Instance.GetMainStatPercent(1);
        SuperUserMainStat3Fill.fillAmount = PlayerManager.Instance.GetMainStatPercent(2);
        SuperUserMainStat4Fill.fillAmount = PlayerManager.Instance.GetMainStatPercent(3);
        for (int i = 0; i < 16; i++) {
            SuperuserStatInputs[i].text = PlayerManager.Instance.GetSubStat(i).ToString();
        }
    }

    public void UpdateSuperUserChanges() {
        MajorStat1FillObject.GetComponent<Image>().fillAmount = PlayerManager.Instance.GetMainStatPercent(0);
        MajorStat2FillObject.GetComponent<Image>().fillAmount = PlayerManager.Instance.GetMainStatPercent(1);
        MajorStat3FillObject.GetComponent<Image>().fillAmount = PlayerManager.Instance.GetMainStatPercent(2);
        MajorStat4FillObject.GetComponent<Image>().fillAmount = PlayerManager.Instance.GetMainStatPercent(3);
    }

    public void UpdateStatChangePercent(int index) {
        SuperUserMainStatsPercents[index].text = (int)(PlayerManager.Instance.GetMainStatPercent(index)*100) + "";
    }


    public void ToggleSuperUserOptions(bool toggle) {
        SuperUserOptionsWindow.SetActive(toggle);
    }

    #endregion

    #region Timer stuff

    public Text TimerDisplay, WeekDisplay;

    public void UpdateTimer (int d, int w ) {
        TimerDisplay.text = "Dag: " + d;
        WeekDisplay.text = "Uke: " + w;
    }

    #endregion

    #region Various utility methods

    public void UpdateMainStatGraphics(int[] mainStatsPercent) {

        MajorStat1FillObject.GetComponent<Image>().fillAmount = mainStatsPercent[0] / 500f;
        MajorStat2FillObject.GetComponent<Image>().fillAmount = mainStatsPercent[1] / 500f;
        MajorStat3FillObject.GetComponent<Image>().fillAmount = mainStatsPercent[2] / 500f;
        MajorStat4FillObject.GetComponent<Image>().fillAmount = mainStatsPercent[3] / 500f;
    }

    public void ToggleContinueActive(bool toggle) {
        continueGamebutton.interactable = toggle;
    }


    [SerializeField]
    private GameObject SettingsDebugStuff;
    [SerializeField]
    private GameObject DebugReporter;
    public void ToggleSettingsDebugMenu(bool toggle) {
        SuperUserOptionsWindow.SetActive(toggle);
        DebugReporter.SetActive(toggle);
    }


    #endregion

    public Text versionNumberText;

    public void SetcardDeckVersionNumber( string version) {
        versionNumberText.text = "Spillversjon: " + version;
    }

    #region SettingsWindowStuff

    [SerializeField]
    private GameObject ChangeUserWindow;
    [SerializeField]
    private Text ChangeUserText;
    [SerializeField]
    private InputField NewUserIDIF;

    [SerializeField]
    private GameObject ChangeDeckWindow;
    [SerializeField]
    private Text ChangeDeckText;
    [SerializeField]
    private InputField NewDeckIDIF;



    public void ToggleChangeUserWindow(bool toggle) {
        ChangeUserWindow.SetActive(toggle);
    }

    public void NewChangeUserText () {
        ChangeUserText.text = "Innlogget med Bruker ID: " + PlayerPrefs.GetInt("PlayerID");
    }

    public string GetNewUserIDField() {
        return NewUserIDIF.text;
    }


    public void ToggleChangeDeckWindow(bool toggle) {
        ChangeDeckWindow.SetActive(toggle);
    }

    public void NewDeckIDText () {
        ChangeDeckText.text = "Spill: " + PlayerPrefs.GetString("DeckID");
    }

    public string GetNewDeckIDField() {
        return NewDeckIDIF.text;
    }

    public void LogOut() {
        SettingsWindowPanel.SetActive(false);
        GameWindowPanel.SetActive(false);
        GameMenuPanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        LoginProcedure.Instance.SetFieldValues();
        LoginscreenPanel.SetActive(true);
    }

    #endregion



    #region New Score System Toggle

    [Header("Score System stuff")]
    [SerializeField]
    private GameObject ScoreBar;
    [SerializeField]
    bool scoreEnabled;

    /// <summary>
    /// Set if the project has score enabled
    /// </summary>
    /// <param name="scoreAmount"></param>
    public void SetScoreSystem() {
        if (scoreEnabled) { EnableScores(); }
        else { DisableScores(); }
    }

    public void EnableScores() {
        ScoreBar.SetActive(true);
    }

    public void DisableScores() {
        ScoreBar.SetActive(false);
    }

    /// <summary>
    /// Set nr of visible scores as well as the names of those scores
    /// </summary>
    /// <param name="n"></param>
    /// <param name="scoreNames"></param>
    public void SetScoreVariables(int n, string[] scoreNames) {

        MajorStat1Object.SetActive(false);
        MajorStat2Object.SetActive(false);
        MajorStat3Object.SetActive(false);
        MajorStat4Object.SetActive(false);

        //if (n > 0) {
        //    MajorStat1Object.SetActive(true);
        //    StatNames[0] = scoreNames[0];
        //    // Set name for Stat 1 here
        //    if (n > 1) {
        //        MajorStat2Object.SetActive(true);
        //        StatNames[1] = scoreNames[1];
        //        // Set name for Stat 2 here
        //        if (n > 2 ) {
        //            MajorStat3Object.SetActive(true);
        //            StatNames[2] = scoreNames[2];
        //            // Set name for Stat 3 here
        //            if (n > 3) {
        //                MajorStat4Object.SetActive(true);
        //                StatNames[3] = scoreNames[3];
        //                // Set name for Stat 4 here
        //            }
        //        }
        //    }
        //}
        
        if (scoreNames[0] != "" && scoreNames[0] != null) {
            MajorStat1Object.SetActive(true);
            StatNames[0] = scoreNames[0];
        }
        if (scoreNames[1] != "" && scoreNames[1] != null) {
            MajorStat2Object.SetActive(true);
            StatNames[1] = scoreNames[1];
        }
        if (scoreNames[2] != "" && scoreNames[2] != null) {
            MajorStat3Object.SetActive(true);
            StatNames[2] = scoreNames[2];
        }
        if (scoreNames[3] != "" && scoreNames[3] != null) {
            MajorStat4Object.SetActive(true);
            StatNames[3] = scoreNames[3];
        }



    }

    public void ResetVisualScores() {
        foreach (Text text in StatValues) {
            text.text = "0";
        }
    }

    public void ToggleGameButtons(bool toggle) {
        startGameButton.interactable = toggle;
        if (!toggle) {
            EmptyCardDeckErrorMessage.SetActive(true);
            continueGamebutton.interactable = toggle;
        } else {
            EmptyCardDeckErrorMessage.SetActive(false);
        }
    }

    #endregion

    [SerializeField]
    private GameObject BadgePopupPanel;

    public void TriggerBadgePopup (BadgeCard bc) {
        Badge badgePanel = BadgePopupPanel.GetComponentInChildren<Badge>();
        badgePanel.SetValues(bc.Image, bc.TitleText, bc.BodyText, bc.VideoUrl);
        BadgeAnim();
    }
    public void BadgeAnim() {
        BadgePopupPanel.GetComponent<Animator>().SetTrigger("PlayBadgePop");
    }
}
