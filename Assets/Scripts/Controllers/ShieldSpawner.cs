using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpawner : MonoBehaviour
{
    // The shield to spawn
    public GameObject shieldPrefab;
    
    // Where to put the shields
    public Transform spawnPoint;
    
    // Keep track of time
    private float shieldTimer = 0f;
    
    // Wait 5.5 seconds between shields (so they don't hit obstacles)
    public float spawnInterval = 5.5f;

    // Stop spawning during questions
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

    // Spawn a shield in a random position
    void SpawnShield()
    {
        // Sometimes on ground, sometimes in air
        int pattern = Random.Range(0, 2);

        if (pattern == 0)
        {
            // Put shield on the ground
            Vector3 pos = new Vector3(spawnPoint.position.x, -3.40f, spawnPoint.position.z);
            Instantiate(shieldPrefab, pos, Quaternion.identity);
        }
        else
        {
            // Put shield up high (need to jump)
            Vector3 pos = new Vector3(spawnPoint.position.x, spawnPoint.position.y + 1.5f, spawnPoint.position.z);
            Instantiate(shieldPrefab, pos, Quaternion.identity);
        }
    }
}