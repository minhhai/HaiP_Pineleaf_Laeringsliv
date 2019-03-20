using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple Hint card GUI class
/// </summary>
public class InSceneTipsCard : MonoBehaviour {

    [SerializeField]
    private Text tipsCardText;


    public void SetText(string _text) {
        tipsCardText.text = _text;
    }


}
