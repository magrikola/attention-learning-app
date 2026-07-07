using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectMatcher : MonoBehaviour, IPointerClickHandler
{
    public string objectColor;
    public Game2Manager game2Manager;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (game2Manager != null)
        {
            game2Manager.CheckAnswer(objectColor, gameObject);
        }
    }
}