using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartUI : MonoBehaviour
{
    // Array of heart images in the UI
    public GameObject[] heartImages;

    private void OnEnable()
    {
        HeartManager.OnHealthChanged += UpdateHearts;
    }

    private void OnDisable()
    {
        HeartManager.OnHealthChanged -= UpdateHearts;
    }

    // Show/hide hearts based on current health
    public void UpdateHearts(int currentHealth)
    {
        // Loop through all heart images
        for (int i = 0; i < heartImages.Length; i++)
        {
            // Show heart if player has this much health, hide if not
            // Example: if health is 2, show first 2 hearts (index 0 and 1)
            heartImages[i].SetActive(i < currentHealth);
        }
    }
}
