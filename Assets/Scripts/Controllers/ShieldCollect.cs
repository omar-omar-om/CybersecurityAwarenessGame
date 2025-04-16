using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShieldCollect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Add shield count through ScoreManager only
            ScoreManager.Instance.AddShield();

            // Destroy the shield object after pickup
            Destroy(gameObject);
        }
    }
}

