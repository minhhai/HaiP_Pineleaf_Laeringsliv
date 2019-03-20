using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple sound controller
/// </summary>
public class SoundController : MonoBehaviour {

    public static SoundController Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    [SerializeField]
    private AudioSource musicPlayer;


    public void ToggleMusic(bool toggle) {
        if (toggle) {
            musicPlayer.Play();
        } else {
            musicPlayer.Pause();
        }
    }


}
