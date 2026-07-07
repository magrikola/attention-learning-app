using UnityEngine;
using UnityEngine.EventSystems;

public class TargetController : MonoBehaviour, IPointerClickHandler
{
    public Game3Manager game3Manager;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (game3Manager != null)
            game3Manager.TargetClicked();
    }
}