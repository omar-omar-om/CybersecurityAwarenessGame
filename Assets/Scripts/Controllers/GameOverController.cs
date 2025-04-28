using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOverController : MonoBehaviour
{
    private static string lastPlayedLevel = "Level1"; 
    private bool isTransitioning = false;
    private Canvas gameOverCanvas;

    private void Awake()
    {
        gameOverCanvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        StartCoroutine(SetupGameOver());
    }

    private IEnumerator SetupGameOver()
    {
        // Make sure we ARE in landscape mode
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        yield return new WaitForEndOfFrame();
        
        // Show the game over screen
        if (gameOverCanvas != null)
        {
            gameOverCanvas.enabled = true;
        }
    }

    // Player wants to try the level again
    public void TryAgain()
    {
        if (!isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(LoadLevel(lastPlayedLevel));
        }
    }

    // Player gives up and goes back to menu
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
        // Hide the game over screen
        if (gameOverCanvas != null)
        {
            gameOverCanvas.enabled = false;
        }

        // Keep the game in landscape mode
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        yield return new WaitForSeconds(0.3f);
        
        // Head back to the main menu
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator LoadLevel(string levelName)
    {
        // Hide the game over screen before loading
        if (gameOverCanvas != null)
        {
            gameOverCanvas.enabled = false;
        }

        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(levelName);
    }

    // Remember which level we were playing
    public static void SetLastPlayedLevel(string levelName)
    {
        lastPlayedLevel = levelName;
    }
} 