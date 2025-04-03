using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    // How fast to move left
    public float speed = 5f;
 
    void Update()
    {
        // Keep moving left
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Remove when off screen
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }
}
