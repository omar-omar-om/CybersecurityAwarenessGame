using UnityEngine;

public class MainMenuMusicController : MonoBehaviour
{
    public AudioClip menuMusic;
    private AudioSource audioSource;
    private static MainMenuMusicController instance;

    void Awake()
    {
        // Ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Set up audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.Play();
    }

    void OnEnable()
    {
        // Subscribe to scene loading events
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from scene loading events
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Check if we're in a level scene
        if (scene.name.Contains("Level"))
        {
            // Stop menu music when entering a level
            audioSource.Stop();
        }
        else if (scene.name == "MainMenu")
        {
            // Resume music when returning to main menu
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    // Public method to stop music (can be called from other scripts if needed)
    public void StopMusic()
    {
        audioSource.Stop();
    }

    // Public method to resume music (can be called from other scripts if needed)
    public void ResumeMusic()
    {
        audioSource.Play();
    }
} 