using UnityEngine;

public class AttentionBridgeManager : MonoBehaviour
{
    [Header("Mode Settings")]
    public bool useAutomaticSimulation = false;

    [Header("Simulation Settings")]
    public float focusedDuration = 5f;
    public float unfocusedDuration = 3f;

    private float timer = 0f;
    private bool simulatedFocus = true;

    void Start()
    {
        ApplyFocusState(true);
    }

    void Update()
    {
        if (!useAutomaticSimulation)
            return;

        timer += Time.deltaTime;

        if (simulatedFocus && timer >= focusedDuration)
        {
            simulatedFocus = false;
            timer = 0f;
            ApplyFocusState(false);
        }
        else if (!simulatedFocus && timer >= unfocusedDuration)
        {
            simulatedFocus = true;
            timer = 0f;
            ApplyFocusState(true);
        }
    }

    public void ApplyFocusState(bool isFocused)
    {
        if (AttentionSystemManager.Instance != null)
        {
            AttentionSystemManager.Instance.SetFocusState(isFocused);
        }

        Debug.Log("[AttentionBridgeManager] Focus state sent: " + isFocused);
    }

    public void ApplyModelResult(bool modelFocusedResult)
    {
        ApplyFocusState(modelFocusedResult);
    }
}