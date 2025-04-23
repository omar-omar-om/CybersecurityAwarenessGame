using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarterSceneController : MonoBehaviour
{
    public float displayTime = 5f; // Time to display the starter scene
    public string mainMenuSceneName = "MainMenu"; // Name of  main menu scene
    public bool useFade = true; // Whether to use fade transition
    public float fadeTime = 1f; // Time for fade transition
    
    public CanvasGroup fadeCanvasGroup;

    private void Awake()
    {
        // Ensure landscape orientation from the very beginning
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    void Start()
    {
        // Start the transition coroutine
        StartCoroutine(CheckLoginAndTransition());
    }

    private IEnumerator CheckLoginAndTransition()
    {
        // If using fade, make sure we start fully visible
        if (useFade && fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0;
        }

        // Wait for the display time
        yield return new WaitForSeconds(displayTime);

        // Check if the user is logged in (global flag)
        int isLoggedIn = PlayerPrefs.GetInt("isLoggedIn", 0);
        
        // Get the last used email from PlayerPrefs
        string lastEmail = PlayerPrefs.GetString("lastLoginEmail", "");
        
        
        if (isLoggedIn == 1 && !string.IsNullOrEmpty(lastEmail))
        {
            // User is logged in, go to MainMenu
            
            if (useFade && fadeCanvasGroup != null)
            {
                // Fade out
                float elapsedTime = 0;
                while (elapsedTime < fadeTime)
                {
                    elapsedTime += Time.deltaTime;
                    fadeCanvasGroup.alpha = elapsedTime / fadeTime;
                    yield return null;
                }
            }
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            // Not logged in, go to Login
            
            if (useFade && fadeCanvasGroup != null)
            {
                // Fade out
                float elapsedTime = 0;
                while (elapsedTime < fadeTime)
                {
                    elapsedTime += Time.deltaTime;
                    fadeCanvasGroup.alpha = elapsedTime / fadeTime;
                    yield return null;
                }
            }
            SceneManager.LoadScene("Login");
        }
    }
} 