using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameWonController : MonoBehaviour
{
    public AudioClip gameWonSound; // Assign in inspector
    private AudioSource audioSource;
    private static string lastPlayedLevel = "Level1"; // Track the last played level
    private bool isTransitioning = false;
    private Canvas gameWonCanvas;

    private void Awake()
    {
        gameWonCanvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        StartCoroutine(SetupGameWon());
    }

    private IEnumerator SetupGameWon()
    {
        // Force landscape orientation for game won screen
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        
        // Wait for orientation to change
        yield return new WaitForEndOfFrame();
        
        // Show game won canvas
        if (gameWonCanvas != null)
        {
            gameWonCanvas.enabled = true;
        }

        // Setup and play game won sound
        if (gameWonSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = gameWonSound;
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.5f;
            audioSource.Play();
        }
    }

    private void OnDestroy()
    {
       
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource);
        }
    }

    public void PlayAgain()
    {
        if (!isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(LoadLevel(lastPlayedLevel));
        }
    }

    public void MainMenu()
    {
        if (!isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(TransitionToMainMenu());
        }
    }

    private IEnumerator TransitionToMainMenu()
    {
        
        // Clean up any DontDestroyOnLoad objects that might be interfering
        GameObject[] persistentObjects = GameObject.FindGameObjectsWithTag("MainMenuUI");
        foreach (GameObject obj in persistentObjects)
        {
            Destroy(obj);
        }
        
        // Check EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        

        // Hide game won canvas
        if (gameWonCanvas != null)
        {
            gameWonCanvas.enabled = false;
        }

        // Set orientation to portrait before transitioning
        Screen.orientation = ScreenOrientation.Portrait;
        
        // Wait for orientation to change
        yield return new WaitForSeconds(0.3f);
        
        // Clean up audio
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource);
        }

        // Load MainMenu scene
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator LoadLevel(string levelName)
    {
        // Hide game won canvas
        if (gameWonCanvas != null)
        {
            gameWonCanvas.enabled = false;
        }

        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(levelName);
    }

    public static void SetLastPlayedLevel(string levelName)
    {
        lastPlayedLevel = levelName;
    }
} 