using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager for the Journal
/// </summary>
public class Journal : MonoBehaviour {

    #region Singleton 
    public static Journal Instance;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    #endregion


    public float entryHeight = 120.0f;


    public List<GameObject> journalEntries = new List<GameObject>();

    #region Prefabs

    public GameObject journalEntryPrefab;

    #endregion


    #region Scene references
    [SerializeField]
    private Transform journalWindow;

    [SerializeField]
    private Transform insertPosition;

    [SerializeField]
    InputField HeaderField;
    [SerializeField]
    InputField BodyField;
    #endregion



    public enum JournalEntrySymbol {
        Blue,
        Green,
        Red,
        Yellow
    }

    public JournalEntrySymbol selectedSymbol = JournalEntrySymbol.Blue;

    /// <summary>
    /// Remove an entry from the journal list
    /// </summary>
    public void RemoveEntry() {

    }

    /// <summary>
    /// Insert new entry into the journal list
    /// </summary>
    public void InsertEntry() {

        GameObject newEntry = Instantiate(journalEntryPrefab, journalWindow);
        journalEntries.Add(newEntry);
        newEntry.transform.SetSiblingIndex(1);
        newEntry.GetComponent<JournalEntry>().SetValues(HeaderField.text, BodyField.text, 0);
        HeaderField.text = "";
        BodyField.text = "";
    }

    public void Update() {

    }


}
