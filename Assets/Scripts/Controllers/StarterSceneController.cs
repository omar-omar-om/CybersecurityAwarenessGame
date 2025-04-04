using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StarterSceneController : MonoBehaviour
{
    public float displayTime = 5f; // Time to display the starter scene
    public string mainMenuSceneName = "MainMenu"; // Name of your main menu scene
    public bool useFade = true; // Whether to use fade transition
    public float fadeTime = 1f; // Time for fade transition
    
    // Reference to a black image for fading (optional)
    public CanvasGroup fadeCanvasGroup;

    void Start()
    {
        // Force portrait orientation
        Screen.orientation = ScreenOrientation.Portrait;
        
        // Start the transition coroutine
        StartCoroutine(TransitionToMainMenu());
    }

    void Awake()
    {
        // Ensure portrait orientation from the very beginning
        Screen.orientation = ScreenOrientation.Portrait;
    }

    IEnumerator TransitionToMainMenu()
    {
        // If using fade, make sure we start fully visible
        if (useFade && fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0;
        }

        // Wait for the display time
        yield return new WaitForSeconds(displayTime);

        // If we're using fade, fade to black
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

        // Load the main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
} 