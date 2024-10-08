using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : NetworkBehaviour
{
    Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    public override void OnStartLocalPlayer()
    {
        if (mainCam != null)
        {
            mainCam.orthographic = false;
            mainCam.transform.SetParent(transform);
            mainCam.transform.localPosition = GetComponent<PlayerController>().playerCameraPosition;
            mainCam.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

            GetComponent<PlayerController>().playerCamera = mainCam;
            GetComponent<PlayerController>().shotManager.playerCamera = mainCam;
        }
        else
            Debug.LogWarning("PlayerCamera: Could not find a camera in scene with 'MainCamera' tag.");
    }

    public override void OnStopLocalPlayer()
    {
        if (mainCam != null)
        {
            mainCam.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(mainCam.gameObject, SceneManager.GetActiveScene());
            mainCam.orthographic = true;
            mainCam.orthographicSize = 15f;
            mainCam.transform.localPosition = new Vector3(0f, 70f, 0f);
            mainCam.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        }
    }
}
