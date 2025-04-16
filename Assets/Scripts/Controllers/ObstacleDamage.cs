using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDamage : MonoBehaviour
{
    // how much damage this obstacle does
    public int damageAmount = 1;
    
    // how long to wait before this obstacle can hurt player again
    [SerializeField] private float cooldownDuration = 0.5f;
    
    // can this obstacle hurt the player right now?
    private bool canDamage = true;
    
    // is player touching this obstacle?
    private bool isCollidingWithPlayer = false;

    // we need these to play sounds and take health
    private HeartManager heartManager;
    private AudioManager audioManager;
    private PlayerDamageFlash playerFlash;

    private Coroutine cooldownCoroutine;  // Track the cooldown coroutine

    private void Start()
    {
        // find the managers we need
        heartManager = FindObjectOfType<HeartManager>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnDisable()
    {
        // Reset when disabled/destroyed
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
        canDamage = true;
    }

    // called when something enters  trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
            // Get the flash component when we first hit the player
            if (playerFlash == null)
            {
                playerFlash = other.GetComponent<PlayerDamageFlash>();
            }
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
        if (!canDamage || !isCollidingWithPlayer) return;
        
        // Stop any existing cooldown
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
        
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

        // Trigger the flash effect
        if (playerFlash != null)
        {
            playerFlash.Flash();
        }

        cooldownCoroutine = StartCoroutine(DamageCooldown());
    }

    // wait a bit before we can hurt player again
    private IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(cooldownDuration);
        canDamage = true;
        cooldownCoroutine = null;
    }
}

