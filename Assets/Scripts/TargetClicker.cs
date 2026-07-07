using UnityEngine;
using UnityEngine.EventSystems;

public class TargetClicker : MonoBehaviour, IPointerClickHandler
{
    public ScoreManager scoreManager;
    public Game1Manager game1Manager;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("TARGET POINTER CLICKED");

        if (game1Manager == null)
        {
            Debug.LogError("TargetClicker: Game1Manager is not connected!");
            return;
        }

        if (game1Manager.IsGamePausedForCalibration())
        {
            Debug.Log("Clicked during calibration: ignored.");
            return;
        }

        PlayMysterySound();

        if (game1Manager.CanScore())
        {
            if (scoreManager != null)
            {
                scoreManager.AddScore(1);
            }
            else
            {
                Debug.LogWarning("TargetClicker: ScoreManager is not connected!");
            }

            game1Manager.RegisterSuccessfulClick();
            MoveToRandomPosition();

            Debug.Log("Clicked while focused: +1 score");
        }
        else
        {
            Debug.Log("Clicked while not focused or game paused: no score");
            ShowNoScoreFeedback();
        }
    }

    void PlayMysterySound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMysteryAlert();
        }
    }

    void MoveToRandomPosition()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            float randomX = Random.Range(-350f, 350f);
            float randomY = Random.Range(-450f, 250f);

            rectTransform.anchoredPosition = new Vector2(randomX, randomY);
        }
        else
        {
            float randomX = Random.Range(-1.6f, 1.6f);
            float randomY = Random.Range(-3.2f, 2.2f);

            transform.position = new Vector3(randomX, randomY, 0f);
        }
    }

    void ShowNoScoreFeedback()
    {
        transform.localScale = originalScale * 0.9f;
        CancelInvoke(nameof(ResetScale));
        Invoke(nameof(ResetScale), 0.1f);
    }

    void ResetScale()
    {
        transform.localScale = originalScale;
    }
}