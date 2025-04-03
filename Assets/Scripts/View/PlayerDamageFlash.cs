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

    // Called when game starts
    private void Awake()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Called when player takes damage
    public void Flash()
    {
        // Start the flash effect
        StartCoroutine(FlashEffect());
    }

    // Coroutine to handle the flash effect
    private IEnumerator FlashEffect()
    {
        // Save the original color
        Color originalColor = spriteRenderer.color;

        // Change to flash color (red)
        spriteRenderer.color = flashColor;

        // Wait for flash duration
        yield return new WaitForSeconds(flashDuration);

        // Change back to original color
        spriteRenderer.color = originalColor;
    }
}