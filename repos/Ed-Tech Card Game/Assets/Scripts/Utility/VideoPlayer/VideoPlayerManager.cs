using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour {
    
    public VideoPlayer videoPlayer;


    public void PlayVideo() {
        videoPlayer.Play();
    }

    public void PauseVideo() {
        videoPlayer.Pause();
    }

    bool isPlaying = false;

    public void ToggleVideo() {
        isPlaying =  videoPlayer.isPlaying;
        if (isPlaying) videoPlayer.Pause();
        else videoPlayer.Play();
        isPlaying = !isPlaying;
        
    }

}
