using UnityEngine;

public class TargetController : MonoBehaviour
{
    public Game3Manager game3Manager;

    void OnMouseDown()
    {
        game3Manager.TargetClicked();
    }
}