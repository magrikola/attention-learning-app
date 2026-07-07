using UnityEngine;

public class ToggleFocusButtonController : MonoBehaviour
{
    public void ToggleFocus()
    {
        if (AttentionSystemManager.Instance != null)
        {
            AttentionSystemManager.Instance.ToggleFocusState();

            Debug.Log("[ToggleFocusButtonController] Focus toggled: " +
                      AttentionSystemManager.Instance.GetFocusText());
        }
        else
        {
            Debug.LogError("[ToggleFocusButtonController] AttentionSystemManager Instance not found! Start the game from MainMenu scene.");
        }
    }

    public void SetFocused()
    {
        if (AttentionSystemManager.Instance != null)
        {
            AttentionSystemManager.Instance.SetFocusState(true);
            Debug.Log("[ToggleFocusButtonController] Set Focused");
        }
        else
        {
            Debug.LogError("[ToggleFocusButtonController] AttentionSystemManager Instance not found!");
        }
    }

    public void SetUnfocused()
    {
        if (AttentionSystemManager.Instance != null)
        {
            AttentionSystemManager.Instance.SetFocusState(false);
            Debug.Log("[ToggleFocusButtonController] Set Not Focused");
        }
        else
        {
            Debug.LogError("[ToggleFocusButtonController] AttentionSystemManager Instance not found!");
        }
    }
}