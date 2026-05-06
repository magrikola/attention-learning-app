using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Game4Manager : MonoBehaviour
{
    public Button playAgainButton;
    public TMP_Text scoreText;
    public TMP_Text infoText;
    public TMP_Text focusText;

    public Button redButton;
    public Button blueButton;
    public Button greenButton;
    public Button yellowButton;

    public float flashTime = 0.5f;
    public float timeBetweenFlashes = 0.25f;
    public float gameDuration = 30f;

    [Header("Button Bounce Effect")]
    public float bounceScale = 1.15f;
    public float bounceDuration = 0.12f;

    private float remainingTime;

    private List<int> sequence = new List<int>();
    private int playerIndex = 0;

    private int correctCount = 0;
    private int wrongCount = 0;

    private int focusedCorrectAttempts = 0;
    private int focusedWrongAttempts = 0;
    private int unfocusedCorrectAttempts = 0;
    private int unfocusedWrongAttempts = 0;

    private int currentStreak = 0;
    private int bestStreak = 0;

    private bool playerTurn = false;
    private bool gameEnded = false;

    private float focusedTime = 0f;
    private float unfocusedTime = 0f;

    private Button[] buttons;
    private Color[] originalColors;
    private Vector3[] originalScales;

    void Start()
    {
        if (playAgainButton != null)
            playAgainButton.gameObject.SetActive(false);

        remainingTime = gameDuration;

        buttons = new Button[] { redButton, blueButton, greenButton, yellowButton };
        originalColors = new Color[buttons.Length];
        originalScales = new Vector3[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            originalColors[i] = buttons[i].image.color;
            originalScales[i] = buttons[i].transform.localScale;
        }

        redButton.onClick.AddListener(() => PlayerPressed(0));
        blueButton.onClick.AddListener(() => PlayerPressed(1));
        greenButton.onClick.AddListener(() => PlayerPressed(2));
        yellowButton.onClick.AddListener(() => PlayerPressed(3));

        UpdateScoreText();
        UpdateFocusText();
        SetButtonsInteractable(false);

        StartCoroutine(StartNewRound());
    }

    void Update()
    {
        if (gameEnded) return;

        remainingTime -= Time.deltaTime;

        bool isFocused = GetFocusState();

        if (isFocused)
            focusedTime += Time.deltaTime;
        else
            unfocusedTime += Time.deltaTime;

        SetButtonsInteractable(playerTurn);

        UpdateFocusText();

        if (remainingTime <= 0)
            EndGame();
    }

    bool GetFocusState()
    {
        if (FakeAttentionManager.Instance == null)
        {
            Debug.LogWarning("FakeAttentionManager Instance could not found. Focus default accepted as true.");
            return true;
        }

        return FakeAttentionManager.Instance.GetFocus();
    }

    void UpdateFocusText()
    {
        if (focusText == null) return;

        if (FakeAttentionManager.Instance == null)
            focusText.text = "Focused";
        else
            focusText.text = FakeAttentionManager.Instance.GetFocusText();
    }

    IEnumerator StartNewRound()
    {
        if (gameEnded) yield break;

        playerTurn = false;
        SetButtonsInteractable(false);

        if (infoText != null)
            infoText.text = "Watch the sequence";

        yield return new WaitForSeconds(1f);

        sequence.Clear();

        int length = Mathf.Clamp(1 + correctCount / 2, 1, 5);

        for (int i = 0; i < length; i++)
            sequence.Add(Random.Range(0, buttons.Length));

        yield return StartCoroutine(ShowSequence());

        if (gameEnded) yield break;

        playerIndex = 0;
        playerTurn = true;

        if (infoText != null)
            infoText.text = "Repeat the sequence";

        SetButtonsInteractable(true);
    }

    IEnumerator ShowSequence()
    {
        for (int i = 0; i < sequence.Count; i++)
        {
            if (gameEnded) yield break;

            int index = sequence[i];

            buttons[index].image.color = Color.white;
            yield return new WaitForSeconds(flashTime);

            buttons[index].image.color = originalColors[index];
            yield return new WaitForSeconds(timeBetweenFlashes);
        }
    }

    void PlayerPressed(int buttonIndex)
    {
        if (gameEnded) return;
        if (!playerTurn) return;

        StartCoroutine(BounceButton(buttonIndex));

        bool isFocused = GetFocusState();
        bool isCorrectButton = buttonIndex == sequence[playerIndex];

        if (isCorrectButton)
        {
            if (isFocused)
                focusedCorrectAttempts++;
            else
                unfocusedCorrectAttempts++;

            playerIndex++;

            if (playerIndex >= sequence.Count)
            {
                correctCount++;
                currentStreak++;

                if (currentStreak > bestStreak)
                    bestStreak = currentStreak;

                if (infoText != null)
                    infoText.text = isFocused ? "Correct!" : "Correct, but unfocused";

                playerTurn = false;
                UpdateScoreText();

                StartCoroutine(NextRoundDelay());
            }
        }
        else
        {
            wrongCount++;
            currentStreak = 0;

            if (isFocused)
                focusedWrongAttempts++;
            else
                unfocusedWrongAttempts++;

            if (infoText != null)
                infoText.text = isFocused ? "Wrong! Game Over" : "Wrong while unfocused";

            playerTurn = false;
            gameEnded = true;

            SetButtonsInteractable(false);
            UpdateScoreText();
            SaveGame4Report();

            if (playAgainButton != null)
                playAgainButton.gameObject.SetActive(true);

            Debug.Log("Game4 ended because of wrong answer.");
        }

        UpdateScoreText();
    }

    IEnumerator BounceButton(int index)
    {
        if (buttons[index] == null) yield break;

        Transform buttonTransform = buttons[index].transform;

        Vector3 normalScale = originalScales[index];
        Vector3 targetScale = normalScale * bounceScale;

        float timer = 0f;

        while (timer < bounceDuration)
        {
            timer += Time.deltaTime;
            buttonTransform.localScale = Vector3.Lerp(normalScale, targetScale, timer / bounceDuration);
            yield return null;
        }

        timer = 0f;

        while (timer < bounceDuration)
        {
            timer += Time.deltaTime;
            buttonTransform.localScale = Vector3.Lerp(targetScale, normalScale, timer / bounceDuration);
            yield return null;
        }

        buttonTransform.localScale = normalScale;
    }

    IEnumerator NextRoundDelay()
    {
        SetButtonsInteractable(false);
        yield return new WaitForSeconds(1f);

        if (!gameEnded)
            StartCoroutine(StartNewRound());
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text =
                "Correct: " + focusedCorrectAttempts ;
        }
    }

    void SetButtonsInteractable(bool value)
    {
        if (redButton != null) redButton.interactable = value;
        if (blueButton != null) blueButton.interactable = value;
        if (greenButton != null) greenButton.interactable = value;
        if (yellowButton != null) yellowButton.interactable = value;
    }
    public void BackToMainMenu()
    {
        SaveGame4Report();
        SceneManager.LoadScene("MainMenu");
    }

    void SaveGame4Report()
    {
        ReportDataManager.SaveGameReport(
            "Game4",
            focusedTime + unfocusedTime,
            focusedTime,
            unfocusedTime,
            focusedCorrectAttempts,
            focusedWrongAttempts,
            unfocusedCorrectAttempts,
            unfocusedWrongAttempts,
            correctCount,
            wrongCount,
            0,
            0f,
            0
        );
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void EndGame()
    {
        gameEnded = true;
        playerTurn = false;
        SetButtonsInteractable(false);
        StopAllCoroutines();

        SaveGame4Report();

        if (infoText != null)
            infoText.text = "Game Over";

        if (playAgainButton != null)
            playAgainButton.gameObject.SetActive(true);

        Debug.Log("=== GAME 4 SUMMARY ===");
        Debug.Log("Correct: " + correctCount);
        Debug.Log("Wrong: " + wrongCount);
        Debug.Log("Focused Correct Attempts: " + focusedCorrectAttempts);
        Debug.Log("Focused Wrong Attempts: " + focusedWrongAttempts);
        Debug.Log("Unfocused Correct Attempts: " + unfocusedCorrectAttempts);
        Debug.Log("Unfocused Wrong Attempts: " + unfocusedWrongAttempts);
        Debug.Log("Best Streak: " + bestStreak);
        Debug.Log("Focused Time: " + focusedTime.ToString("F2"));
        Debug.Log("Unfocused Time: " + unfocusedTime.ToString("F2"));
    }
}