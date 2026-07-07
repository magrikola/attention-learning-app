using UnityEngine;

public class ARFocusTester : MonoBehaviour
{
    public float switchInterval = 5f;

    private float timer = 0f;
    private bool currentFocus = true;

    void Start()
    {
        SendFocusState(true);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= switchInterval)
        {
            timer = 0f;
            currentFocus = !currentFocus;
            SendFocusState(currentFocus);
        }
    }

    void SendFocusState(bool focus)
    {
        if (AttentionSystemManager.Instance != null)
        {
            AttentionSystemManager.Instance.SetFocusState(focus);
            Debug.Log("[ARFocusTester] AR TEST -> Focus State Changed: " + focus);
        }
        else
        {
            Debug.LogWarning("[ARFocusTester] AttentionSystemManager not found!");
        }
    }
}