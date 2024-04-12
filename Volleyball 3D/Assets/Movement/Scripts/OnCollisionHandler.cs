using UnityEngine;

public class OnCollisionHandler : MonoBehaviour
{
    public bool ballDetected { get; set; } = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            ballDetected = true;
            Debug.Log("Ball");
        }
    }
}
