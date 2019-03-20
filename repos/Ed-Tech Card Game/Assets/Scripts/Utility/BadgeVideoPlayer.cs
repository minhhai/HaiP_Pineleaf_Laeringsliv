using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VideoHelper;
using UnityEngine.Video;

/// <summary>
/// Class for handling playing videos from badges
/// </summary>
public class BadgeVideoPlayer : MonoBehaviour {

    public static BadgeVideoPlayer Instance;

    /// <summary>
    /// Reference to the video controller
    /// </summary>
    public VideoController BadgeVideoController;


    public VideoClip testClip;


    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    public void PlayVideoClip(string url) {
        Screen.orientation = ScreenOrientation.Landscape;
        BadgeVideoController.gameObject.SetActive(true);
        //BadgeVideoController.PrepareForClip(testClip); //TODO: Just for testing, remove
        BadgeVideoController.PrepareForUrl(url);
    }

    public void EndVideoClip() {
        Screen.orientation = ScreenOrientation.Portrait;
        BadgeVideoController.gameObject.SetActive(false);
        
    }


}
