using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class GameOverController : MonoBehaviour
{
    public AudioClip gameOverSound; // Assign in inspector
    private AudioSource audioSource;
    private static string lastPlayedLevel = "Level1"; // Track the last played level
    private bool isTransitioning = false;
    private Canvas gameOverCanvas;

    private void Awake()
    {
        gameOverCanvas = GetComponent<Canvas>();
        Debug.Log("[GameOverController] Awake called. Canvas: " + (gameOverCanvas == null ? "null" : "found"));
    }

    private void Start()
    {
        Debug.Log("[GameOverController] Start called");
        StartCoroutine(SetupGameOver());
    }

    private IEnumerator SetupGameOver()
    {
        Debug.Log("[GameOverController] Setting up Game Over screen");
        // Force landscape orientation for game over screen
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        
        // Wait for orientation to change
        yield return new WaitForEndOfFrame();
        
        // Show game over canvas
        if (gameOverCanvas != null)
        {
            gameOverCanvas.enabled = true;
            Debug.Log("[GameOverController] Game Over canvas enabled");
        }

        // Setup and play game over sound
        if (gameOverSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = gameOverSound;
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.5f;
            audioSource.Play();
            Debug.Log("[GameOverController] Game Over sound playing");
        }
    }

    private void OnDestroy()
    {
        Debug.Log("[GameOverController] OnDestroy called");
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource);
        }
    }

    public void TryAgain()
    {
        Debug.Log("[GameOverController] TryAgain button clicked");
        if (!isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(LoadLevel(lastPlayedLevel));
        }
    }

    public void MainMenu()
    {
        Debug.Log("[GameOverController] MainMenu button clicked. isTransitioning: " + isTransitioning);
        if (!isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(TransitionToMainMenu());
        }
    }

    private IEnumerator TransitionToMainMenu()
    {
        Debug.Log("[GameOverController] Starting transition to MainMenu");
        
        // Clean up any DontDestroyOnLoad objects that might be interfering
        GameObject[] persistentObjects = GameObject.FindGameObjectsWithTag("MainMenuUI");
        foreach (GameObject obj in persistentObjects)
        {
            Debug.Log("[GameOverController] Destroying persistent object: " + obj.name);
            Destroy(obj);
        }
        
        // Check EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        Debug.Log("[GameOverController] EventSystem before transition: " + (eventSystem == null ? "Not found" : "Found") + 
                 ", Active: " + (eventSystem != null && eventSystem.gameObject.activeInHierarchy));

        // Hide game over canvas
        if (gameOverCanvas != null)
        {
            gameOverCanvas.enabled = false;
            Debug.Log("[GameOverController] Game Over canvas disabled");
        }

        // Set orientation to portrait before transitioning
        Screen.orientation = ScreenOrientation.Portrait;
        Debug.Log("[GameOverController] Set orientation to Portrait");
        
        // Wait for orientation to change - INCREASING THE DELAY
        Debug.Log("[GameOverController] Waiting for orientation change...");
        yield return new WaitForSeconds(0.3f); // Increased from 0.1f to 0.3f
        Debug.Log("[GameOverController] Wait complete");
        
        // Clean up audio
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource);
            Debug.Log("[GameOverController] Audio stopped and destroyed");
        }

        // SOLUTION: Use LoadSceneMode.Single to completely reload the scene, 
        // which will ensure all button references are properly set
        Debug.Log("[GameOverController] Loading MainMenu scene with complete reload");
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    private IEnumerator LoadLevel(string levelName)
    {
        Debug.Log("[GameOverController] Loading level: " + levelName);
        // Hide game over canvas
        if (gameOverCanvas != null)
        {
            gameOverCanvas.enabled = false;
            Debug.Log("[GameOverController] Game Over canvas disabled before loading level");
        }

        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(levelName);
    }

    public static void SetLastPlayedLevel(string levelName)
    {
        lastPlayedLevel = levelName;
        Debug.Log("[GameOverController] Set last played level: " + levelName);
    }
} 