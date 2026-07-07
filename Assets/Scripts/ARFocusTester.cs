using UnityEngine;

public class ARFocusTester : MonoBehaviour
{
    public float switchInterval = 5f;

    private float timer = 0f;
    private bool currentFocus = true;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= switchInterval)
        {
            timer = 0f;

            currentFocus = !currentFocus;

            if (FakeAttentionManager.Instance != null)
            {
                FakeAttentionManager.Instance.SetFocus(currentFocus);

                Debug.Log("AR TEST -> Focus State Changed: " + currentFocus);
            }
        }
    }
}