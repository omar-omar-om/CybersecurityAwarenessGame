using UnityEngine;

public class MainMenuMusicController : MonoBehaviour
{
    public AudioClip menuMusic;
    private AudioSource audioSource;

    private void Awake()
    {
        // Set up audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.Play();
    }
} 