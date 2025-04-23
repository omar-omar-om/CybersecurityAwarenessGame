using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColorChanger : MonoBehaviour
{
    // The SpriteRenderer component of the background
    private SpriteRenderer backgroundRenderer;

    // Colors to cycle through
    private Color[] colors = new Color[] {
        new Color(1f, 1f, 1f),          // FFFFFF (White)
        new Color(0f, 0.56f, 1f),       // 008FFF (Blue)
        new Color(0f, 1f, 0.31f)        // 00FF50 (Green-ish)
    };

    // Current color index
    private int currentColorIndex = 0;
    
    // Base transition duration
    public float baseDuration = 3.0f;
    
    // Extra time for blue color (index 1)
    public float extraBlueTime = 3.0f;
    
    // Track transition progress
    private float transitionProgress = 0f;

    private Color currentColor;
    private Color targetColor;

    private void Awake()
    {
        // Get the background renderer
        backgroundRenderer = GetComponent<SpriteRenderer>();
        currentColor = colors[0];
        targetColor = colors[1];
    }

    private void Start()
    {
        // Any additional initialization can go here
    }

    private void Update()
    {
        if (backgroundRenderer == null) return;

        // Get current transition duration based on color
        float currentDuration = (currentColorIndex == 1) ? baseDuration + extraBlueTime : baseDuration;

        // Update transition progress
        transitionProgress += Time.deltaTime / currentDuration;

        // If we've completed the transition
        if (transitionProgress >= 1f)
        {
            // Move to next color
            currentColorIndex = (currentColorIndex + 1) % colors.Length;
            transitionProgress = 0f;
        }

        // Calculate next color index
        int nextColorIndex = (currentColorIndex + 1) % colors.Length;

        // Smoothly lerp between current color and next color
        Color newColor = Color.Lerp(colors[currentColorIndex], colors[nextColorIndex], transitionProgress);
        backgroundRenderer.color = newColor;
    }
} 