using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Game3Manager : MonoBehaviour
{
    public Button playAgainButton;

    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text attentionText;
    public TMP_Text reactionInfoText;
    public TMP_Text missedText;

    public GameObject reactionTarget;

    public int score = 0;
    public int missedCount = 0;

    public float gameDuration = 30f;
    public float targetVisibleTime = 1.0f;

    private float remainingTime;
    private float targetAppearTime;
    private bool targetActive = false;
    private bool gameEnded = false;

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveCurrentGame3Data();
        }

        if (targetActive && Time.time - targetAppearTime > targetVisibleTime)
        {
            MissTarget();
        }

        if (remainingTime <= 0)
        {
            EndGame();
        }
    }

    bool GetFocusState()
    {
        if (FakeAttentionManager.Instance == null)
        {
            
            return true;
        }

        return FakeAttentionManager.Instance.GetFocus();
    }

    void UpdateAttentionText()
    {
        if (attentionText == null) return;

        if (FakeAttentionManager.Instance == null)
            attentionText.text = "Focused";
        else
            attentionText.text = FakeAttentionManager.Instance.GetFocusText();
    }

    void ShowTarget()
    {
        if (gameEnded) return;
        if (reactionTarget == null) return;

        float randomX = Random.Range(-2f, 2f);
        float randomY = Random.Range(-3f, 2f);

        reactionTarget.transform.position = new Vector3(randomX, randomY, 0f);
        reactionTarget.SetActive(true);

        targetAppearTime = Time.time;
        targetActive = true;
    }

    void HideTarget()
    {
        if (reactionTarget != null)
            reactionTarget.SetActive(false);

        targetActive = false;
    }

    public void TargetClicked()
    {
        if (!targetActive || gameEnded) return;

        bool isFocused = GetFocusState();
        float reactionTime = Time.time - targetAppearTime;

        score++;

        if (isFocused)
        {
            focusedCorrectClicks++;

            totalReactionTime += reactionTime;
            reactionCount++;

            if (reactionInfoText != null)
                reactionInfoText.text = "Reaction: " + reactionTime.ToString("F2") + "s";
        }
        else
        {
            unfocusedCorrectClicks++;

            if (reactionInfoText != null)
                reactionInfoText.text = "Unfocused correct click";
        }

        UpdateUI();
        SaveCurrentGame3Data();

        StartCoroutine(TargetClickedAfterBounce());
    }

    IEnumerator TargetClickedAfterBounce()
    {
        targetActive = false;

        BounceEffectClicker effect = reactionTarget.GetComponent<BounceEffectClicker>();

        if (effect != null)
            effect.PlayEffect();

        yield return new WaitForSeconds(0.20f);

        HideTarget();

        float nextDelay = Random.Range(0.8f, 1.8f);
        Invoke(nameof(ShowTarget), nextDelay);
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
            Debug.Log("Game3 focused wrong click counted.");
        }
        else
        {
            unfocusedWrongClicks++;
            Debug.Log("Game3 unfocused wrong click counted.");
        }

        SaveCurrentGame3Data();
    }
    bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        return EventSystem.current.IsPointerOverGameObject();
    }

    void MissTarget()
    {
        missedCount++;

        if (reactionInfoText != null)
            reactionInfoText.text = "Missed!";

        HideTarget();
        UpdateUI();
        SaveCurrentGame3Data();

        float nextDelay = Random.Range(0.6f, 1.4f);
        Invoke(nameof(ShowTarget), nextDelay);
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (missedText != null)
            missedText.text = "Missed: " + missedCount;
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void EndGame()
    {
        gameEnded = true;

        if (playAgainButton != null)
            playAgainButton.gameObject.SetActive(true);

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
}