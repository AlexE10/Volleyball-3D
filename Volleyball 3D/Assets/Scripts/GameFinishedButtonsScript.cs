using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror.Examples.NetworkRoom;

public class GameFinishedButtonsScript : MonoBehaviour
{
    public void OnStartAnotherGameClick()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnBackToMenuClick()
    {
        NetworkRoomManagerExt.singleton.StopClient();
        SceneManager.LoadScene("Menu");

    }
}