using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpawner : MonoBehaviour
{
    // Shield prefab to spawn
    public GameObject shieldPrefab;
    
    // Where to spawn shields
    public Transform spawnPoint;
    
    // Timer to track when to spawn next
    private float shieldTimer = 0f;
    
    // How often to spawn shields (5.5 seconds to avoid overlapping with obstacles)
    public float spawnInterval = 5.5f;

    // Can shields spawn?
    public static bool canSpawn = true;

    // Called every frame
    void Update()
    {
        // Don't spawn if spawning is disabled
        if (canSpawn == false) return;

        // Add time to timer
        shieldTimer += Time.deltaTime;

        // Check if it's time to spawn
        if (shieldTimer >= spawnInterval)
        {
            SpawnShield();
            shieldTimer = 0f;  // Reset timer
        }
    }

    // Spawn a shield at a random height
    void SpawnShield()
    {
        // Don't spawn shields during questions
        if (canSpawn == false) return;
        
        // Pick random pattern (0 = ground level, 1 = in air)
        int pattern = Random.Range(0, 2);

        if (pattern == 0)
        {
            // Spawn shield at ground level
            Vector3 pos = new Vector3(spawnPoint.position.x, -3.40f, spawnPoint.position.z);
            Instantiate(shieldPrefab, pos, Quaternion.identity);
        }
        else
        {
            // Spawn shield in air (player needs to jump)
            Vector3 pos = new Vector3(spawnPoint.position.x, spawnPoint.position.y + 1.5f, spawnPoint.position.z);
            Instantiate(shieldPrefab, pos, Quaternion.identity);
        }
    }
}