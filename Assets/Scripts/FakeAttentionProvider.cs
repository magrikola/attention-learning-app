using UnityEngine;

public class FakeAttentionProvider : MonoBehaviour, IAttentionProvider
{
    public bool currentFocusState = true;

    void Start()
    {
        if (AttentionSystemManager.Instance != null)
        {
            AttentionSystemManager.Instance.SetProvider(this);
        }
    }

    public bool GetFocusState()
    {
        return currentFocusState;
    }

    public void SetFocusState(bool value)
    {
        currentFocusState = value;

        if (FakeAttentionManager.Instance != null)
        {
            FakeAttentionManager.Instance.SetFocus(value);
        }

        Debug.Log("[FakeAttentionProvider] Focus changed: " + value);
    }
}