using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShieldManager : MonoBehaviour
{
    // Make ShieldManager accessible from other scripts
    public static ShieldManager Instance;

    // Track how many shields player has
    private int shieldCount = 0;

    // Tell UI to update when shield count changes
    public event Action<int> OnShieldCountChanged;

    // Called when game starts
    private void Awake()
    {
        // Setup singleton
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Add one shield to count
    public void AddShield()
    {
        shieldCount++;
        
        // Tell UI to update
        if (OnShieldCountChanged != null)
        {
            OnShieldCountChanged(shieldCount);
        }
    }

    // Get current number of shields
    public int GetShieldCount()
    {
        return shieldCount;
    }

    // Reset shield count back to 0
    public void ResetShieldCount()
    {
        shieldCount = 0;
        
        // Tell UI to update
        if (OnShieldCountChanged != null)
        {
            OnShieldCountChanged(shieldCount);
        }
    }
}
