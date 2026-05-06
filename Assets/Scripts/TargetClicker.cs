using UnityEngine;

public class TargetClick : MonoBehaviour
{
    public ScoreManager scoreManager;
    public Game1Manager game1Manager;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void OnMouseDown()
    {
        if (game1Manager == null)
        {
            Debug.LogError("Game1Manageris not connected!");
            return;
        }

        if (game1Manager.CanScore())
        {
            if (scoreManager != null)
                scoreManager.AddScore(1);

            game1Manager.RegisterSuccessfulClick();
            MoveToRandomPosition();

            Debug.Log("Clicked while focused: +1 score");
        }
        else
        {
            Debug.Log("Clicked while NOT focused: no score");
            ShowNoScoreFeedback();
        }
    }

    void MoveToRandomPosition()
    {
        float randomX = Random.Range(-1.6f, 1.6f);
        float randomY = Random.Range(-3.2f, 2.2f);

        transform.position = new Vector3(randomX, randomY, 0f);
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