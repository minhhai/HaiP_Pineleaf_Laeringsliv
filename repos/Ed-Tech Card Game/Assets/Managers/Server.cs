using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LaeringslivCore;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

/// <summary>
/// Class for communicating with the server
/// </summary>
public class Server : MonoBehaviour
{ 
    public static Server Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public string playerID = "0";

    public void SetPlayerID(string newID) {
        playerID = newID;
    }

    public string GetPlayerID() {
        return playerID;
    }

    public int GameID = 0;

    public void SetGameID(int newGameID) {
        GameID = newGameID;
    }

    public string OrganizationID = "Default Organization";

    public void SetOrganizationID(string newOrgID) {
        OrganizationID = newOrgID;
    }

    public int DeviceID = 0;

    public void SetDeviceID(int newID) {
        DeviceID = newID;
    }



    public static void LogEvent(EventLog eventLog)
    {
        Instance.StartCoroutine(Instance.SendEvent(eventLog));
    }

    // Old, not used atm
    private IEnumerator SendEvent(EventLog eventLog)
    {
        EventForm eventForm = new EventForm();
        
        eventForm.AddField("PlayerID", int.Parse(playerID));
        eventForm.AddField("Time", eventLog.Time);
        eventForm.AddField("Event", (int)eventLog.Event);
        eventForm.AddField("CardID", eventLog.CardID);
        eventForm.AddField("Choice", eventLog.Choice + 1); // Iterate by one for readability on server
        
        string postURL = "https://laeringsliv.azurewebsites.net/PlayerEvents/Create";
        UnityWebRequest www = UnityWebRequest.Post(postURL, eventForm);

        yield return www.SendWebRequest();
        
        Debug.Log($"Log sent: {string.Concat(www.GetResponseHeaders().Select(x => $"{x.Key} {x.Value} \n ")) }");
        
        
    }

    class EventForm : WWWForm
    {
        
    }


    private List<EventLog> eventBundle = new List<EventLog>();
    private int eventBundleSendPoint = 20;


    public void BundleEvent(EventLog eventLog) 
    {
        eventBundle.Add(eventLog);
        if (eventBundle.Count >= eventBundleSendPoint) {
            StartCoroutine(SendEventBundle(new List<EventLog> (eventBundle)));
            eventBundle.Clear();
        }
    }

    private IEnumerator SendEventBundle(List<EventLog> _eventBundle ) 
    {
        int counter = 0;
        foreach(EventLog e in _eventBundle) {

            EventForm eventForm = new EventForm();

            eventForm.AddField("PlayerID", playerID);
            eventForm.AddField("OrganizationID", OrganizationID);
            eventForm.AddField("GameID", GameID);
            eventForm.AddField("DeviceID", DeviceID);

            eventForm.AddField("Time", e.Time);
            eventForm.AddField("Event", (int)e.Event);
            eventForm.AddField("CardID", e.CardID);
            int newChoice = e.Choice + 1;               // Iterate by one for readability on server
            eventForm.AddField("Choice", newChoice);


            string totalScores = JsonConvert.SerializeObject(PlayerManager.Instance.GetSubStatsList());
            eventForm.AddField("TotalScoreList", totalScores);

            string postURL = "https://laeringsliv.azurewebsites.net/PlayerEvents/Create";
            UnityWebRequest www = UnityWebRequest.Post(postURL, eventForm);

            yield return www.SendWebRequest();

            counter++;
        }
        Debug.Log(counter + " logs successfully sent to server.");
        //Debug.Log($"Log sent: {string.Concat(www.GetResponseHeaders().Select(x => $"{x.Key} {x.Value} \n ")) }");



    }

    public void ForceSendBundle() {
        StartCoroutine(SendEventBundle(new List<EventLog>(eventBundle)));
        eventBundle.Clear();
    }

    // TODO: Currently not in use, event forms and Json don't play well with nested Lists
    private IEnumerator NewSendEventBundle(List<EventLog> _eventBundle) {
        int counter = 0;

        EventForm eventFormBundle = new EventForm();
        eventFormBundle.AddField("PlayerID", int.Parse(playerID));
        eventFormBundle.AddField("FormCounter", _eventBundle.Count);

        List<EventForm> eventFormList = new List<EventForm>();

        foreach (EventLog e in _eventBundle) {

            EventForm eventForm = new EventForm();
            eventForm.AddField("Time", e.Time);
            eventForm.AddField("Event", (int)e.Event);
            eventForm.AddField("CardID", e.CardID);
            eventForm.AddField("Choice", e.Choice);

            eventFormList.Add(eventForm);

            eventFormBundle.AddField("EventEntry" + counter, JsonUtility.ToJson(eventForm));

            counter++;
        }

        string postURL = "https://laeringsliv.azurewebsites.net/PlayerEvents/Create";

        EventForm stringBundle = new EventForm();
        stringBundle.AddField("EventString", JsonUtility.ToJson(eventFormBundle));


        UnityWebRequest www = UnityWebRequest.Post(postURL, stringBundle);

        yield return www.SendWebRequest();

        Debug.Log(counter + " logs successfully sent to server.");
    }


}
