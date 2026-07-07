using UnityEngine;

public class AttentionSystemManager : MonoBehaviour
{
    public static AttentionSystemManager Instance;

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

        Debug.Log("[AttentionSystem] Provider changed: " + provider.GetType().Name);
    }

    public bool GetFocusState()
    {
        if (currentProvider == null)
        {
            return true;
        }

        return currentProvider.GetFocusState();
    }
}