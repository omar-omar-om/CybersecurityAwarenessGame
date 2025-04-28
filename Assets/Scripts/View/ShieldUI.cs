using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShieldUI : MonoBehaviour
{
    // Text that shows shield count
    public TMP_Text shieldText;

    // Called when game starts
    private void Start()
    {
        StartCoroutine(SetupShieldUI());
    }

    // Setup the shield UI
    private IEnumerator SetupShieldUI()
    {
        // Wait one frame
        yield return null;

        // Connect to ShieldManager events
        if (ShieldManager.Instance != null)
        {
            ShieldManager.Instance.OnShieldCountChanged += UpdateShieldText;
        }
    }

    // Update the shield count text
    private void UpdateShieldText(int count)
    {
        shieldText.text = "Shield: " + count;
    }
}
