using Mirror;
using Mirror.Examples.Pong;
using UnityEngine;

public class ShotManager : NetworkBehaviour
{
    [SerializeField] private float lowkickPower;
    [SerializeField] private float upperkickPower;
    [SerializeField] public GameObject ball;
    [SerializeField] public Camera playerCamera;
    [SerializeField] public GameObject lowkickHelper;
    [SerializeField, Range(1, 20)] private float lowkickRange;
    [SerializeField, Range(1, 20)] private float upperkickRange;
    [SerializeField] private float reflectedPointHeight;
    [SerializeField] private float pathHeightForUpperkick;

    public Rigidbody ballRigidbody;

    private float previousDistance;
    private Vector3 playerPositionReflectedToNet;

    private void Start()
    {
        //ball = GameObject.Find("volleyball");   
        //ballRigidbody = ball.GetComponent<Rigidbody>();
        //lowkickHelper = GameObject.Find("LowkickHelper");
        //playerPositionReflectedToNet = transform.position;
        //playerPositionReflectedToNet.z = 0f;
        //previousDistance = Vector3.Distance(transform.position, playerPositionReflectedToNet);
    }
    public void Lowkick()
    {
        if (Vector3.Distance(transform.position, ball.transform.position) < lowkickRange)
        {
            //NetworkIdentity playerNetIdentity = GetComponent<NetworkIdentity>();

            //// Get the NetworkIdentity component of the ball
            //NetworkIdentity ballNetIdentity = ball.GetComponent<NetworkIdentity>();

            //// Check if the player has authority over the ball
            //if (playerNetIdentity.connectionToClient == ballNetIdentity.connectionToClient)
            //{
            //    Debug.LogWarning("Player has authority over the ball!");
            //}
            //else
            //{
            //    Debug.LogWarning("Player does not have authority over the ball!");
            //}

            AdaptLowkickPower();

            Ray viewDirectionRay = new Ray(transform.position, playerCamera.transform.forward);
            if (lowkickHelper.GetComponent<BoxCollider>().Raycast(viewDirectionRay, out RaycastHit hit, Mathf.Infinity))
            {
                Vector3 updatedHit = hit.point;
                updatedHit.y = reflectedPointHeight;
                updatedHit -= transform.position;

                ballRigidbody.AddForce(updatedHit * lowkickPower);

                Debug.Log("It works");
            }
        }
    }

    public void UpperKick()
    {
        if (Vector3.Distance(transform.position, ball.transform.position) < upperkickRange)
        {
            AdaptUpperkickPower();

            Vector3 offset = new Vector3(0, 0.1f, 0);
            Vector3 upperkickDirection = GetReflected(offset) + new Vector3(0, pathHeightForUpperkick, 0);

            ballRigidbody.velocity = upperkickDirection * upperkickPower;
        }
    }

    private Vector3 GetReflected(Vector3 offset)
    {
        Vector3 cameraOffset = playerCamera.transform.forward + offset;
        cameraOffset.x = 0f;
        cameraOffset.z = 0f;
        Vector3 volleyballlVector = transform.position - ball.transform.position;
        Vector3 planeTangent = Vector3.Cross(volleyballlVector, cameraOffset);
        Vector3 planeNormal = Vector3.Cross(planeTangent, volleyballlVector);
        Vector3 reflected = Vector3.Reflect(cameraOffset, planeNormal);

        return reflected.normalized;
    }

    private void AdaptLowkickPower()
    {
        playerPositionReflectedToNet = transform.position;
        playerPositionReflectedToNet.z = 0;

        float currentDistance = Vector3.Distance(transform.position, playerPositionReflectedToNet);

        if (currentDistance - previousDistance > 1)
        {
            reflectedPointHeight -= 2;
            lowkickPower++;
        }
        else if (currentDistance - previousDistance < -1)
        {
            reflectedPointHeight += 2;
            lowkickPower--;
        }

        previousDistance = currentDistance;

        //Debug.Log(reflectedPointHeight);
    }

    private void AdaptUpperkickPower()
    {
        playerPositionReflectedToNet = transform.position;
        playerPositionReflectedToNet.z = 0;

        float currentDistance = Vector3.Distance(transform.position, playerPositionReflectedToNet);

        if (currentDistance - previousDistance > 1)
        {
            pathHeightForUpperkick += 0.1f;
            upperkickPower--;
        }
        else if (currentDistance - previousDistance < -1)
        {
            pathHeightForUpperkick -= 0.1f;
            upperkickPower++;   
        }

        previousDistance = currentDistance;
    }
}
