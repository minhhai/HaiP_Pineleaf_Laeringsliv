using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using LaeringslivCore;
using System;
using Newtonsoft.Json;

/// <summary>
/// Handles most stuff related to badges, aka achievements
/// </summary>
public class BadgeManager : MonoBehaviour {

    #region Singleton

    public static BadgeManager Instance;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    #endregion

    [SerializeField]
    private Transform BadgeListTransform;

    [SerializeField]
    private GameObject BadgeElementPrefab;

    [SerializeField]
    private List<BadgeInfoCapsule> EarnedBadgeList = new List<BadgeInfoCapsule> ();

    [SerializeField]
    private List<int> EarnedbadgeIdList;

    bool badgeManagerPrimed = false;

    //TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");


    #region Badge logic functions


    public void AddBadge(int badgeID) {
        if (!EarnedbadgeIdList.Contains(badgeID)) {
            List<BadgeCard> badgecardList = CardManager.Instance.GetBadgeCardList();
            for (int j = 0; j < badgecardList.Count; j++) {
                if (badgeID == badgecardList[j].BadgeCardId) {
                    AddBadge(badgecardList[j]);

                }
            }
        }
    }

    public void AddBadge(BadgeCard badgeCard) {
        DateTime timeStamp = DateTime.Now;
        GameObject newBadgeObject = Instantiate(BadgeElementPrefab, BadgeListTransform);
        BadgeInfoCapsule newBadgeInfoCapsule = new BadgeInfoCapsule {
            badgeID = badgeCard.BadgeCardId,
            badgeImageID = badgeCard.Image,
            badgeTitleText = badgeCard.TitleText,
            badgeBodyText = badgeCard.BodyText,
            videoUrl = badgeCard.VideoUrl,
            time = timeStamp

        };
        newBadgeObject.GetComponent<Badge>().SetValues(badgeCard.Image, badgeCard.TitleText, badgeCard.BodyText, badgeCard.VideoUrl, timeStamp);
        EarnedbadgeIdList.Add(badgeCard.BadgeCardId);
        EarnedBadgeList.Add(newBadgeInfoCapsule);
    }

    public void AddBadge(BadgeInfoCapsule badgeInfoCapsule) {
        GameObject newBadgeObject = Instantiate(BadgeElementPrefab, BadgeListTransform);
        newBadgeObject.GetComponent<Badge>().SetValues(badgeInfoCapsule.badgeImageID, badgeInfoCapsule.badgeTitleText, badgeInfoCapsule.badgeBodyText, badgeInfoCapsule.videoUrl, badgeInfoCapsule.time);
        EarnedBadgeList.Add(badgeInfoCapsule);

        Debug.Log("Added badge to List");
    }


    #endregion



    #region Save functions

    /// <summary>
    /// Save earned badges to the device
    /// </summary>
    /// <returns></returns>
    public void SaveEarnedBadgeList() {
        if (badgeManagerPrimed) {
            BinaryFormatter bf = new BinaryFormatter();
            string dataPath = "";
            dataPath = Application.persistentDataPath + "/EarnedBadgesList.txt";
            FileStream file = File.Open(dataPath, FileMode.OpenOrCreate);

            string earnedbadgesString = JsonConvert.SerializeObject(EarnedBadgeList, Formatting.Indented);
            bf.Serialize(file, earnedbadgesString);
            file.Close();

            print("Saved " + EarnedBadgeList.Count + " badges to file");
        }
        
    }
    
    /// <summary>
    /// Load earned badges list
    /// </summary>
    /// <returns></returns>
    public List<BadgeInfoCapsule> LoadEarnedBadgeList() {

        string dataPath = Application.persistentDataPath;

        dataPath = Application.persistentDataPath + "/EarnedBadgesList.txt";

        if (File.Exists(dataPath)) {
            
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open);
            string returnString = (string)bf.Deserialize(file);
            file.Close();
            List<BadgeInfoCapsule> badgeList = JsonConvert.DeserializeObject<List<BadgeInfoCapsule>>(returnString);

            try {
                if (badgeList == null) {
                    throw new Exception("Badgefile outdated or corrupted, generating new for now. Sorry about this.");
                }
                print("Loaded " + badgeList.Count + " badges from file");
                badgeManagerPrimed = true;
                return badgeList;
            } catch (Exception e) {
                File.Delete(dataPath);
                print(e);
                badgeManagerPrimed = true;
                return null;
            }
            
        } else {
            print("No previous list of badges exists.");
            badgeManagerPrimed = true;
            return null;
        }
    }

    /// <summary>
    /// Initialize badges from the stored BadgeInfoCapsule list
    /// </summary>
    public void SetBadges() {
        List<BadgeInfoCapsule> badgeList = LoadEarnedBadgeList();
        if (badgeList != null) {
            for (int i = 0; i < badgeList.Count; i++) {
                AddBadge(badgeList[i]);
            }
        }
        
    }

    public void DebugTestBadges() {
        List<BadgeCard> badgeCards = CardManager.Instance.GetBadgeCardList();
        foreach (BadgeCard card in badgeCards) {
            AddBadge(card);
        }


    }

    public void InitializeEarnedBadges() {


        List<BadgeInfoCapsule> earnedBadges = LoadEarnedBadgeList();

        Dictionary<String, int> gameIDs = new Dictionary<string, int>();

        List<int> badgeIDs = new List<int>();

        int counter = 0;


        if (earnedBadges != null && earnedBadges.Count > 0) {

            foreach (BadgeInfoCapsule badgeInfo in earnedBadges) {

                if (!badgeIDs.Contains(badgeInfo.badgeID)) {
                    AddBadge(badgeInfo);
                    badgeIDs.Add(badgeInfo.badgeID);
                    counter++;
                } 
            }

            //if (earnedBadges[0].gameID == null) {
            //    foreach (BadgeInfoCapsule badgeInfo in earnedBadges) {
            //        AddBadge(badgeInfo);
            //        counter++;
            //    }

            //}

            //else {
            //    foreach (BadgeInfoCapsule badgeInfo in earnedBadges) {

            //        if (!gameIDs.ContainsKey(badgeInfo.gameID)) {
            //            AddBadge(badgeInfo);
            //            gameIDs.Add(badgeInfo.gameID, badgeInfo.badgeID);
            //            counter++;
            //        } else {
            //            bool duplicate = false;
            //            foreach (KeyValuePair<string, int> bd in gameIDs) {
            //                if (badgeInfo.gameID == bd.Key && badgeInfo.badgeID == bd.Value) {
            //                    duplicate = true;
            //                    break;
            //                }
            //            }
            //            if (!duplicate) {
            //                AddBadge(badgeInfo);
            //                gameIDs.Add(badgeInfo.gameID, badgeInfo.badgeID);
            //                counter++;
            //            }
            //        }
            //    }

            //}



        }
        Debug.Log("Loaded " + counter + " badges from file");
    }


    #endregion
}
