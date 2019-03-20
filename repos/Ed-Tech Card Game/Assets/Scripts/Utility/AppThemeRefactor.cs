using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Class for handling changing between different app themes.
/// </summary>
public class AppThemeRefactor : MonoBehaviour {

    public static AppThemeRefactor Instance;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }


    [SerializeField]
    private Sprite DefaultThemeMainMenuIcon;


    [Header("Theme 1")]
    [SerializeField]
    private Sprite Theme1MainMenuIcon;
    [SerializeField]
    private Color Theme1Color;
    [SerializeField]
    private Color Theme1FontColor;
    [SerializeField]
    private Color Theme1ButtonNormalColor;

    [Header("Theme 2")]
    [SerializeField]
    private Sprite Theme2MainMenuIcon;
    [SerializeField]
    private Color Theme2Color;
    [SerializeField]
    private Color Theme2FontColor;
    [SerializeField]
    private Color Theme2ButtonNormalColor;

    [Header("Theme 3")]
    [SerializeField]
    private Sprite Theme3MainMenuIcon;
    [SerializeField]
    private Color Theme3Color;




    [Header("App contents")]
    [SerializeField]
    private GameObject MainMenuLogo;
    [SerializeField]
    private GameObject MainMenuBackground;
    [SerializeField]
    private GameObject[] MainMenuButtons;
    [SerializeField]
    private GameObject[] MainmenuTexts;



    [SerializeField]
    private GameObject[] ProjectColor1Objects;
    [SerializeField]
    private GameObject[] ProjectFontColorObjects;
    [SerializeField]
    private GameObject[] ProjectButtonColorObjects;
    [SerializeField]
    private GameObject[] JournalEntries;




    public void ChangeTheme(int themeID) {

        Sprite logoSprite;
        Color themeColor;
        Color fontColor;
        Color buttonNormalColor;

        CardManager.Instance.SetTargetStack(themeID);

        PlayerStateSaveManager.Instance.SaveThemeID(themeID);
        switch (themeID) {
            case (0):
                logoSprite = DefaultThemeMainMenuIcon;
                themeColor = Theme1Color;
                fontColor = Theme1FontColor;
                buttonNormalColor = Theme1ButtonNormalColor;
                break;
            case (1):
                logoSprite = Theme1MainMenuIcon;
                themeColor = Theme1Color;
                fontColor = Theme1FontColor;
                buttonNormalColor = Theme1ButtonNormalColor;
                break;
            case (2):
                logoSprite = Theme2MainMenuIcon;
                themeColor = Theme2Color;
                fontColor = Theme2FontColor;
                buttonNormalColor = Theme2ButtonNormalColor;
                break;
            default:
                logoSprite = DefaultThemeMainMenuIcon;
                themeColor = Theme1Color;
                fontColor = Theme1FontColor;
                buttonNormalColor = Theme1ButtonNormalColor;
                break;
        }
        MainMenuLogo.GetComponent<Image>().sprite = logoSprite;
        MainMenuBackground.GetComponent<Image>().color = themeColor;
        foreach (GameObject button in MainMenuButtons) {
            ColorBlock newColor = button.GetComponent<Button>().colors;
            newColor.normalColor = buttonNormalColor;
            button.GetComponent<Button>().colors = newColor;

            button.GetComponentInChildren<Text>().color = fontColor;
        }

        foreach (GameObject ob in ProjectColor1Objects) {
            ob.GetComponent<Image>().color = themeColor;
        }
        foreach (GameObject button in ProjectButtonColorObjects) {
            ColorBlock newColor = button.GetComponent<Button>().colors;
            newColor.normalColor = buttonNormalColor;
            button.GetComponent<Button>().colors = newColor;

            if (button.GetComponentInChildren<Text>() != null) button.GetComponentInChildren<Text>().color = fontColor;
        }
        foreach (GameObject fontOb in ProjectFontColorObjects) {
            if (fontOb.GetComponent<Image>() != null) fontOb.GetComponent<Image>().color = fontColor;
            else if (fontOb.GetComponent<Text>() != null) fontOb.GetComponent<Text>().color = fontColor;
        }
    }

    public void Update() {
    }
}
