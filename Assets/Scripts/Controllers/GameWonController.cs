using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameWonController : MonoBehaviour
{
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
        
        // Hide game won canvas
        if (gameWonCanvas != null)
        {
            gameWonCanvas.enabled = false;
        }

        // Set orientation to portrait before transitioning
        Screen.orientation = ScreenOrientation.Portrait;
        
        // Wait for orientation to change
        yield return new WaitForSeconds(0.3f);

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