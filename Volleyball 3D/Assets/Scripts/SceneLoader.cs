using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log("Scene Loaded: " + sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
