using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstacle1Prefab; // Assign in Inspector
    public GameObject obstacle2Prefab; // Assign in Inspector
    public Transform spawnPoint;       // Reference point (X/Z used only)
    public float spawnRate = 2f;       // Time between spawns
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    void SpawnObstacle()
    {
        int obstacleType = Random.Range(0, 10); // 0 to 9

        if (obstacleType < 9) 
        {
            SpawnObstacle1();
        }
        else 
        {
            SpawnObstacle2();
        }
    }

    void SpawnObstacle1()
    {
        int spawnPattern = Random.Range(0, 3); // Choose random pattern

        if (spawnPattern == 0)
        {
            // Single spike - perfectly aligned
            Vector3 pos = new Vector3(spawnPoint.position.x, -3.54f, spawnPoint.position.z);
            Instantiate(obstacle1Prefab, pos, Quaternion.identity);
        }
        else if (spawnPattern == 1)
        {
            // Two spikes beside each other - perfectly aligned
            Vector3 basePos = new Vector3(spawnPoint.position.x, -3.54f, spawnPoint.position.z);
            Vector3 offset = new Vector3(1f, 0f, 0f);
            Instantiate(obstacle1Prefab, basePos, Quaternion.identity);
            Instantiate(obstacle1Prefab, basePos + offset, Quaternion.identity);
        }
        else if (spawnPattern == 2)
        {
            // Floating flipped spike
            Vector3 liftedPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y + 1.5f, spawnPoint.position.z);
            Quaternion flippedRotation = Quaternion.Euler(0, 0, 90); // Rotate left
            Instantiate(obstacle1Prefab, liftedPos, flippedRotation);
        }
    }

    void SpawnObstacle2()
    {
        // Perfectly aligned
        Vector3 pos = new Vector3(spawnPoint.position.x, -3.40f, spawnPoint.position.z);
        Instantiate(obstacle2Prefab, pos, Quaternion.identity);
    }
}
