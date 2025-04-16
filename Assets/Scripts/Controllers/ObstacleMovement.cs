using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    // How fast the obstacle moves
    public float speed = 5f;
 
    void Update()
    {
        // Move obstacle to the left
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Destroy obstacle when it's off screen
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }
}
