using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFinishedButtonsScript : MonoBehaviour
{
    public void OnStartAnotherGameClick()
    {
        SceneManager.LoadScene("Field1");
    }

    public void OnBackToMenuClick()
    {
        SceneManager.LoadScene("Menu");
    }
}