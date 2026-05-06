using UnityEngine;

public class ObjectMatcher : MonoBehaviour
{
    public string objectColor;
    public Game2Manager game2Manager;

    void OnMouseDown()
    {
        if (game2Manager != null)
        {
            game2Manager.CheckAnswer(objectColor, gameObject);
        }
    }
}