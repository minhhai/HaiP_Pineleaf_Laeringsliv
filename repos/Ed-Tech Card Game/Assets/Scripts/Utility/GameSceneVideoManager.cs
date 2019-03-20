using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VideoHelper;

public class GameSceneVideoManager : MonoBehaviour {

    public static GameSceneVideoManager Instance;

    public VideoController GameSceneVideoController;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    public void PlayVideoClip(string url) {
        Screen.orientation = ScreenOrientation.Landscape;
        GameSceneVideoController.gameObject.SetActive(true);
        GameSceneVideoController.PrepareForUrl(url);
    }

    public void EndOfVideo() {
        Screen.orientation = ScreenOrientation.Portrait;
        GameSceneVideoController.gameObject.SetActive(false);
        GameManager.Instance.NextCard();
    }

    public void SkipVideo() {
        EndOfVideo();
    }
}
