using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class HeartManager : MonoBehaviour
{
    public PlayerDamageFlash playerFlash;  
    private HeartSystem heartSystem;
    
    // Tell UI when hearts change
    public static event Action<int> OnHealthChanged;

    private void Start()
    {
        // Start with 3 hearts
        heartSystem = new HeartSystem();
        
        // Update UI whenever hearts change
        heartSystem.OnHeartsChanged += UpdateHealth;
        
        // Show starting hearts
        heartSystem.ResetHearts();

        // Remember which level we're on for game over screen
        GameOverController.SetLastPlayedLevel(SceneManager.GetActiveScene().name);
    }

    private void UpdateHealth(int currentHealth)
    {
        // Update the hearts display
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        // Lose hearts
        heartSystem.TakeDamage(damageAmount);

        // Flash red when hurt
        if (playerFlash != null)
        {
            playerFlash.Flash();
        }

        // Game over if no hearts left
        if (heartSystem.CurrentHearts <= 0)
        {
            Debug.Log("Game Over!");
            SceneManager.LoadScene("GameOver");
        }
    }

    public int GetCurrentHealth()
    {
        return heartSystem.CurrentHearts;
    }

    public void RestoreHealth(int healAmount)
    {
        heartSystem.RestoreHealth(healAmount);
    }
}
