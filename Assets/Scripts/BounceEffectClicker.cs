using UnityEngine;
using System.Collections;

public class BounceEffectClicker : MonoBehaviour
{
    private Vector3 originalScale;

    [Header("Bounce Settings")]
    public float squashAmount = 0.8f;   
    public float stretchAmount = 1.1f;  
    public float speed = 0.01f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void PlayEffect()
    {
        StopAllCoroutines();
        StartCoroutine(BounceAnim());
    }

    IEnumerator BounceAnim()
    {
        
        transform.localScale = originalScale * squashAmount;
        yield return new WaitForSeconds(speed);

        
        transform.localScale = originalScale * stretchAmount;
        yield return new WaitForSeconds(speed);

        
        transform.localScale = originalScale;
    }
}