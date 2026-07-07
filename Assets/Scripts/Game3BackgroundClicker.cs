using UnityEngine;
using UnityEngine.EventSystems;

public class Game3BackgroundClicker : MonoBehaviour, IPointerClickHandler
{
    public Game3Manager game3Manager;

    void Start()
    {
        Debug.Log("Game3BackgroundClicker aktif. Attached object: " + gameObject.name);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("GAME3 BACKGROUND UI POINTER CLICKED");

        if (game3Manager == null)
        {
            Debug.LogWarning("Game3BackgroundClicker: Game3Manager bağlı değil!");
            return;
        }

        game3Manager.RegisterWrongClick();
    }
}