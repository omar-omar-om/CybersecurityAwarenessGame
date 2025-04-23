using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // here is all audio source which is important to play the clips
    private AudioSource backgroundMusic;
    private AudioSource hitSound;
    private AudioSource shieldCollect;
    private AudioSource correctAnswer;
    private AudioSource wrongAnswer;
    // clips in inspector to be set
    public AudioClip backgroundMusicClip;
    public AudioClip hitSoundClip;
    public AudioClip shieldCollectClip;
    public AudioClip correctAnswerClip;
    public AudioClip wrongAnswerClip;

    
    public float musicVolume = 0.15f; 
    public float hitVolume = 0.3f; 
    public float shieldVolume = 0.4f; 
    public float answerVolume = 0.35f; 

    private void Start()
    {
        // Add AudioSource 
        // could be added from unity easier but im just trying diff approaches
        backgroundMusic = gameObject.AddComponent<AudioSource>();
        hitSound = gameObject.AddComponent<AudioSource>();
        shieldCollect = gameObject.AddComponent<AudioSource>();
        correctAnswer = gameObject.AddComponent<AudioSource>();
        wrongAnswer = gameObject.AddComponent<AudioSource>();
        
        if (backgroundMusicClip != null)
        {
            // Configure the background music audio source
            backgroundMusic.clip = backgroundMusicClip;
            backgroundMusic.volume = musicVolume;
            backgroundMusic.loop = true; // Make it loop continuously
            backgroundMusic.playOnAwake = true;
            
            backgroundMusic.Play();
        }
        

        // Configure the hit sound audio source
        if (hitSoundClip != null)
        {
            hitSound.clip = hitSoundClip;
            hitSound.volume = hitVolume;
            hitSound.loop = false;
            hitSound.playOnAwake = false;
        }

        // Configure the shield collection audio source
        if (shieldCollectClip != null)
        {
            shieldCollect.clip = shieldCollectClip;
            shieldCollect.volume = shieldVolume;
            shieldCollect.loop = false;
            shieldCollect.playOnAwake = false;
        }

        // Configure the correct answer audio source
        if (correctAnswerClip != null)
        {
            correctAnswer.clip = correctAnswerClip;
            correctAnswer.volume = answerVolume;
            correctAnswer.loop = false;
            correctAnswer.playOnAwake = false;
        }

        // Configure the wrong answer audio source
        if (wrongAnswerClip != null)
        {
            wrongAnswer.clip = wrongAnswerClip;
            wrongAnswer.volume = answerVolume;
            wrongAnswer.loop = false;
            wrongAnswer.playOnAwake = false;
        }
        
        // Wait one frame to make sure ShieldManager is initialized ( resolved issue i had)
        StartCoroutine(SetupShieldAudio());
        
        // Find and subscribe to the QuestionModel
        QuestionModel questionModel = FindObjectOfType<QuestionModel>();
        if (questionModel != null)
        {
            questionModel.OnAnswerProcessed += HandleAnswerProcessed;
        }
    }

    private IEnumerator SetupShieldAudio()
    {
        yield return null;
        if (ShieldManager.Instance != null)
        {
            ShieldManager.Instance.OnShieldCountChanged += HandleShieldCollected;
        }
    }

    private void OnDestroy()
    {
        if (ShieldManager.Instance != null)
        {
            ShieldManager.Instance.OnShieldCountChanged -= HandleShieldCollected;
        }
        
        // Unsubscribe from question events
        QuestionModel questionModel = FindObjectOfType<QuestionModel>();
        if (questionModel != null)
        {
            questionModel.OnAnswerProcessed -= HandleAnswerProcessed;
        }
    }

    public void PlayHitSound()
    {
        // Only play if we have a clip and the sound is not already playing
        if (hitSoundClip != null && !hitSound.isPlaying)
        {
            hitSound.Play();
        }
    }

    private void HandleShieldCollected(int shieldCount)
    {
        // Only play the sound when shield count increases
        if (shieldCollectClip != null && shieldCount > 0)
        {
            shieldCollect.Play();
        }
    }

    private void HandleAnswerProcessed(bool isCorrect)
    {
        if (isCorrect == true && correctAnswerClip != null)
        {
            correctAnswer.Play();
        }
        else if (isCorrect == false && wrongAnswerClip != null)
        {
            wrongAnswer.Play();
        }
    }

    // Method to adjust volume (can be called from other scripts if needed)
    public void SetVolume(float volume)
    {
        // Make sure volume is not less than 0
        if (volume < 0)
        {
            volume = 0;
        }
        // Make sure volume is not more than 1
        if (volume > 1)
        {
            volume = 1;
        }
        
        musicVolume = volume;
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = musicVolume;
        }
    }
} 