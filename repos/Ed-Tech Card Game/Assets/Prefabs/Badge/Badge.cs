using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// In-game representation of a badge, aka achievement
/// </summary>
public class Badge : MonoBehaviour {

    public Image BadgeIcon;

    public Text BadgeName;
    
    private string BadgeBody;

    public Text BadgeTimeStamp;

    private string videoURL = "";

    //TimeZone zone = TimeZone.CurrentTimeZone;


    //public void Awake() {
    //    try {
    //        zone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
    //    } catch (TimeZoneNotFoundException) {
    //        zone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo");
    //    }
    //}


    public void SetValues(string ImageID, string badgeName, string badgeBody, string url, DateTime timeStamp) {
        BadgeIcon.sprite = Resources.Load<Sprite>("Images/" + ImageID);
        BadgeName.text = badgeName;
        BadgeBody = badgeBody;

        BadgeTimeStamp.text = timeStamp.ToString("dd/MM/yyyy HH:mm:ss");

        videoURL = url;
    }

    public void SetValues(string ImageID, string badgeName, string badgeBody, string url) {
        Sprite imageSprite = Resources.Load<Sprite>("Images/" + ImageID);
        if (imageSprite != null) {
            BadgeIcon.sprite = imageSprite;
        } else {
            BadgeIcon.sprite = Resources.Load<Sprite>("Images/QuestionMark");
        }
        
        BadgeName.text = badgeName;
        BadgeBody = badgeBody;

        DateTime timeStamp = DateTime.Now;

        //DateTime timeStamp = TimeZone.ConvertTimeFromUtc(DateTime.UtcNow, zone);

        BadgeTimeStamp.text = timeStamp.ToString("dd/MM/yyyy HH:mm:ss");

        videoURL = url;
    }

    public void PlayBadgeVideo() {
        BadgeVideoPlayer.Instance.PlayVideoClip(videoURL);
    }

    public void DisplayBigBadge(string url) {
        BigBadge.Instance.SetValues(BadgeIcon.sprite, BadgeName.text, BadgeBody, BadgeTimeStamp.text, url);
    }

    public void BadgeClick () {
        
            DisplayBigBadge(videoURL);
            //Application.OpenURL(videoURL);
    }
}
