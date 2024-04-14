using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Examples.NetworkRoom
{
    public class Spawner
    {
        [ServerCallback]
        internal static void InitialSpawn()
        {
            for (int i = 0; i < 10; i++)
                SpawnReward();
        }

        [ServerCallback]
        internal static void SpawnReward()
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-19, 20), 1, Random.Range(-19, 20));
            NetworkServer.Spawn(Object.Instantiate(NetworkRoomManagerExt.singleton.rewardPrefab, spawnPosition, Quaternion.identity));
        }

        [ServerCallback]
        internal static void SpawnBallRandom() 
        {
            var startPositions = NetworkManager.startPositions;

            int spawnIndex = (int)Time.time % startPositions.Count; 

            if (startPositions.Count == 0)
            {
                Debug.LogError("No Network Start Positions set.");
                return;
            }

            // Create the ball at the chosen start position
            Vector3 spawnPosition = startPositions[spawnIndex].position;
            NetworkRoomManagerExt.singleton.lastUsedSpawnIndex = spawnIndex;
            NetworkServer.Spawn(Object.Instantiate(NetworkRoomManagerExt.singleton.rewardPrefab, spawnPosition, Quaternion.identity));
        }

        //[ServerCallback]
        //public static void RespawnBallAtOppositePoint()
        //{
        //    var startPositions = NetworkManager.startPositions;

        //    if (startPositions.Count < 2)
        //    {
        //        Debug.LogError("Insufficient Network Start Positions set. At least 2 required.");
        //        return;
        //    }

        //    // Assuming spawnIndex is globally tracked:
        //    int lastSpawnIndex = NetworkRoomManagerExt.singleton.lastUsedSpawnIndex;
        //    int spawnIndex = (lastSpawnIndex + 1) % 2;  // Toggle between 0 and 1

        //    Vector3 spawnPosition = startPositions[spawnIndex].position;
        //    GameObject ball = Object.Instantiate(NetworkRoomManagerExt.singleton.rewardPrefab, spawnPosition, Quaternion.identity);
        //    NetworkServer.Spawn(ball);

        //    // Update last used spawn index
        //    NetworkRoomManagerExt.singleton.lastUsedSpawnIndex = spawnIndex;
        //}
    }
}
