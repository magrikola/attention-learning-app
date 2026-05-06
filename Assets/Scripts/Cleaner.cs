using UnityEngine;

public class Cleaner : MonoBehaviour
{
    void Awake()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("ALL PLAYER PREFS CLEARED");
    }
}