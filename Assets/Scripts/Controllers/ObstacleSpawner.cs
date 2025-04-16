using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstacleSpawner : MonoBehaviour
{
    // Obstacle prefabs to spawn
    public GameObject obstacle1Prefab;
    public GameObject obstacle2Prefab;
    
    // Where to spawn obstacles
    public Transform spawnPoint;
    
    // How often to spawn obstacles
    public float spawnRate = 2f;
    
    // Timer to track when to spawn next
    private float timer = 0f;

    // Can obstacles spawn? this used with my question system thats why it is also static
    public static bool canSpawn = true;

    // Check if we're in Level 1
    private bool isLevel1;

    void Start()
    {
        // Check current scene
        isLevel1 = SceneManager.GetActiveScene().name == "Level1";
        
        // If in Level 1, set spawn rate to 3 seconds
        if (isLevel1)
        {
            spawnRate = 3f;
        }
    }

    void Update()
    {
        // Don't spawn if spawning is disabled
        if (canSpawn == false) return;

        // Add time to timer
        timer += Time.deltaTime;
        
        // Check if it's time to spawn
        if (timer >= spawnRate)
        {
            SpawnObstacle();
            timer = 0f;  // Reset timer
        }
    }

    // Spawn a random obstacle
    void SpawnObstacle()
    {
        // In Level 1, only spawn obstacle1
        if (isLevel1)
        {
            SpawnObstacle1();
            return;
        }

        // For Level 2, keep original random spawning
        // 90% chance for obstacle1, 10% chance for obstacle2
        int obstacleType = Random.Range(0, 10);
        if (obstacleType < 9)
        {
            SpawnObstacle1();
        }
        else
        {
            SpawnObstacle2();
        }
    }

    // Spawn obstacle type 1 with different patterns
    void SpawnObstacle1()
    {
        // Pick a random pattern (0, 1, or 2)
        int pattern = Random.Range(0, 3);

        if (pattern == 0)
        {
            // Single spike on ground
            Vector3 pos = new Vector3(spawnPoint.position.x, -3.54f, spawnPoint.position.z);
            Instantiate(obstacle1Prefab, pos, Quaternion.identity);
        }
        else if (pattern == 1)
        {
            // Two spikes next to each other
            Vector3 firstPos = new Vector3(spawnPoint.position.x, -3.54f, spawnPoint.position.z);
            Vector3 secondPos = firstPos + new Vector3(1f, 0f, 0f);
            
            Instantiate(obstacle1Prefab, firstPos, Quaternion.identity);
            Instantiate(obstacle1Prefab, secondPos, Quaternion.identity);
        }
        else if (pattern == 2)
        {
            // One spike in air
            Vector3 pos = new Vector3(spawnPoint.position.x, spawnPoint.position.y + 1.5f, spawnPoint.position.z);
            Quaternion rotation = Quaternion.Euler(0, 0, 90);  // Rotate spike left
            Instantiate(obstacle1Prefab, pos, rotation);
        }
    }

    // Spawn obstacle type 2
    void SpawnObstacle2()
    {
        Vector3 pos = new Vector3(spawnPoint.position.x, -3.40f, spawnPoint.position.z);
        Instantiate(obstacle2Prefab, pos, Quaternion.identity);
    }
}
