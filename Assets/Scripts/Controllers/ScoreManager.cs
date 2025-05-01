using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class ScoreManager : MonoBehaviour
{
    // This makes our ScoreManager accessible from other scripts
    public static ScoreManager Instance;
    
    // Reference to the score text in UI
    public TMP_Text scoreText;
    // Reference to the best score text in UI
    public TMP_Text bestScoreText;
    
    // Variables to track game state
    private int score = 0;
    private int bestScore = 0;
    private int shields = 0;
    private int correctAnswersInARow = 0;
    private HeartManager heartManager;
    private bool isBettingAllShields = false;
    private int savedShields = 0;
    
    // Called when the script starts
    private void Awake()
    {
        // Singleton setup for current scene only
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Find the HeartManager in the game
        heartManager = FindObjectOfType<HeartManager>();
        FindScoreTexts();
        
        // Load the best score for the current level
        LoadBestScore();
        
        // Show initial score of 0
        UpdateScoreDisplay();
    }

    private void OnLevelWasLoaded(int level)
    {
        // Check if current score is higher than best score before resetting
        StartCoroutine(UpdateBestScoreCoroutine());
        
        // Reset score when loading a new level
        ResetScore();
        
        // Update references when new level loads
        heartManager = FindObjectOfType<HeartManager>();
        FindScoreTexts();
        
        // Load the best score for the new level
        LoadBestScore();
        
        UpdateScoreDisplay();
    }

    private void FindScoreTexts()
    {
        // Find the score text specifically by name
        GameObject scoreTextObj = GameObject.Find("ScoreText");
        if (scoreTextObj != null)
        {
            scoreText = scoreTextObj.GetComponent<TMP_Text>();
        }
        
        // Find the best score text specifically by name
        GameObject bestScoreTextObj = GameObject.Find("BestScoreText");
        if (bestScoreTextObj != null)
        {
            bestScoreText = bestScoreTextObj.GetComponent<TMP_Text>();
        }
    }

    // Called when player collects a shield
    public void AddShield()
    {
        // Add one shield
        shields++;
        
        // Update shield display
        ShieldManager.Instance.ResetShieldCount();
        // Show correct number of shields in UI
        for (int i = 0; i < shields; i++)
        {
            ShieldManager.Instance.AddShield();
        }
    }

    public int GetCurrentShieldCount()
    {
        return shields;
    }

    public void SetBettingAllShields(bool bettingAll)
    {
        isBettingAllShields = bettingAll;
        if (bettingAll)
        {
            savedShields = shields; // Store current shields in case we need to restore them
        }
    }

    // Called when player answers a question
    public void ProcessQuestionResult(bool isCorrect)
    {
        if (SceneManager.GetActiveScene().name == "Level2" && isBettingAllShields)
        {
            // Level 2 with all shields bet
            if (isCorrect)
            {
                score += savedShields; // Add points equal to all shields that were bet
            }
            else
            {
                score -= savedShields; // Subtract points equal to all shields that were bet
            }
            // Shields are already reset when betting
        }
        else if (SceneManager.GetActiveScene().name == "Level2")
        {
            // Level 2 without betting (shields were saved)
            if (isCorrect)
            {
                score += 1; // Always just 1 point when shields are saved
            }
            else
            {
                score -= 1; // Always just -1 point when shields are saved
            }
            // Don't reset shields since they were saved
        }
        else
        {
            // Original Level 1 behavior
            if (isCorrect)
            {
                if (shields == 0)
                {
                    score += 1; // Add 1 point if no shields
                }
                else
                {
                    score += shields; // Add points equal to shields collected
                }
                
                correctAnswersInARow++;
                
                if (correctAnswersInARow >= 3)
                {
                    correctAnswersInARow = 0;
                    TryGiveHealthBonus();
                }
            }
            else
            {
                if (shields == 0)
                {
                    score -= 1; // Subtract 1 point if no shields
                }
                else
                {
                    score -= shields; // Subtract points equal to shields collected
                }
                correctAnswersInARow = 0;
            }
            
            // Reset shields after question (only in Level 1)
            shields = 0;
        }
        
        // Reset betting state
        isBettingAllShields = false;
        savedShields = 0;
        
        // Update UI
        ShieldManager.Instance.ResetShieldCount();
        for (int i = 0; i < shields; i++)
        {
            ShieldManager.Instance.AddShield();
        }
        
        // Check if current score is higher than best score
        StartCoroutine(UpdateBestScoreCoroutine());
        
        // Update UI
        UpdateScoreDisplay();
    }

    // Try to give health bonus after 3 correct answers
    private void TryGiveHealthBonus()
    {
        // Only give health if not at full health
        if (heartManager != null && heartManager.GetCurrentHealth() < 3)
        {
            heartManager.RestoreHealth(1);
        }
    }

    // Update the score text in UI
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        
        if (bestScoreText != null)
        {
            bestScoreText.text = "Best: " + bestScore;
        }
    }
    
    // Set the best score (called from ProgressSynchronizer)
    public void SetBestScore(int newBestScore)
    {
        if (newBestScore > bestScore)
        {
            bestScore = newBestScore;
            UpdateScoreDisplay();
            
            // Save to PlayerPrefs but dont trigger a server update since we just got this from the server...
            string currentLevel = SceneManager.GetActiveScene().name;
            string scoreKey = GetScoreKey(currentLevel);
            PlayerPrefs.SetInt(scoreKey, bestScore);
            PlayerPrefs.Save();
        }
    }
    
    // Gets the score key for the specified level
    private string GetScoreKey(string level)
    {
        string userId = PlayerPrefs.GetString("UserID", "");
        return $"{userId}_{level}_BestScore";
    }
    
    // Load the best score for the current level
    private void LoadBestScore()
    {
        string currentLevel = SceneManager.GetActiveScene().name;
        string scoreKey = GetScoreKey(currentLevel);
        bestScore = PlayerPrefs.GetInt(scoreKey, 0);
    }
    
    // Save the best score for the current level
    private IEnumerator SaveBestScoreCoroutine()
    {
        string currentLevel = SceneManager.GetActiveScene().name;
        string scoreKey = GetScoreKey(currentLevel);
        PlayerPrefs.SetInt(scoreKey, bestScore);
        PlayerPrefs.Save();
        
        // Also update best score on the server if online
        yield return StartCoroutine(UpdateBestScoreOnServerCoroutine());
    }
    
    // Update best score on the server (if online)
    private IEnumerator UpdateBestScoreOnServerCoroutine()
    {
        // Get the current user ID
        string userId = PlayerPrefs.GetString("UserID", "");
        
        if (string.IsNullOrEmpty(userId))
            yield break;
            
            // Get the current scene name
            string currentLevel = SceneManager.GetActiveScene().name;
            
            // Create a JSON object for best scores
            string bestScoresJson = $"{{\"{currentLevel}\": {bestScore}}}";
        
        bool syncComplete = false;
        string errorMessage = "";
            
            // Start a coroutine to send the data
        yield return StartCoroutine(NetworkManager.Instance.UpdateGameProgressCoroutine(userId, bestScoresJson, 
            (success, message) => {
                syncComplete = true;
                if (!success)
                    errorMessage = message;
            }));
            
        // Wait for sync to complete
        while (!syncComplete)
            yield return null;
            
        if (!string.IsNullOrEmpty(errorMessage))
            Debug.LogError($"Failed to sync score: {errorMessage}");
    }

    // Update the best score if the current score is higher
    public IEnumerator UpdateBestScoreCoroutine()
    {
        if (score > bestScore)
        {
            bestScore = score;
            yield return StartCoroutine(SaveBestScoreCoroutine());
        }
    }

    // Reset score when starting new game
    public void ResetScore()
    {
        score = 0;
        shields = 0;
        correctAnswersInARow = 0;
        UpdateScoreDisplay();
    }

    public void ResetShields()
    {
        shields = 0;
        ShieldManager.Instance.ResetShieldCount();
    }
} 