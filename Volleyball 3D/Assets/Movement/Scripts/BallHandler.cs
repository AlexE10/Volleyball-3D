using Unity.VisualScripting;
using UnityEngine;
using Mirror.Examples.NetworkRoom;
using Mirror;

public class BallHandler : MonoBehaviour
{
    private Vector3 initialPosition;
    [SerializeField] private float fallSpeed;
    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //Vector3 newPosition = initialPosition - new Vector3(0, 0, 1);
        //transform.position = newPosition;
        //Rigidbody rb = GetComponent<Rigidbody>();
        //rb.velocity = Vector3.zero;
        //initialPosition = newPosition;
        if (collision.collider.CompareTag("Ground"))
        {
            var startPositions = NetworkManager.startPositions;

            if (startPositions.Count < 2)
            {
                Debug.LogError("Insufficient Network Start Positions set. At least 2 required.");
                return;
            }

            int lastSpawnIndex = NetworkRoomManagerExt.singleton.lastUsedSpawnIndex;
            int spawnIndex = (lastSpawnIndex + 1) % 2;  // This should correctly toggle between 0 and 1

            Debug.Log($"Old Index: {lastSpawnIndex}, New Index: {spawnIndex}");
            Debug.Log($"Moving to position: {startPositions[spawnIndex].position}");

            
            transform.position = startPositions[spawnIndex].position;
            NetworkRoomManagerExt.singleton.lastUsedSpawnIndex = spawnIndex;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
        }
    }
}