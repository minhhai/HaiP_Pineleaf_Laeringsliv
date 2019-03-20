using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Big Badge, requested to blow up an achievement badge to a bigger version as an alternative to video
/// </summary>
public class BigBadge : MonoBehaviour
{

    public static BigBadge Instance;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    [SerializeField]
    private GameObject BigBadgeObject;

    [SerializeField]
    private Image BigBadgeImage;

    [SerializeField]
    private Text BigBadgeTitleText, BigBadgeTime, BigBadgeBodyText;

    [SerializeField]
    private GameObject URLButton;

    private string currentURL;

    public void SetValues(Sprite image, string name, string bodyText, string time, string url) {
        BigBadgeImage.sprite = image;
        BigBadgeTitleText.text = name;
        BigBadgeBodyText.text = bodyText;
        BigBadgeTime.text = time;
        if (url == "" || url == null) {
            URLButton.SetActive(false);
        } else {
            URLButton.SetActive(true);
            currentURL = url;
        }
        BigBadgeObject.SetActive(true);
    }

    public void ClickURLBUtton() {
        if (currentURL != null || currentURL != "")
            Application.OpenURL(currentURL);
    }

    public void Disable() {
        BigBadgeObject.SetActive(false);
    }

}
