using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HeartSystem
{
    // Maximum number of hearts player can have
    private int maxHearts;
    
    // Current number of hearts player has
    public int CurrentHearts { get; private set; }

    // Event that tells UI to update when hearts change
    public event Action<int> OnHeartsChanged;

    // Constructor called when creating new HeartSystem in heartmanager  
    public HeartSystem(int maxHearts = 3)
    {
        this.maxHearts = maxHearts;
        CurrentHearts = maxHearts;
    }

    // Called when player takes damage
    public void TakeDamage(int amount)
    {
        // Reduce current hearts by damage amount
        CurrentHearts -= amount;
        
        // Make sure hearts don't go below 0
        if (CurrentHearts < 0)
        {
            CurrentHearts = 0;
        }
        
        // Tell UI to update
        if (OnHeartsChanged != null)
        {
            OnHeartsChanged(CurrentHearts);
        }
    }

    // Reset hearts back to maximum
    public void ResetHearts()
    {
        CurrentHearts = maxHearts;
        
        // Tell UI to update
        if (OnHeartsChanged != null)
        {
            OnHeartsChanged(CurrentHearts);
        }
    }

    // Add health back to player
    public void RestoreHealth(int amount)
    {
        // Add health but don't go over maximum
        CurrentHearts += amount;
        if (CurrentHearts > maxHearts)
        {
            CurrentHearts = maxHearts;
        }
        
        // Tell UI to update
        if (OnHeartsChanged != null)
        {
            OnHeartsChanged(CurrentHearts);
        }
    }
}