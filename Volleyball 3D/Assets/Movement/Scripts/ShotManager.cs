using Mirror;
using Mirror.Examples.Pong;
using UnityEngine;

public class ShotManager : NetworkBehaviour
{
    [SyncVar] private uint lastPlayerId = 0;

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
    public void Lowkick(uint playerId)
    {
        if (playerId == lastPlayerId)
        {
            Debug.Log("Cannot hit the ball twice in succession.");
            return;
        }
        if (Vector3.Distance(transform.position, ball.transform.position) < lowkickRange)
        {
            ballRigidbody.useGravity = true;
            AdaptLowkickPower();

            Vector3 directionToBall = (ball.transform.position - transform.position).normalized;
            float forceMagnitude = lowkickPower;
            Vector3 forceVector = directionToBall * forceMagnitude + Vector3.up * (forceMagnitude * 0.3f); 

            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.AddForce(forceVector, ForceMode.Impulse);

            lastPlayerId = playerId;

            Debug.Log($"Lowkick applied with force: {forceVector}");
        }
    }

    public void UpperKick(uint playerId)
    {
        if (playerId == lastPlayerId)
        {
            Debug.Log("Cannot hit the ball twice in succession.");
            return;
        }
        if (Vector3.Distance(transform.position, ball.transform.position) < upperkickRange)
        {
            AdaptUpperkickPower();

            // Direction from player to ball, normalized
            Vector3 directionToBall = (ball.transform.position - transform.position).normalized;

            // Adjustments to make the spike force more realistic
            Vector3 spikeDirection = new Vector3(directionToBall.x, -0.3f, directionToBall.z).normalized; // Enhance horizontal direction and add downward component

            float forceMagnitude = upperkickPower;
            Vector3 forceVector = spikeDirection * forceMagnitude;

            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.AddForce(forceVector, ForceMode.Impulse);

            lastPlayerId = playerId;

            Debug.Log($"UpperKick applied with force: {forceVector}");
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
        float distanceToNet = Mathf.Abs(transform.position.z);
        lowkickPower = Mathf.Clamp(lowkickPower, 10, 15); // Example clamp, adjust based on testing
    }

    private void AdaptUpperkickPower()
    {
        float distanceToNet = Mathf.Abs(transform.position.z); // Assuming net is along z=0
        upperkickPower = Mathf.Clamp(upperkickPower, 15, 30); // Example clamp, adjust based on testing
    }
}
