using UnityEngine;

public class AttentionSystemManager : MonoBehaviour
{
    public static AttentionSystemManager Instance;

    [Header("Current Attention State")]
    [SerializeField] private bool isFocused = true;

    [Header("Debug")]
    public bool logChanges = true;

    private IAttentionProvider currentProvider;

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

    public void SetProvider(IAttentionProvider provider)
    {
        currentProvider = provider;

        if (logChanges && provider != null)
        {
            Debug.Log("[AttentionSystemManager] Provider changed: " + provider.GetType().Name);
        }
    }

    public bool GetFocusState()
    {
        if (currentProvider != null)
        {
            return currentProvider.GetFocusState();
        }

        return isFocused;
    }

    public void SetFocusState(bool value)
    {
        currentProvider = null;

        isFocused = value;

        if (FakeAttentionManager.Instance != null)
        {
            FakeAttentionManager.Instance.SetFocus(value);
        }

        if (logChanges)
        {
            Debug.Log("[AttentionSystemManager] Focus State: " + (isFocused ? "Focused" : "Not Focused"));
        }
    }

    public void ToggleFocusState()
    {
        bool newState = !GetFocusState();
        SetFocusState(newState);
    }

    public string GetFocusText()
    {
        return GetFocusState() ? "Focused" : "Not Focused";
    }

  
    public void SetFocusedFromButton()
    {
        SetFocusState(true);
    }

    public void SetUnfocusedFromButton()
    {
        SetFocusState(false);
    }

    public void ToggleFocusFromButton()
    {
        ToggleFocusState();
    }
}