using UnityEngine;

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
        transform.position = initialPosition;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
    }
}