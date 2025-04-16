using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageFlash : MonoBehaviour
{
    // Component that controls the player's color
    private SpriteRenderer spriteRenderer;
    
    // Color to flash when taking damage
    public Color flashColor = Color.red;
    
    // How long to show the flash
    public float flashDuration = 0.1f;

    private Color originalColor;
    private Coroutine flashCoroutine;

    // Called when game starts
    private void Awake()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;  // Store original color at start
    }

    private void OnDisable()
    {
        // Make sure we reset color when disabled
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
    }

    // Called when player takes damage
    public void Flash()
    {
        // Stop existing flash if it's running
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        
        // Start new flash
        flashCoroutine = StartCoroutine(FlashEffect());
    }

    // Coroutine to handle the flash effect
    private IEnumerator FlashEffect()
    {
        // Change to flash color
        spriteRenderer.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        // Always reset to original color
        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }
}