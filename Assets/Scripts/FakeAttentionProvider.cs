using UnityEngine;

public class FakeAttentionProvider : MonoBehaviour, IAttentionProvider
{
    public bool currentFocusState = true;

    public bool GetFocusState()
    {
        return currentFocusState;
    }

    public void SetFocusState(bool value)
    {
        currentFocusState = value;

        Debug.Log("[FakeAttentionProvider] Focus changed: " + value);
    }
}