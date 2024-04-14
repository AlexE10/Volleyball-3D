using Unity.VisualScripting;
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
        //Vector3 newPosition = initialPosition - new Vector3(0, 0, 1);
        //transform.position = newPosition;
        //Rigidbody rb = GetComponent<Rigidbody>();
        //rb.velocity = Vector3.zero;
        //initialPosition = newPosition;
        transform.position = initialPosition;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
    }
}