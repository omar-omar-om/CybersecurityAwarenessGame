using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMovement : MonoBehaviour
{
    // How fast the shield moves
    public float moveSpeed = 5f;

    // Called every frame
    void Update()
    {
        // Move shield to the left
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // Destroy shield when it's off screen
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }
}
