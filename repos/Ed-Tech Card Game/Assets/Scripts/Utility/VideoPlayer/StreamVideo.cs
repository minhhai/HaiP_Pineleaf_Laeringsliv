using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StreamVideo : MonoBehaviour {

    public RawImage image;
    public VideoClip videoToPlay;

    private VideoPlayer videoPlayer;
    private VideoSource videoSource;
    private AudioSource audioSource;



	// Use this for initialization
	void Start () {
        Screen.orientation = ScreenOrientation.Landscape;
        Application.runInBackground = true;
        StartCoroutine(PlayVideo());
	}

    private bool isPlaying = true;

    public void ToggleVideo() {
        isPlaying = videoPlayer.isPlaying;
        if (isPlaying) {
            videoPlayer.Pause();
            audioSource.Pause();
        } else {
            videoPlayer.Play();
            audioSource.Play();
        }
        isPlaying = !isPlaying;

    }

    IEnumerator PlayVideo() {

        SoundController.Instance.ToggleMusic(false);

        //Add VideoPlayer to the GameObject
        videoPlayer = gameObject.AddComponent<VideoPlayer>();

        //Add AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();

        //Disable Play on Awake for both Video and Audio
        videoPlayer.playOnAwake = true;
        audioSource.playOnAwake = false;
        audioSource.Pause();

        //We want to play from video clip not from url

        videoPlayer.source = VideoSource.VideoClip;

        // Vide clip from Url
        //videoPlayer.source = VideoSource.Url;
        //videoPlayer.url = "";

        //Set Audio Output to AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //Assign the Audio from Video to AudioSource to be played
        videoPlayer.controlledAudioTrackCount = 1;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        //Set video To Play then prepare Audio to prevent Buffering
        videoPlayer.clip = videoToPlay;
        videoPlayer.Prepare();

        //Wait until video is prepared
        while (!videoPlayer.isPrepared) {
            yield return null;
        }

        Debug.Log("Done Preparing Video");

        //Assign the Texture from Video to RawImage to be displayed
        image.texture = videoPlayer.texture;

        // Wait until video is done
        while (true) {
            videoPlayer.loopPointReached += EndReached;
            yield return null;
        }
        
    }

    public void SkipVideo() {
        StopAllCoroutines();
        Continue();
    }


    public void Continue() {
        Screen.orientation = ScreenOrientation.Portrait;

        SoundController.Instance.ToggleMusic(true);
        transform.parent.gameObject.SetActive(false);
    }

    void EndReached(VideoPlayer vp) {
        Continue();
    }
}
