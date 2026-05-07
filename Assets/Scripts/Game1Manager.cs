using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Game1Manager : MonoBehaviour
{
    public Button playAgainButton;

    private float lastSavedFocusedTime = 0f;
    private float lastSavedUnfocusedTime = 0f;

    private int lastSavedFocusedCorrectClicks = 0;
    private int lastSavedFocusedWrongClicks = 0;
    private int lastSavedUnfocusedCorrectClicks = 0;
    private int lastSavedUnfocusedWrongClicks = 0;

    public float gameDuration = 30f;
    private float remainingTime;

    public TMP_Text timerText;
    public TMP_Text attentionText;

    public ScoreManager scoreManager;

    [Header("Target Bounce Effect")]
    public RectTransform targetRect;
    public float bounceDuration = 0.25f;
    public float bounceScale = 1.25f;

    private bool targetIsBouncing = false;
    private Vector3 targetOriginalScale;

    private int focusedCorrectClicks = 0;
    private int focusedWrongClicks = 0;

    private int unfocusedCorrectClicks = 0;
    private int unfocusedWrongClicks = 0;

    private float focusedTime = 0f;
    private float unfocusedTime = 0f;
    private bool gameEnded = false;

    private float saveTimer = 0f;

    void Start()
    {
        if (playAgainButton != null)
            playAgainButton.gameObject.SetActive(false);

        if (targetRect != null)
            targetOriginalScale = targetRect.localScale;

        remainingTime = gameDuration;
        UpdateAttentionText();
    }

    void Update()
    {
        if (gameEnded) return;

        remainingTime -= Time.deltaTime;

        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(remainingTime);

        bool isFocused = GetFocusState();

        if (isFocused)
            focusedTime += Time.deltaTime;
        else
            unfocusedTime += Time.deltaTime;

        UpdateAttentionText();

        saveTimer += Time.deltaTime;

        if (saveTimer >= 1f)
        {
            SaveCurrentGame1Data();
            saveTimer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveCurrentGame1Data();
        }

        if (remainingTime <= 0)
        {
            EndGame();
        }
    }

    bool GetFocusState()
    {
    if (AttentionSystemManager.Instance != null)
    {
        return AttentionSystemManager.Instance.GetFocusState();
    }

    if (FakeAttentionManager.Instance != null)
    {
        return FakeAttentionManager.Instance.GetFocus();
    }

    return true;
    }
  void UpdateAttentionText()
  {
    if (attentionText == null) return;

    bool isFocused = GetFocusState();
    attentionText.text = isFocused
    ? "✓ Focused"
    : "! Not Focused";
    }

    public bool CanScore()
   {
        bool isFocused = GetFocusState();

        if (!isFocused)
        {
            unfocusedCorrectClicks++;
            SaveCurrentGame1Data();

            Debug.Log("Game1 unfocused correct click counted.");
            return false;
        }

        return true;
    }

    public void RegisterSuccessfulClick()
    {
        if (gameEnded) return;
        if (targetIsBouncing) return;

        focusedCorrectClicks++;
        SaveCurrentGame1Data();

        StartCoroutine(BounceTarget());
    }

    IEnumerator BounceTarget()
    {
        if (targetRect == null)
            yield break;

        targetIsBouncing = true;

        Vector3 startScale = targetOriginalScale;
        Vector3 bigScale = targetOriginalScale * bounceScale;

        float halfDuration = bounceDuration / 2f;
        float timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            targetRect.localScale = Vector3.Lerp(startScale, bigScale, t);
            yield return null;
        }

        timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            targetRect.localScale = Vector3.Lerp(bigScale, startScale, t);
            yield return null;
        }

        targetRect.localScale = startScale;
        targetIsBouncing = false;
    }

    public void RegisterWrongClick()
    {
        if (gameEnded) return;
        if (IsPointerOverUI())
        {
            return;
        }

        bool isFocused = GetFocusState();

        if (isFocused)
        {
            focusedWrongClicks++;
            Debug.Log("Game1 focused wrong click counted.");
        }
        else
        {
            unfocusedWrongClicks++;
            Debug.Log("Game1 unfocused wrong click counted.");
        }

        SaveCurrentGame1Data();
    }

    public bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        return EventSystem.current.IsPointerOverGameObject();
    }

    void EndGame()
    {
        if (playAgainButton != null)
            playAgainButton.gameObject.SetActive(true);

        gameEnded = true;
        SaveCurrentGame1Data();

        Debug.Log("=== GAME 1 SESSION SUMMARY ===");
        Debug.Log("Score: " + GetFinalScore());
        Debug.Log("Focused Correct Clicks: " + focusedCorrectClicks);
        Debug.Log("Focused Wrong Clicks: " + focusedWrongClicks);
        Debug.Log("Unfocused Correct Clicks: " + unfocusedCorrectClicks);
        Debug.Log("Unfocused Wrong Clicks: " + unfocusedWrongClicks);
        Debug.Log("Focused Time: " + focusedTime.ToString("F2"));
        Debug.Log("Unfocused Time: " + unfocusedTime.ToString("F2"));
    }

    int GetFinalScore()
    {
        if (scoreManager != null)
            return scoreManager.score;

        return focusedCorrectClicks;
    }

    void SaveCurrentGame1Data()
    {
        float focusedDelta = focusedTime - lastSavedFocusedTime;
        float unfocusedDelta = unfocusedTime - lastSavedUnfocusedTime;

        int focusedCorrectDelta = focusedCorrectClicks - lastSavedFocusedCorrectClicks;
        int focusedWrongDelta = focusedWrongClicks - lastSavedFocusedWrongClicks;
        int unfocusedCorrectDelta = unfocusedCorrectClicks - lastSavedUnfocusedCorrectClicks;
        int unfocusedWrongDelta = unfocusedWrongClicks - lastSavedUnfocusedWrongClicks;

        float durationDelta = focusedDelta + unfocusedDelta;

        if (durationDelta <= 0f &&
            focusedCorrectDelta == 0 &&
            focusedWrongDelta == 0 &&
            unfocusedCorrectDelta == 0 &&
            unfocusedWrongDelta == 0)
            return;

        ReportDataManager.SaveGameReport(
            "Game1",
            durationDelta,
            focusedDelta,
            unfocusedDelta,
            focusedCorrectDelta,
            focusedWrongDelta,
            unfocusedCorrectDelta,
            unfocusedWrongDelta,
            focusedCorrectDelta,
            focusedWrongDelta,
            0,
            0f,
            0
        );

        lastSavedFocusedTime = focusedTime;
        lastSavedUnfocusedTime = unfocusedTime;
        lastSavedFocusedCorrectClicks = focusedCorrectClicks;
        lastSavedFocusedWrongClicks = focusedWrongClicks;
        lastSavedUnfocusedCorrectClicks = unfocusedCorrectClicks;
        lastSavedUnfocusedWrongClicks = unfocusedWrongClicks;
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveCurrentGame1Data();
    }

    void OnApplicationQuit()
    {
        SaveCurrentGame1Data();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}