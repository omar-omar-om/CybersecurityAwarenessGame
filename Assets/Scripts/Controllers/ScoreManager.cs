using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    // This makes our ScoreManager accessible from other scripts
    public static ScoreManager Instance;
    
    // Reference to the score text in UI
    public TMP_Text scoreText;
    
    // Variables to track game state
    private int score = 0;
    private int shields = 0;
    private int correctAnswersInARow = 0;
    private HeartManager heartManager;
    private bool isBettingAllShields = false;
    private int savedShields = 0;

    // Called when the script starts
    private void Awake()
    {
        // Proper singleton setup that persists between scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        FindScoreText();
        // Show initial score of 0
        UpdateScoreDisplay();
    }

    private void OnLevelWasLoaded(int level)
    {
        // Reset score when loading a new level
        ResetScore();
        
        // Update references when new level loads
        heartManager = FindObjectOfType<HeartManager>();
        FindScoreText();
        UpdateScoreDisplay();
    }

    private void FindScoreText()
    {
        // Find the score text specifically by name
        GameObject scoreTextObj = GameObject.Find("ScoreText");
        if (scoreTextObj != null)
        {
            scoreText = scoreTextObj.GetComponent<TMP_Text>();
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