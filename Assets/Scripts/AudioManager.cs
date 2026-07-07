using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Clips")]
    public AudioClip uiClickSound;
    public AudioClip mysteryAlertSound;
    public AudioClip correctAnswerSound;
    public AudioClip wrongAnswerSound;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Instance.CopyMissingClipsFrom(this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.volume = 1f;
        audioSource.spatialBlend = 0f;

        Debug.Log("AudioManager aktif oldu.");
        Debug.Log("Wrong clip: " + (wrongAnswerSound != null ? wrongAnswerSound.name : "NULL"));
    }

    void CopyMissingClipsFrom(AudioManager other)
    {
        if (uiClickSound == null && other.uiClickSound != null)
            uiClickSound = other.uiClickSound;

        if (mysteryAlertSound == null && other.mysteryAlertSound != null)
            mysteryAlertSound = other.mysteryAlertSound;

        if (correctAnswerSound == null && other.correctAnswerSound != null)
            correctAnswerSound = other.correctAnswerSound;

        if (wrongAnswerSound == null && other.wrongAnswerSound != null)
            wrongAnswerSound = other.wrongAnswerSound;

        Debug.Log("Duplicate AudioManager bulundu. Eksik sesler mevcut AudioManager'a kopyalandı.");
    }

    public void PlayUIClick()
    {
        if (uiClickSound == null)
        {
            Debug.LogWarning("AudioManager: UI Click Sound atanmadı.");
            return;
        }

        audioSource.PlayOneShot(uiClickSound);
    }

    public void PlayMysteryAlert()
    {
        if (mysteryAlertSound == null)
        {
            Debug.LogWarning("AudioManager: Mystery Alert Sound atanmadı.");
            return;
        }

        audioSource.PlayOneShot(mysteryAlertSound);
    }

    public void PlayCorrect()
    {
        if (correctAnswerSound == null)
        {
            Debug.LogWarning("AudioManager: Correct Answer Sound atanmadı.");
            return;
        }

        audioSource.PlayOneShot(correctAnswerSound);
    }

    public void PlayWrong()
    {
        Debug.Log("AudioManager PlayWrong çağrıldı.");

        if (wrongAnswerSound == null)
        {
            Debug.LogWarning("AudioManager: Wrong Answer Sound atanmadı.");
            return;
        }

        audioSource.PlayOneShot(wrongAnswerSound, 1f);
    }
}