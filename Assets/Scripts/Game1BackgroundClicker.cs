using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Game1BackgroundClicker : MonoBehaviour
{
    public Game1Manager game1Manager;

    void OnMouseDown()
    {
        if (IsPointerOverUI())
            return;

        if (game1Manager != null)
            game1Manager.RegisterWrongClick();
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