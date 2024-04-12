using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void OnStartGameClick()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnHowToPlayClick()
    {
        SceneManager.LoadScene("HowToPlay");
    }
}