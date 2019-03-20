using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Class for managing the saving of card collections
/// </summary>
public class CollectionSaveManager : MonoBehaviour {

    #region Singleton

    public static CollectionSaveManager Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    #endregion
    

    #region NewMethodsTesting


    public void SaveCollection (int collectionID, string collectionString) {
        BinaryFormatter bf = new BinaryFormatter();
        string dataPath = "";
        switch (collectionID) {
            case (1):
                dataPath = Application.persistentDataPath + "/BackupCardCollection1.txt";
                break;
            case (2):
                dataPath = Application.persistentDataPath + "/BackupCardCollection2.txt";
                break;
            case (3):
                dataPath = Application.persistentDataPath + "/BackupCardCollection3.txt";
                break;
            default:
                break;
        }
        print("Saved deck to path: " + dataPath);
        FileStream file = File.Open(dataPath, FileMode.OpenOrCreate);

        bf.Serialize(file, collectionString);
        file.Close();
    }

    public string LoadCollection (int collectionID) {

        string dataPath = Application.persistentDataPath;

        switch (collectionID) {
            case (1):
                dataPath += "/BackupCardCollection1.txt";
                break;
            case (2):
                dataPath += "/BackupCardCollection2.txt";
                break;
            case (3):
                dataPath += "/BackupCardCollection3.txt";
                break;
            default:
                break;
        }

        if (File.Exists(dataPath)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open);
            string returnString = (string)bf.Deserialize(file);
            file.Close();
            return returnString;
        } else {
            return "";
        }


        
    }
    
    #endregion


    #region Badge Saving

    List<BadgeInfoCapsule> earnedBadgesList;

    public void SaveEarnedBadges(List<BadgeInfoCapsule> earnedBadges) {
        BinaryFormatter bf = new BinaryFormatter();
        string dataPath = Application.persistentDataPath + "/EarnedBadges.txt";

        if (File.Exists(dataPath)) {
            FileStream file = File.Open(dataPath, FileMode.OpenOrCreate);

            bf.Serialize(file, earnedBadges);
            file.Close();
            Debug.Log("Saved Earned badges to path: " + dataPath);
        }
    }

    public List <BadgeInfoCapsule> LoadEarnedBadges() {
        string dataPath = Application.persistentDataPath + "/EarnedBadges.txt";

        if (File.Exists(dataPath)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open);
            List <BadgeInfoCapsule> returnEarnedBadges = (List<BadgeInfoCapsule>)bf.Deserialize(file);
            file.Close();
            return returnEarnedBadges;
        } else {
            Debug.Log("No file of badges found.");
            return null;
        }
    }



    #endregion

}
