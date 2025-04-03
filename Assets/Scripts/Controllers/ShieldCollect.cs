using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShieldCollect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Give player a shield
            ScoreManager.Instance.AddShield();

            // Remove shield from game
            Destroy(gameObject);
        }
    }
}

