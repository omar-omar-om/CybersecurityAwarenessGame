using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownController : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float countdownDuration = 1f; // Duration for each number
    public AudioClip countdownSound; // Sound to play for each number
    private AudioSource audioSource;
    private PlayerJump playerJump;
    private GroundVFX groundVFX;

    void Start()
    {
        // Find the player's PlayerJump component
        playerJump = FindObjectOfType<PlayerJump>();
        groundVFX = FindObjectOfType<GroundVFX>();
        
        // Initially hide the text
        countdownText.gameObject.SetActive(false);
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        // Start the countdown
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        // Disable obstacle spawning during countdown
        ObstacleSpawner.canSpawn = false;
        
        // Wait a brief moment before starting
        yield return new WaitForSeconds(0.5f);
        
        countdownText.gameObject.SetActive(true);

        // Play sound once at start if assigned
        if (countdownSound != null)
        {
            audioSource.clip = countdownSound;
            audioSource.Play();
        }
        
        // Countdown from 3 to 1
        for (int i = 3; i >= 1; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(countdownDuration);
        }
        
        // Show "GO!" message
        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        
        // Hide the text
        countdownText.gameObject.SetActive(false);
        
        // Enable obstacle spawning
        ObstacleSpawner.canSpawn = true;

        // Start the player's animation and particles
        playerJump.StartAnimation();

        // Start the ground VFX
        if (groundVFX != null)
        {
            groundVFX.StartVFX();
        }
    }
} 