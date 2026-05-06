using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Game3BackgroundClicker : MonoBehaviour
{
    public Game3Manager game3Manager;

    void OnMouseDown()
    {
        if (IsPointerOverUI())
            return;

        if (game3Manager != null)
            game3Manager.RegisterWrongClick();
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}