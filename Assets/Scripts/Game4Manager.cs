using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Game4Manager : MonoBehaviour
{
    public Button playAgainButton;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text infoText;
    public TMP_Text focusText;

    public Button redButton;
    public Button blueButton;
    public Button greenButton;
    public Button yellowButton;

    public float flashTime = 0.5f;
    public float timeBetweenFlashes = 0.25f;

    [Header("Button Bounce Effect")]
    public float bounceScale = 1.15f;
    public float bounceDuration = 0.12f;

    [Header("Sequence Highlight")]
    public float sequenceScale = 1.22f;
    public float dimAlpha = 0.30f;

    [Header("Calibration Pause")]
    public ARFaceHeadPoseInput faceHeadPoseInput;

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
    private bool wasPausedForCalibration = false;

    private float focusedTime = 0f;
    private float unfocusedTime = 0f;

    private Button[] buttons;
    private Color[] originalColors;
    private Vector3[] originalScales;

    void Start()
    {
        if (playAgainButton != null)
            playAgainButton.gameObject.SetActive(false);

        if (faceHeadPoseInput == null)
            faceHeadPoseInput = FindObjectOfType<ARFaceHeadPoseInput>();

        buttons = new Button[] { redButton, blueButton, greenButton, yellowButton };
        originalColors = new Color[buttons.Length];
        originalScales = new Vector3[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;

            originalColors[i] = buttons[i].image.color;
            originalScales[i] = buttons[i].transform.localScale;
        }

        if (redButton != null) redButton.onClick.AddListener(() => PlayerPressed(0));
        if (blueButton != null) blueButton.onClick.AddListener(() => PlayerPressed(1));
        if (greenButton != null) greenButton.onClick.AddListener(() => PlayerPressed(2));
        if (yellowButton != null) yellowButton.onClick.AddListener(() => PlayerPressed(3));

        UpdateScoreText();
        UpdateFocusText();
        SetButtonsInteractable(false);
        ResetAllButtonsVisuals();

        if (IsGamePausedForCalibration())
        {
            wasPausedForCalibration = true;

            if (infoText != null)
                infoText.text = "Calibrating...";

            SetButtonsInteractable(false);
            ResetAllButtonsVisuals();
        }
        else
        {
            StartCoroutine(StartNewRound());
        }
    }

    void Update()
    {
        if (gameEnded) return;

        if (IsGamePausedForCalibration())
        {
            if (!wasPausedForCalibration)
            {
                wasPausedForCalibration = true;

                StopAllCoroutines();
                playerTurn = false;
                SetButtonsInteractable(false);
                ResetAllButtonsVisuals();

                if (infoText != null)
                    infoText.text = "Calibrating...";
            }

            UpdateFocusText();
            return;
        }

        if (wasPausedForCalibration)
        {
            wasPausedForCalibration = false;

            playerTurn = false;
            SetButtonsInteractable(false);
            ResetAllButtonsVisuals();

            if (infoText != null)
                infoText.text = "Watch the sequence";

            StartCoroutine(StartNewRound());
        }

        bool isFocused = GetFocusState();

        if (isFocused)
            focusedTime += Time.deltaTime;
        else
            unfocusedTime += Time.deltaTime;

        SetButtonsInteractable(playerTurn);
        UpdateFocusText();
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

    void UpdateFocusText()
    {
        if (focusText == null) return;

        if (IsGamePausedForCalibration())
        {
            focusText.text = "Calibrating...";
            return;
        }

        bool isFocused = GetFocusState();

        focusText.text = isFocused ? "Focused" : "Not Focused";
    }

    void DimAllButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;

            buttons[i].image.color = new Color(
                originalColors[i].r,
                originalColors[i].g,
                originalColors[i].b,
                dimAlpha
            );

            buttons[i].transform.localScale = originalScales[i];
        }
    }

    void ResetAllButtonsVisuals()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;

            buttons[i].image.color = originalColors[i];
            buttons[i].transform.localScale = originalScales[i];
        }
    }

    IEnumerator StartNewRound()
    {
        if (gameEnded) yield break;
        if (IsGamePausedForCalibration()) yield break;

        playerTurn = false;
        SetButtonsInteractable(false);
        ResetAllButtonsVisuals();

        if (infoText != null)
            infoText.text = "Watch the sequence";

        yield return new WaitForSeconds(1f);

        if (gameEnded) yield break;
        if (IsGamePausedForCalibration()) yield break;

        sequence.Clear();

        int length = Mathf.Clamp(1 + correctCount / 2, 1, 5);

        for (int i = 0; i < length; i++)
            sequence.Add(Random.Range(0, buttons.Length));

        yield return StartCoroutine(ShowSequence());

        if (gameEnded) yield break;
        if (IsGamePausedForCalibration()) yield break;

        playerIndex = 0;
        playerTurn = true;

        if (infoText != null)
            infoText.text = "Repeat the sequence";

        SetButtonsInteractable(true);
        ResetAllButtonsVisuals();
    }

    IEnumerator ShowSequence()
    {
        if (IsGamePausedForCalibration()) yield break;

        DimAllButtons();
        yield return new WaitForSeconds(0.15f);

        for (int i = 0; i < sequence.Count; i++)
        {
            if (gameEnded) yield break;
            if (IsGamePausedForCalibration()) yield break;

            int index = sequence[i];

            if (buttons[index] == null) continue;

            buttons[index].image.color = new Color(
                originalColors[index].r,
                originalColors[index].g,
                originalColors[index].b,
                1f
            );

            buttons[index].transform.localScale = originalScales[index] * sequenceScale;

            yield return new WaitForSeconds(flashTime);

            buttons[index].image.color = new Color(
                originalColors[index].r,
                originalColors[index].g,
                originalColors[index].b,
                dimAlpha
            );

            buttons[index].transform.localScale = originalScales[index];

            yield return new WaitForSeconds(timeBetweenFlashes);
        }

        ResetAllButtonsVisuals();
    }

    void PlayerPressed(int buttonIndex)
    {
        if (gameEnded) return;
        if (IsGamePausedForCalibration()) return;
        if (!playerTurn) return;
        if (sequence.Count == 0) return;
        if (playerIndex < 0 || playerIndex >= sequence.Count) return;

        PlayMysterySound();
        StartCoroutine(BounceButton(buttonIndex));

        bool isFocused = GetFocusState();
        bool isCorrectButton = buttonIndex == sequence[playerIndex];

        if (!isFocused)
        {
            if (isCorrectButton)
            {
                unfocusedCorrectAttempts++;

                if (infoText != null)
                    infoText.text = "Correct, but not focused";
            }
            else
            {
                unfocusedWrongAttempts++;

                if (infoText != null)
                    infoText.text = "Wrong while unfocused";
            }

            StartCoroutine(PlayWrongSoundDelayed());

            playerTurn = false;
            SetButtonsInteractable(false);
            ResetAllButtonsVisuals();

            SaveGame4Report();
            UpdateScoreText();

            StartCoroutine(RestartRoundAfterUnfocusedAttempt());
            return;
        }

        if (isCorrectButton)
        {
            focusedCorrectAttempts++;
            playerIndex++;

            if (playerIndex >= sequence.Count)
            {
                correctCount++;
                currentStreak++;

                StartCoroutine(PlayCorrectSoundDelayed());

                if (currentStreak > bestStreak)
                    bestStreak = currentStreak;

                if (infoText != null)
                    infoText.text = "Correct!";

                playerTurn = false;
                UpdateScoreText();
                SaveGame4Report();

                StartCoroutine(NextRoundDelay());
            }
            else
            {
                if (infoText != null)
                    infoText.text = "Good! Continue";

                SaveGame4Report();
            }
        }
        else
        {
            wrongCount++;
            currentStreak = 0;
            focusedWrongAttempts++;

            StartCoroutine(PlayWrongSoundDelayed());

            if (infoText != null)
                infoText.text = "Wrong! Game Over";

            playerTurn = false;
            gameEnded = true;

            SetButtonsInteractable(false);
            ResetAllButtonsVisuals();
            UpdateScoreText();
            SaveGame4Report();

            if (playAgainButton != null)
                playAgainButton.gameObject.SetActive(true);

            Debug.Log("Game4 ended because of wrong answer.");
        }

        UpdateScoreText();
    }

    IEnumerator RestartRoundAfterUnfocusedAttempt()
    {
        yield return new WaitForSeconds(1f);

        if (!gameEnded && !IsGamePausedForCalibration())
            StartCoroutine(StartNewRound());
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

        if (!gameEnded && !IsGamePausedForCalibration())
            StartCoroutine(StartNewRound());
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + correctCount;
    }

    void SetButtonsInteractable(bool value)
    {
        if (redButton != null) redButton.interactable = value;
        if (blueButton != null) blueButton.interactable = value;
        if (greenButton != null) greenButton.interactable = value;
        if (yellowButton != null) yellowButton.interactable = value;
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

    public void EndSession()
    {
        EndGame();
    }

    void EndGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        playerTurn = false;
        SetButtonsInteractable(false);
        StopAllCoroutines();
        ResetAllButtonsVisuals();

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

    void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveGame4Report();
    }

    void OnApplicationQuit()
    {
        SaveGame4Report();
    }

    public void BackToGames()
    {
        SaveGame4Report();
        SceneManager.LoadScene("GameSelection");
    }

    public void BackToMainMenu()
    {
        SaveGame4Report();
        SceneManager.LoadScene("MainMenu");
    }
}