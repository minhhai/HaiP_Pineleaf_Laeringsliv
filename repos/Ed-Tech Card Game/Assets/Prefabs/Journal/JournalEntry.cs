using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Prefab for a new Journal Entry
/// </summary>
public class JournalEntry : MonoBehaviour {


    [SerializeField]
    private Text entryHeader;
    [SerializeField]
    private Text entryBody;

    

    public void SetValues (string headerText, string bodyText, int iconID) {
        entryHeader.text = headerText;
        entryBody.text = bodyText;
        switch (iconID) {
            case (0):

                break;
            case (1):

                break;
            case (2):

                break;
            case (3):

                break;
            default:
                break;
        }
    }


    [SerializeField]
    private Text date;

    [SerializeField]
    private Image Icon;

}
