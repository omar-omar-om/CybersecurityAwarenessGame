using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CountdownController : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float countdownDuration = 1f; // Duration for each number
    public AudioClip countdownSound; // Sound to play for each number
    private AudioSource audioSource;

    void Start()
    {
        // Initially hide the text
        countdownText.gameObject.SetActive(false);
        
        // Set up basic audio source
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
    }
} 