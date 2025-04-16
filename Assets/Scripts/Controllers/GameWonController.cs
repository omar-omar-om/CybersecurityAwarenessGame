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
        Debug.Log("[GameWonController] Awake called. Canvas: " + (gameWonCanvas == null ? "null" : "found"));
    }

    private void Start()
    {
        Debug.Log("[GameWonController] Start called");
        StartCoroutine(SetupGameWon());
    }

    private IEnumerator SetupGameWon()
    {
        Debug.Log("[GameWonController] Setting up Game Won screen");
        // Force landscape orientation for game won screen
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        
        // Wait for orientation to change
        yield return new WaitForEndOfFrame();
        
        // Show game won canvas
        if (gameWonCanvas != null)
        {
            gameWonCanvas.enabled = true;
            Debug.Log("[GameWonController] Game Won canvas enabled");
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
            Debug.Log("[GameWonController] Game Won sound playing");
        }
    }

    private void OnDestroy()
    {
        Debug.Log("[GameWonController] OnDestroy called");
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource);
        }
    }

    public void PlayAgain()
    {
        Debug.Log("[GameWonController] PlayAgain button clicked");
        if (!isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(LoadLevel(lastPlayedLevel));
        }
    }

    public void MainMenu()
    {
        Debug.Log("[GameWonController] MainMenu button clicked. isTransitioning: " + isTransitioning);
        if (!isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(TransitionToMainMenu());
        }
    }

    private IEnumerator TransitionToMainMenu()
    {
        Debug.Log("[GameWonController] Starting transition to MainMenu");
        
        // Clean up any DontDestroyOnLoad objects that might be interfering
        GameObject[] persistentObjects = GameObject.FindGameObjectsWithTag("MainMenuUI");
        foreach (GameObject obj in persistentObjects)
        {
            Debug.Log("[GameWonController] Destroying persistent object: " + obj.name);
            Destroy(obj);
        }
        
        // Check EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        Debug.Log("[GameWonController] EventSystem before transition: " + (eventSystem == null ? "Not found" : "Found") + 
                 ", Active: " + (eventSystem != null && eventSystem.gameObject.activeInHierarchy));

        // Hide game won canvas
        if (gameWonCanvas != null)
        {
            gameWonCanvas.enabled = false;
            Debug.Log("[GameWonController] Game Won canvas disabled");
        }

        // Set orientation to portrait before transitioning
        Screen.orientation = ScreenOrientation.Portrait;
        Debug.Log("[GameWonController] Set orientation to Portrait");
        
        // Wait for orientation to change
        Debug.Log("[GameWonController] Waiting for orientation change...");
        yield return new WaitForSeconds(0.3f);
        Debug.Log("[GameWonController] Wait complete");
        
        // Clean up audio
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource);
            Debug.Log("[GameWonController] Audio stopped and destroyed");
        }

        // Load MainMenu scene
        Debug.Log("[GameWonController] Loading MainMenu scene with complete reload");
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    private IEnumerator LoadLevel(string levelName)
    {
        Debug.Log("[GameWonController] Loading level: " + levelName);
        // Hide game won canvas
        if (gameWonCanvas != null)
        {
            gameWonCanvas.enabled = false;
            Debug.Log("[GameWonController] Game Won canvas disabled before loading level");
        }

        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(levelName);
    }

    public static void SetLastPlayedLevel(string levelName)
    {
        lastPlayedLevel = levelName;
        Debug.Log("[GameWonController] Set last played level: " + levelName);
    }
} 