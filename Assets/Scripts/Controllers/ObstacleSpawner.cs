using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    // The yellow spikes
    public GameObject yellowObstaclePrefab;
    
    // The red spikes
    public GameObject redObstaclePrefab;
    
    // Where to spawn obstacles
    public Transform spawnPoint;
    
    // Wait 2 seconds between spawns
    public float timeBetweenSpawns = 2f;
    
    // Keep track of time
    private float spawnTimer = 0f;

    // Stop spawning during questions
    public static bool canSpawn = true;

    void Update()
    {
        // Don't spawn if spawning is disabled
        if (canSpawn == false) return;

        // Add time to timer
        spawnTimer += Time.deltaTime;
        
        // Check if it's time to spawn
        if (spawnTimer >= timeBetweenSpawns)
        {
            SpawnObstacle();
            spawnTimer = 0f;  // Reset timer
        }
    }

    // Spawn a random obstacle
    void SpawnObstacle()
    {
        // Mostly spawn yellow spikes (90%), sometimes red (10%)
        int randomNumber = Random.Range(0, 10);
        
        if (randomNumber < 9)
        {
            SpawnYellowObstacle();
        }
        else
        {
            SpawnRedObstacle();
        }
    }

    // Spawn yellow obstacle type 1 with different patterns
    void SpawnYellowObstacle()
    {
        // Pick a random pattern (0, 1, or 2)
        int patternNumber = Random.Range(0, 3);

        if (patternNumber == 0)
        {
            // Just one spike on the ground
            Vector3 groundPosition = new Vector3(spawnPoint.position.x, -3.54f, spawnPoint.position.z);
            Instantiate(yellowObstaclePrefab, groundPosition, Quaternion.identity);
        }
        else if (patternNumber == 1)
        {
            // Two spikes next to each other
            Vector3 firstSpikePosition = new Vector3(spawnPoint.position.x, -3.54f, spawnPoint.position.z);
            Vector3 secondSpikePosition = firstSpikePosition + new Vector3(1f, 0f, 0f);
            
            Instantiate(yellowObstaclePrefab, firstSpikePosition, Quaternion.identity);
            Instantiate(yellowObstaclePrefab, secondSpikePosition, Quaternion.identity);
        }
        else
        {
            // One spike hanging from the ceiling
            Vector3 airPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y + 1.5f, spawnPoint.position.z);
            Quaternion leftRotation = Quaternion.Euler(0, 0, 90);
            Instantiate(yellowObstaclePrefab, airPosition, leftRotation);
        }
    }

    // Spawn red obstacle type 2
    void SpawnRedObstacle()
    {
        // Red spikes always spawn on the ground
        Vector3 position = new Vector3(spawnPoint.position.x, -3.40f, spawnPoint.position.z);
        Instantiate(redObstaclePrefab, position, Quaternion.identity);
    }
}
