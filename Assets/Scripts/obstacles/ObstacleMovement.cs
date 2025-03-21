using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public float speed = 5f; // Speed of the obstacle movement

    void Update()
    {
        // Move the obstacle to the left
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Destroy the obstacle when it moves off-screen
        if (transform.position.x < -10f) // Adjust the -10f 
        {
            Destroy(gameObject);
        }
    }
}
