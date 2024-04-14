using Mirror;
using Mirror.Examples.NetworkRoom;
using UnityEngine;

public class OnCollisionHandler : NetworkBehaviour
{
    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        PlayerController[] controller = FindObjectsOfType<PlayerController>();

        PlayerScore currentPlayScore;
        if (collision.collider.tag == "0")
        {
            currentPlayScore = controller[0].GetComponent<PlayerScore>();
        }
        else
        {
            currentPlayScore = controller[1].GetComponent<PlayerScore>();
        }

        int otherIndex = 0;
        if (currentPlayScore.index == 0)
        {
            otherIndex = 1;
        }

        if (collision.collider.tag == otherIndex.ToString())
        {
            Debug.Log("Merge");
            currentPlayScore.UpdateScore(true);
        }
    }
}
