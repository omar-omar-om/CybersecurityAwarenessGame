using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDamage : MonoBehaviour
{
    // how much damage this obstacle does
    public int damageAmount = 1;
    
    // how long to wait before this obstacle can hurt player again
    private float cooldownDuration = 0.5f;
    
    // can this obstacle hurt the player right now?
    private bool canDamage = true;
    
    // is player touching this obstacle?
    private bool isCollidingWithPlayer = false;

    // we need these to play sounds and take health
    private HeartManager heartManager;
    private AudioManager audioManager;

    private void Start()
    {
        // find the managers we need
        heartManager = FindObjectOfType<HeartManager>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // called when something enters  trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
            TryDamagePlayer();
        }
    }

    // called when something leaves ther trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
        }
    }

    // try to hurt the player
    private void TryDamagePlayer()
    {
        if (canDamage == false || isCollidingWithPlayer == false) return;
        
        // play hit sound
        if (audioManager != null)
        {
            audioManager.PlayHitSound();
        }
        
        // take health from player
        if (heartManager != null)
        {
            heartManager.TakeDamage(damageAmount);
        }

        // start cooldown so we can't hurt player again right away ( resolved issue i had)
        StartCoroutine(DamageCooldown());
    }

    // wait a bit before we can hurt player again
    private IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(cooldownDuration);
        canDamage = true;
    }
}

