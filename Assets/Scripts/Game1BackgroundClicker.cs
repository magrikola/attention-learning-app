using UnityEngine;
using UnityEngine.EventSystems;

public class Game1BackgroundClicker : MonoBehaviour, IPointerClickHandler
{
    public Game1Manager game1Manager;

    void Start()
    {
        Debug.Log("Game1BackgroundClicker aktif. Attached object: " + gameObject.name);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("BACKGROUND UI POINTER CLICKED");

        if (game1Manager == null)
        {
            Debug.LogWarning("Game1BackgroundClicker: Game1Manager bağlı değil!");
            return;
        }

        game1Manager.RegisterWrongClick();
    }
}