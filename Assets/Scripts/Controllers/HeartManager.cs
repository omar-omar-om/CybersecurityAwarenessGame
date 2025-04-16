using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class HeartManager : MonoBehaviour
{
    public PlayerDamageFlash playerFlash;  
    private HeartSystem heartSystem;
    
    // Event that UI can subscribe to
    public static event Action<int> OnHealthChanged;

    private void Start()
    {
        heartSystem = new HeartSystem(); // Start with 3 hearts
        heartSystem.OnHeartsChanged += UpdateHealth;
        heartSystem.ResetHearts(); // Initial UI setup

        // Store current level name when starting
        GameOverController.SetLastPlayedLevel(SceneManager.GetActiveScene().name);
    }

    private void UpdateHealth(int health)
    {
        OnHealthChanged?.Invoke(health);
    }

    public void TakeDamage(int amount)
    {
        heartSystem.TakeDamage(amount);

        // Trigger red flash effect on player
        if (playerFlash != null)
        {
            playerFlash.Flash();
        }

        if (heartSystem.CurrentHearts <= 0)
        {
            Debug.Log("Game Over!");
            // Load the game over scene
            SceneManager.LoadScene("GameOver");
        }
    }

    public int GetCurrentHealth()
    {
        return heartSystem.CurrentHearts;
    }

    public void RestoreHealth(int amount)
    {
        heartSystem.RestoreHealth(amount);
    }
}
