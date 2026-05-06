using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game2Manager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text attentionText;
    public TMP_Text instructionText;

    public Button playAgainButton;

    [Header("Color Objects")]
    public RectTransform redObject;
    public RectTransform blueObject;
    public RectTransform yellowObject;

    public int score = 0;
    public float gameDuration = 30f;

    private float remainingTime;
    private string targetColor;

    private string[] colors = { "RED", "BLUE", "YELLOW" };

    private int correctAnswers = 0;
    private int wrongAnswers = 0;

    private int focusedCorrectAttempts = 0;
    private int focusedWrongAttempts = 0;
    private int unfocusedCorrectAttempts = 0;
    private int unfocusedWrongAttempts = 0;

    private float focusedTime = 0f;
    private float unfocusedTime = 0f;

    private float saveTimer = 0f;

    private float lastSavedFocusedTime = 0f;
    private float lastSavedUnfocusedTime = 0f;
    private int lastSavedCorrectAnswers = 0;
    private int lastSavedWrongAnswers = 0;
    private int lastSavedFocusedCorrectAttempts = 0;
    private int lastSavedFocusedWrongAttempts = 0;
    private int lastSavedUnfocusedCorrectAttempts = 0;
    private int lastSavedUnfocusedWrongAttempts = 0;

    private bool gameEnded = false;

    void Start()
    {
        remainingTime = gameDuration;

        GenerateNewTask();
        RandomizeColorPositions();

        UpdateUI();
        UpdateAttentionText();

        if (playAgainButton != null)
            playAgainButton.gameObject.SetActive(false);
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
            SaveCurrentGame2Data();
            saveTimer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveCurrentGame2Data();
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
            Debug.LogWarning("FakeAttentionManager Instance could not found. Focus default accepted as TRUE.");
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

    void GenerateNewTask()
    {
        targetColor = colors[Random.Range(0, colors.Length)];

        if (instructionText != null)
            instructionText.text = "Tap the " + targetColor + " object";
    }

    void RandomizeColorPositions()
    {
        if (redObject == null || blueObject == null || yellowObject == null)
            return;

        Vector2[] positions = new Vector2[3];

        positions[0] = redObject.anchoredPosition;
        positions[1] = blueObject.anchoredPosition;
        positions[2] = yellowObject.anchoredPosition;

        for (int i = 0; i < positions.Length; i++)
        {
            int rand = Random.Range(i, positions.Length);

            Vector2 temp = positions[i];
            positions[i] = positions[rand];
            positions[rand] = temp;
        }

        redObject.anchoredPosition = positions[0];
        blueObject.anchoredPosition = positions[1];
        yellowObject.anchoredPosition = positions[2];
    }

    public void CheckAnswer(string selectedColor, GameObject clickedObject)
    {
        if (gameEnded) return;

        BounceEffectClicker effect = clickedObject.GetComponent<BounceEffectClicker>();
        if (effect != null)
            effect.PlayEffect();

        bool isCorrect = selectedColor == targetColor;
        bool isFocused = GetFocusState();

        if (isCorrect)
        {
            score++;
            correctAnswers++;

            if (isFocused)
                focusedCorrectAttempts++;
            else
                unfocusedCorrectAttempts++;
        }
        else
        {
            wrongAnswers++;

            if (isFocused)
                focusedWrongAttempts++;
            else
                unfocusedWrongAttempts++;
        }

        UpdateUI();
        SaveCurrentGame2Data();

        StartCoroutine(NextTaskAfterEffect());
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    System.Collections.IEnumerator NextTaskAfterEffect()
    {
        yield return new WaitForSeconds(0.18f);

        GenerateNewTask();
        RandomizeColorPositions();
    }

    void EndGame()
    {
        gameEnded = true;

        SaveCurrentGame2Data();

        if (playAgainButton != null)
            playAgainButton.gameObject.SetActive(true);

        Debug.Log("=== GAME 2 SUMMARY ===");
        Debug.Log("Score: " + score);
        Debug.Log("Correct: " + correctAnswers);
        Debug.Log("Wrong: " + wrongAnswers);
        Debug.Log("Focused Correct Attempts: " + focusedCorrectAttempts);
        Debug.Log("Focused Wrong Attempts: " + focusedWrongAttempts);
        Debug.Log("Unfocused Correct Attempts: " + unfocusedCorrectAttempts);
        Debug.Log("Unfocused Wrong Attempts: " + unfocusedWrongAttempts);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SaveCurrentGame2Data()
    {
        float focusedDelta = focusedTime - lastSavedFocusedTime;
        float unfocusedDelta = unfocusedTime - lastSavedUnfocusedTime;

        int correctDelta = correctAnswers - lastSavedCorrectAnswers;
        int wrongDelta = wrongAnswers - lastSavedWrongAnswers;

        int focusedCorrectDelta = focusedCorrectAttempts - lastSavedFocusedCorrectAttempts;
        int focusedWrongDelta = focusedWrongAttempts - lastSavedFocusedWrongAttempts;
        int unfocusedCorrectDelta = unfocusedCorrectAttempts - lastSavedUnfocusedCorrectAttempts;
        int unfocusedWrongDelta = unfocusedWrongAttempts - lastSavedUnfocusedWrongAttempts;

        float durationDelta = focusedDelta + unfocusedDelta;

        if (durationDelta <= 0 &&
            correctDelta == 0 &&
            wrongDelta == 0 &&
            focusedCorrectDelta == 0 &&
            focusedWrongDelta == 0 &&
            unfocusedCorrectDelta == 0 &&
            unfocusedWrongDelta == 0)
            return;

        ReportDataManager.SaveGameReport(
            "Game2",
            durationDelta,
            focusedDelta,
            unfocusedDelta,
            focusedCorrectDelta,
            focusedWrongDelta,
            unfocusedCorrectDelta,
            unfocusedWrongDelta,
            correctDelta,
            wrongDelta,
            0,
            0f,
            0
        );

        lastSavedFocusedTime = focusedTime;
        lastSavedUnfocusedTime = unfocusedTime;
        lastSavedCorrectAnswers = correctAnswers;
        lastSavedWrongAnswers = wrongAnswers;
        lastSavedFocusedCorrectAttempts = focusedCorrectAttempts;
        lastSavedFocusedWrongAttempts = focusedWrongAttempts;
        lastSavedUnfocusedCorrectAttempts = unfocusedCorrectAttempts;
        lastSavedUnfocusedWrongAttempts = unfocusedWrongAttempts;
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveCurrentGame2Data();
    }

    void OnApplicationQuit()
    {
        SaveCurrentGame2Data();
    }
}