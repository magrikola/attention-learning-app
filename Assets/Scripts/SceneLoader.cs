using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadGameSelection()
    {
        SceneManager.LoadScene("GameSelection");
    }

    public void LoadGame1()
    {
        SceneManager.LoadScene("Game1");
    }

    public void LoadGame2()
    {
        SceneManager.LoadScene("Game2_Matching");
    }

    public void LoadGame3()
    {
        SceneManager.LoadScene("Game3_Reaction");
    }

    public void LoadGame4()
    {
        SceneManager.LoadScene("Game4_Tracking");
    }

    public void LoadParentPanel()
    {
        SceneManager.LoadScene("ParentPanel");
    }

    public void QuitGame()
    {
        Debug.Log("Game Closed");
        Application.Quit();
    }
}