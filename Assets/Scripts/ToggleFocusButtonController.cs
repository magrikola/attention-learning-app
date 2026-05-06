using UnityEngine;

public class ToggleFocusButtonController : MonoBehaviour
{
    public void ToggleFocus()
    {
        if (FakeAttentionManager.Instance != null)
        {
            FakeAttentionManager.Instance.ToggleFakeFocus();
        }
        else
        {
            Debug.LogError("FakeAttentionManager Instance could not found!");
        }
    }
}