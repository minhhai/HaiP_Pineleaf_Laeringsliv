using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for handling the visual components of the card class
/// </summary>
public class InSceneCard : MonoBehaviour {

    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private Text shortCardText;
    [SerializeField]
    private Text longCardText;
    [SerializeField]
    private Text CardCharacterName;
    [SerializeField]
    private GameObject CardCharacterNamebackground;


    // References to the various parts of the card
    public GameObject shortTextBox, imagebox, longTextBox;
    // References to the various swipe event texts
    public Text leftSwipeText, rightSwipeText, downSwipeText;

    public void SetCardValues(string _cardImageID, string _cardText, string[] cardEventTexts, string _cardCharacter, int[] randomArray) {

        if (_cardImageID != "Tekst" && _cardImageID != "" ) { // TODO: Insert whatever identifier we use for no picture here, "Tekst" was the one that was used in the examples I got
            shortTextBox.SetActive(true);
            imagebox.SetActive(true);
            longTextBox.SetActive(false);
            cardImage.sprite = Resources.Load<Sprite>("images/" + _cardImageID); // Folder to put image resources in
            shortCardText.text = _cardText;
            if (_cardCharacter != "N/A") {
                CardCharacterName.text = _cardCharacter;
                CardCharacterNamebackground.SetActive(true);
            } else {
                CardCharacterName.text = "";
                CardCharacterNamebackground.SetActive(false);
            }
        } else {
            shortTextBox.SetActive(false);
            imagebox.SetActive(false);
            longTextBox.SetActive(true);
            cardImage.sprite = null;
            longCardText.text = _cardText;
        }
        
        leftSwipeText.text = cardEventTexts[randomArray[0]];
        rightSwipeText.text = cardEventTexts[randomArray[1]];
        downSwipeText.text = cardEventTexts[randomArray[2]];
    }

}
