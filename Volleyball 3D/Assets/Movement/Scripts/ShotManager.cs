using UnityEngine;

public class ShotManager : MonoBehaviour
{
    [SerializeField] private float lowkickPower;
    [SerializeField] private float upperkickPower;
    [SerializeField] public GameObject ball;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject lowkickHelper;

    private Rigidbody ballRigidbody;

    private void Start()
    {
        ballRigidbody = ball.GetComponent<Rigidbody>(); 
    }

    public void Lowkick()
    {
        Ray viewDirectionRay = new Ray(transform.position, playerCamera.transform.forward);
        if (lowkickHelper.GetComponent<BoxCollider>().Raycast(viewDirectionRay, out RaycastHit hit, Mathf.Infinity))
        {
            Vector3 updatedHit = hit.point;
            updatedHit.y = 50;
            updatedHit -= transform.position;

            ballRigidbody.AddForce(updatedHit * lowkickPower);
        }
    }

    public void UpperKick()
    {
        Vector3 offset = new Vector3(0, 0.1f, 0);
        Vector3 upperkickDirection = GetReflected(offset) + new Vector3(0, 0.3f, 0);

        ballRigidbody.velocity = upperkickDirection * upperkickPower;
    }

    private Vector3 GetReflected(Vector3 offset)
    {
        Vector3 cameraOffset = playerCamera.transform.forward + offset;
        cameraOffset.x = 0f;
        cameraOffset.z = 0f;
        Debug.Log(cameraOffset);
        Vector3 volleyballlVector = transform.position - ball.transform.position;
        Vector3 planeTangent = Vector3.Cross(volleyballlVector, cameraOffset);
        Vector3 planeNormal = Vector3.Cross(planeTangent, volleyballlVector);
        Vector3 reflected = Vector3.Reflect(cameraOffset, planeNormal);

        return reflected.normalized;
    }
}
