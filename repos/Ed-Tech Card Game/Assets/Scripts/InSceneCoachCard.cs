using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// In-game representation of the coach card
/// </summary>
public class InSceneCoachCard : MonoBehaviour {

    [SerializeField]
    private Text coachCardText;


    public void SetText(string _text) {
        coachCardText.text = _text;
    }
}
