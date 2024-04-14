using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 5f;
    private int currentWaypointIndex = 0;
    [SerializeField] GameObject[] carsPrefabs;

    private bool instantiate;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;   
        instantiate = true;
    }

    void Update()
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            if(currentWaypointIndex == 2 && instantiate)
            {
                int randomCarPrefab = Random.Range(0, carsPrefabs.Length);

                GameObject newCar = Instantiate(carsPrefabs[randomCarPrefab], initialPosition, Quaternion.identity);

                newCar.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

                newCar.AddComponent<CarController>();   

                CarController controller = newCar.GetComponent<CarController>();    
                controller.waypoints = waypoints;
                controller.moveSpeed = moveSpeed;
                controller.currentWaypointIndex = 0;
                controller.carsPrefabs = carsPrefabs;   

                instantiate = false;
            }

            Transform currentWaypoint = waypoints[currentWaypointIndex];

            Vector3 direction = currentWaypoint.position - transform.position;
            direction.Normalize();

            transform.position += direction * moveSpeed * Time.deltaTime;

            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, angle, 0);

            if (Vector3.Distance(transform.position, currentWaypoint.position) < 0.1f)
            {
                currentWaypointIndex++;
                instantiate = true;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
