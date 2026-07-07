using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game3Manager : MonoBehaviour
{
    public Button playAgainButton;

    [Header("Result Popup")]
    public GameObject resultPopup;
    public TMP_Text resultTitleText;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text attentionText;
    public TMP_Text reactionInfoText;
    public TMP_Text missedText;

    [Header("Target")]
    public RectTransform reactionTarget;

    [Tooltip("Boş bırakırsan ReactionTarget'ın parent RectTransform'u spawn alanı olarak kullanılır.")]
    public RectTransform spawnArea;

    [Header("Target Spawn Area")]
    public float edgePaddingX = 120f;
    public float edgePaddingY = 220f;

    [Header("Game Stats")]
    public int score = 0;
    public int missedCount = 0;

    [Header("Game Settings")]
    public float gameDuration = 30f;
    public float targetVisibleTime = 1.0f;

    private float remainingTime;
    private float targetAppearTime;
    private bool targetActive = false;
    private bool gameEnded = false;
    private bool wasCalibrating = false;

    private float focusedTime = 0f;
    private float unfocusedTime = 0f;

    private int focusedCorrectClicks = 0;
    private int focusedWrongClicks = 0;

    private int unfocusedCorrectClicks = 0;
    private int unfocusedWrongClicks = 0;

    private float totalReactionTime = 0f;
    private int reactionCount = 0;

    private float saveTimer = 0f;

    private float lastSavedFocusedTime = 0f;
    private float lastSavedUnfocusedTime = 0f;
    private int lastSavedFocusedCorrectClicks = 0;
    private int lastSavedFocusedWrongClicks = 0;
    private int lastSavedUnfocusedCorrectClicks = 0;
    private int lastSavedUnfocusedWrongClicks = 0;
    private int lastSavedMissedCount = 0;
    private float lastSavedReactionTime = 0f;
    private int lastSavedReactionCount = 0;

    void Start()
    {
        if (playAgainButton != null)
            playAgainButton.gameObject.SetActive(false);

        if (resultPopup != null)
            resultPopup.SetActive(false);

        remainingTime = gameDuration;

        if (reactionInfoText != null)
            reactionInfoText.text = "";

        HideTarget();
        UpdateUI();
        UpdateAttentionText();

        Invoke(nameof(ShowTarget), 1f);
    }

    void Update()
    {
        if (gameEnded) return;

        if (IsAttentionSystemBusy())
        {
            wasCalibrating = true;

            UpdateAttentionText();

            if (timerText != null)
                timerText.text = "Calibrating...";

            if (reactionInfoText != null)
                reactionInfoText.text = "Calibrating...";

            HideTarget();
            CancelInvoke(nameof(ShowTarget));

            return;
        }

        if (wasCalibrating)
        {
            wasCalibrating = false;

            if (reactionInfoText != null)
                reactionInfoText.text = "";

            HideTarget();
            CancelInvoke(nameof(ShowTarget));
            Invoke(nameof(ShowTarget), 0.5f);
        }

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
            SaveCurrentGame3Data();
            saveTimer = 0f;
        }

        if (targetActive && Time.time - targetAppearTime > targetVisibleTime)
        {
            MissTarget();
        }

        if (remainingTime <= 0f)
        {
            EndGame();
        }
    }

    bool GetFocusState()
    {
        if (AttentionSystemManager.Instance != null)
            return AttentionSystemManager.Instance.GetFocusState();

        if (FakeAttentionManager.Instance != null)
            return FakeAttentionManager.Instance.GetFocus();

        return true;
    }

    bool IsAttentionSystemBusy()
    {
        ARFaceHeadPoseInput input = FindObjectOfType<ARFaceHeadPoseInput>();
        return input != null && input.IsCalibrationBlockingGameplay;
    }

    void UpdateAttentionText()
    {
        if (attentionText == null) return;

        if (IsAttentionSystemBusy())
        {
            attentionText.text = "Calibrating...";
            return;
        }

        attentionText.text = GetFocusState()
            ? "Focused"
            : "Not Focused";
    }

    void ShowTarget()
    {
        if (gameEnded) return;
        if (IsAttentionSystemBusy()) return;
        if (reactionTarget == null) return;

        RectTransform area = spawnArea;

        if (area == null)
            area = reactionTarget.parent as RectTransform;

        if (area == null)
        {
            Debug.LogWarning("Game3Manager: Spawn area bulunamadı.");
            return;
        }

        Rect rect = area.rect;

        float halfTargetWidth = reactionTarget.rect.width * 0.5f * reactionTarget.localScale.x;
        float halfTargetHeight = reactionTarget.rect.height * 0.5f * reactionTarget.localScale.y;

        float minX = rect.xMin + edgePaddingX + halfTargetWidth;
        float maxX = rect.xMax - edgePaddingX - halfTargetWidth;

        float minY = rect.yMin + edgePaddingY + halfTargetHeight;
        float maxY = rect.yMax - edgePaddingY - halfTargetHeight;

        if (minX > maxX)
        {
            minX = rect.xMin + halfTargetWidth;
            maxX = rect.xMax - halfTargetWidth;
        }

        if (minY > maxY)
        {
            minY = rect.yMin + halfTargetHeight;
            maxY = rect.yMax - halfTargetHeight;
        }

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        reactionTarget.anchoredPosition = new Vector2(randomX, randomY);
        reactionTarget.gameObject.SetActive(true);

        targetAppearTime = Time.time;
        targetActive = true;
    }

    void HideTarget()
    {
        if (reactionTarget != null)
            reactionTarget.gameObject.SetActive(false);

        targetActive = false;
    }

    public void TargetClicked()
    {
        if (!targetActive || gameEnded) return;
        if (IsAttentionSystemBusy()) return;

        PlayMysterySound();

        bool isFocused = GetFocusState();
        float reactionTime = Time.time - targetAppearTime;

        if (isFocused)
        {
            StartCoroutine(PlayCorrectSoundDelayed());

            score++;
            focusedCorrectClicks++;

            totalReactionTime += reactionTime;
            reactionCount++;

            if (reactionInfoText != null)
                reactionInfoText.text = "Reaction: " + reactionTime.ToString("F2") + "s";

            Debug.Log("Game3 clicked while focused: +1 score");
        }
        else
        {
            unfocusedCorrectClicks++;
            StartCoroutine(PlayWrongSoundDelayed());

            if (reactionInfoText != null)
                reactionInfoText.text = "Unfocused correct click";

            Debug.Log("Game3 unfocused correct click counted: no score");
        }

        UpdateUI();
        SaveCurrentGame3Data();

        StartCoroutine(TargetClickedAfterBounce());
    }

    IEnumerator TargetClickedAfterBounce()
    {
        targetActive = false;

        if (reactionTarget != null)
        {
            BounceEffectClicker effect = reactionTarget.GetComponent<BounceEffectClicker>();

            if (effect != null)
                effect.PlayEffect();
        }

        yield return new WaitForSeconds(0.20f);

        HideTarget();

        if (!gameEnded && !IsAttentionSystemBusy())
        {
            float nextDelay = Random.Range(0.8f, 1.8f);
            Invoke(nameof(ShowTarget), nextDelay);
        }
    }

    public void RegisterWrongClick()
    {
        if (gameEnded) return;
        if (IsAttentionSystemBusy()) return;

        PlayWrongSound();

        bool isFocused = GetFocusState();

        if (isFocused)
        {
            focusedWrongClicks++;
            Debug.Log("Game3 focused wrong click counted.");
        }
        else
        {
            unfocusedWrongClicks++;
            Debug.Log("Game3 unfocused wrong click counted.");
        }

        SaveCurrentGame3Data();
    }

    void MissTarget()
    {
        if (gameEnded) return;
        if (IsAttentionSystemBusy()) return;

        PlayWrongSound();

        bool isFocused = GetFocusState();

        if (isFocused)
        {
            missedCount++;

            if (reactionInfoText != null)
                reactionInfoText.text = "Missed!";
        }
        else
        {
            if (reactionInfoText != null)
                reactionInfoText.text = "Target expired while not focused";
        }

        HideTarget();
        UpdateUI();
        SaveCurrentGame3Data();

        if (!gameEnded && !IsAttentionSystemBusy())
        {
            float nextDelay = Random.Range(0.6f, 1.4f);
            Invoke(nameof(ShowTarget), nextDelay);
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (missedText != null)
            missedText.text = "Missed: " + missedCount;
    }

    void PlayMysterySound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMysteryAlert();
    }

    void PlayCorrectSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCorrect();
    }

    void PlayWrongSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayWrong();
    }

    IEnumerator PlayCorrectSoundDelayed()
    {
        yield return new WaitForSeconds(0.12f);
        PlayCorrectSound();
    }

    IEnumerator PlayWrongSoundDelayed()
    {
        yield return new WaitForSeconds(0.12f);
        PlayWrongSound();
    }

    public void PlayAgain()
    {
        SaveCurrentGame3Data();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void EndGame()
    {
        if (gameEnded) return;

        gameEnded = true;

        if (playAgainButton != null)
            playAgainButton.gameObject.SetActive(true);

        if (resultPopup != null)
            resultPopup.SetActive(true);

        if (resultTitleText != null)
            resultTitleText.text = "Session Complete";

        if (reactionInfoText != null)
            reactionInfoText.text = "Game Over";

        SaveCurrentGame3Data();

        HideTarget();
        CancelInvoke();

        Debug.Log("=== GAME 3 SUMMARY ===");
        Debug.Log("Score: " + score);
        Debug.Log("Missed: " + missedCount);
        Debug.Log("Focused Correct Clicks: " + focusedCorrectClicks);
        Debug.Log("Focused Wrong Clicks: " + focusedWrongClicks);
        Debug.Log("Unfocused Correct Clicks: " + unfocusedCorrectClicks);
        Debug.Log("Unfocused Wrong Clicks: " + unfocusedWrongClicks);
    }

    void SaveCurrentGame3Data()
    {
        float focusedDelta = focusedTime - lastSavedFocusedTime;
        float unfocusedDelta = unfocusedTime - lastSavedUnfocusedTime;

        int focusedCorrectDelta = focusedCorrectClicks - lastSavedFocusedCorrectClicks;
        int focusedWrongDelta = focusedWrongClicks - lastSavedFocusedWrongClicks;
        int unfocusedCorrectDelta = unfocusedCorrectClicks - lastSavedUnfocusedCorrectClicks;
        int unfocusedWrongDelta = unfocusedWrongClicks - lastSavedUnfocusedWrongClicks;
        int missedDelta = missedCount - lastSavedMissedCount;

        float reactionDelta = totalReactionTime - lastSavedReactionTime;
        int reactionCountDelta = reactionCount - lastSavedReactionCount;

        float durationDelta = focusedDelta + unfocusedDelta;

        if (durationDelta <= 0f &&
            focusedCorrectDelta == 0 &&
            focusedWrongDelta == 0 &&
            unfocusedCorrectDelta == 0 &&
            unfocusedWrongDelta == 0 &&
            missedDelta == 0 &&
            reactionCountDelta == 0)
            return;

        ReportDataManager.SaveGameReport(
            "Game3",
            durationDelta,
            focusedDelta,
            unfocusedDelta,
            focusedCorrectDelta,
            focusedWrongDelta,
            unfocusedCorrectDelta,
            unfocusedWrongDelta,
            focusedCorrectDelta,
            focusedWrongDelta,
            missedDelta,
            reactionDelta,
            reactionCountDelta
        );

        lastSavedFocusedTime = focusedTime;
        lastSavedUnfocusedTime = unfocusedTime;
        lastSavedFocusedCorrectClicks = focusedCorrectClicks;
        lastSavedFocusedWrongClicks = focusedWrongClicks;
        lastSavedUnfocusedCorrectClicks = unfocusedCorrectClicks;
        lastSavedUnfocusedWrongClicks = unfocusedWrongClicks;
        lastSavedMissedCount = missedCount;
        lastSavedReactionTime = totalReactionTime;
        lastSavedReactionCount = reactionCount;
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveCurrentGame3Data();
    }

    void OnApplicationQuit()
    {
        SaveCurrentGame3Data();
    }

    public void BackToGames()
    {
        SaveCurrentGame3Data();
        SceneManager.LoadScene("MainMenu");
    }

    public void BackToMainMenu()
    {
        SaveCurrentGame3Data();
        SceneManager.LoadScene("MainMenu");
    }
}