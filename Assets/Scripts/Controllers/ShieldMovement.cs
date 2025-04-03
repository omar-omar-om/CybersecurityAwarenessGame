using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMovement : MonoBehaviour
{
    // How fast to move left
    public float moveSpeed = 5f;

    // Called every frame
    void Update()
    {
        // Keep moving left
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // Remove when off screen
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }
}
