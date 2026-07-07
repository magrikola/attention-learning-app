using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Game1Manager : MonoBehaviour
{
    public Button playAgainButton;

    [Header("Result Popup")]
    public GameObject resultPopup;
    public TMP_Text resultTitleText;
    public GameObject targetObject;

    private float lastSavedFocusedTime = 0f;
    private float lastSavedUnfocusedTime = 0f;

    private int lastSavedFocusedCorrectClicks = 0;
    private int lastSavedFocusedWrongClicks = 0;
    private int lastSavedUnfocusedCorrectClicks = 0;
    private int lastSavedUnfocusedWrongClicks = 0;

    [Header("Game Settings")]
    public float gameDuration = 30f;
    private float remainingTime;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text attentionText;
    public ScoreManager scoreManager;

    [Header("Calibration Pause")]
    public ARFaceHeadPoseInput faceHeadPoseInput;

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
        gameEnded = false;
        remainingTime = gameDuration;

        if (playAgainButton != null)
            playAgainButton.gameObject.SetActive(false);

        if (resultPopup != null)
            resultPopup.SetActive(false);

        if (targetObject != null)
            targetObject.SetActive(true);

        if (targetRect != null)
            targetOriginalScale = targetRect.localScale;

        if (faceHeadPoseInput == null)
            faceHeadPoseInput = FindObjectOfType<ARFaceHeadPoseInput>();

        UpdateTimerText();
        UpdateAttentionText();
    }

    void Update()
    {
        if (gameEnded)
            return;

        if (IsGamePausedForCalibration())
        {
            UpdateTimerText();
            UpdateAttentionText();
            return;
        }

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            UpdateTimerText();
            EndGame();
            return;
        }

        UpdateTimerText();

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
    }

    public bool IsGamePausedForCalibration()
    {
        if (faceHeadPoseInput == null)
            faceHeadPoseInput = FindObjectOfType<ARFaceHeadPoseInput>();

        if (faceHeadPoseInput == null)
            return false;

        return faceHeadPoseInput.IsCalibrationBlockingGameplay;
    }

    bool GetFocusState()
    {
        if (AttentionSystemManager.Instance != null)
            return AttentionSystemManager.Instance.GetFocusState();

        if (FakeAttentionManager.Instance != null)
            return FakeAttentionManager.Instance.GetFocus();

        return true;
    }

    void UpdateTimerText()
    {
        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(remainingTime);
    }

    void UpdateAttentionText()
    {
        if (attentionText == null)
            return;

        if (IsGamePausedForCalibration())
        {
            attentionText.text = "Calibrating...";
            return;
        }

        bool isFocused = GetFocusState();

        attentionText.text = isFocused
            ? "Focused"
            : "Not Focused";
    }

    public bool CanScore()
    {
        if (gameEnded)
            return false;

        if (IsGamePausedForCalibration())
            return false;

        bool isFocused = GetFocusState();

        if (!isFocused)
        {
            unfocusedCorrectClicks++;
            StartCoroutine(PlayWrongSoundDelayed());
            SaveCurrentGame1Data();

            Debug.Log("Game1 unfocused correct click counted. No score.");
            return false;
        }

        return true;
    }

    public void RegisterSuccessfulClick()
    {
        if (gameEnded)
            return;

        if (IsGamePausedForCalibration())
            return;

        if (targetIsBouncing)
            return;

        focusedCorrectClicks++;
        SaveCurrentGame1Data();

        StartCoroutine(PlayCorrectSoundDelayed());
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
        if (gameEnded)
            return;

        if (IsGamePausedForCalibration())
            return;

        PlayWrongSound();

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
        return false;
    }

    void PlayCorrectSound()
    {
        Debug.Log("Game1Manager: PlayCorrectSound çağrıldı.");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCorrect();
        }
        else
        {
            Debug.LogWarning("Game1Manager: AudioManager.Instance NULL!");
        }
    }

    IEnumerator PlayCorrectSoundDelayed()
    {
        yield return new WaitForSeconds(0.12f);
        PlayCorrectSound();
    }

    void PlayWrongSound()
    {
        Debug.Log("Game1Manager: PlayWrongSound çağrıldı.");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayWrong();
        }
        else
        {
            Debug.LogWarning("Game1Manager: AudioManager.Instance NULL!");
        }
    }

    IEnumerator PlayWrongSoundDelayed()
    {
        yield return new WaitForSeconds(0.12f);
        PlayWrongSound();
    }

    void EndGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;
        remainingTime = 0f;

        UpdateTimerText();
        UpdateAttentionText();

        if (targetObject != null)
            targetObject.SetActive(false);

        if (resultPopup != null)
        {
            resultPopup.SetActive(true);
            resultPopup.transform.SetAsLastSibling();
        }

        if (resultTitleText != null)
            resultTitleText.text = "Session Complete";

        if (playAgainButton != null)
            playAgainButton.gameObject.SetActive(true);

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

    public void BackToGames()
    {
        SceneManager.LoadScene("MainMenu");
    }
}