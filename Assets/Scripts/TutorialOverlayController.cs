using UnityEngine;
using UnityEngine.Video;

public class TutorialOverlayController : MonoBehaviour
{
    [Header("UI")]
    public GameObject tutorialOverlay;

    [Header("Video")]
    public VideoPlayer videoPlayer;

    void Start()
    {
        ShowTutorial();
    }

    void ShowTutorial()
    {
        if (tutorialOverlay != null)
            tutorialOverlay.SetActive(true);

        Time.timeScale = 0f;

        if (videoPlayer != null)
            videoPlayer.Play();
    }

    public void OnOkButtonClicked()
    {
        if (videoPlayer != null)
            videoPlayer.Stop();

        if (tutorialOverlay != null)
            tutorialOverlay.SetActive(false);

        Time.timeScale = 1f;
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}