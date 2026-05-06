using UnityEngine;

public class FakeAttentionManager : MonoBehaviour
{
    public static FakeAttentionManager Instance;

    [Header("Current Attention State")]
    public bool isFocused = true;

    [Header("Prototype / Debug")]
    public bool useFakeAttention = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ToggleFakeFocus()
    {
        if (!useFakeAttention)
            return;

        SetFocus(!isFocused);
    }

    public void SetFocus(bool value)
    {
        isFocused = value;
        Debug.Log("Attention State: " + GetFocusText());
    }

    public bool GetFocus()
    {
        return isFocused;
    }

    public string GetFocusText()
    {
        return isFocused ? "Focused" : "Not Focused";
    }
}