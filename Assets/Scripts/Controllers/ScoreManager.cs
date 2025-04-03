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
        if (SceneManager.GetActiveScene().name == "Level2")
        {
            ProcessLevel2QuestionResult(isCorrect);
        }
        else
        {
            ProcessLevel1QuestionResult(isCorrect);
        }
        
        // Reset betting state and update UI
        ResetBettingState();
        UpdateShieldDisplay();
        UpdateScoreDisplay();
    }

    private void ProcessLevel2QuestionResult(bool isCorrect)
    {
        if (isBettingAllShields)
        {
            // Player bet all their shields
            if (isCorrect)
            {
                score += savedShields; // Win points equal to shields bet
            }
            else
            {
                score -= savedShields; // Lose points equal to shields bet
            }
        }
        else
        {
            // Player saved their shields
            if (isCorrect)
            {
                score += 1; // Win 1 point
            }
            else
            {
                score -= 1; // Lose 1 point
            }
        }
    }

    private void ProcessLevel1QuestionResult(bool isCorrect)
    {
        if (isCorrect)
        {
            // Player got the question right
            if (shields == 0)
            {
                score += 1; // Win 1 point if no shields
            }
            else
            {
                score += shields; // Win points equal to shields collected
            }
            
            correctAnswersInARow++;
            
            // Check for health bonus
            if (correctAnswersInARow >= 3)
            {
                correctAnswersInARow = 0;
                TryGiveHealthBonus();
            }
        }
        else
        {
            // Player got the question wrong
            if (shields == 0)
            {
                score -= 1; // Lose 1 point if no shields
            }
            else
            {
                score -= shields; // Lose points equal to shields collected
            }
            correctAnswersInARow = 0;
        }
        
        // Reset shields after question in Level 1
        shields = 0;
    }

    private void ResetBettingState()
    {
        isBettingAllShields = false;
        savedShields = 0;
    }

    private void UpdateShieldDisplay()
    {
        ShieldManager.Instance.ResetShieldCount();
        for (int i = 0; i < shields; i++)
        {
            ShieldManager.Instance.AddShield();
        }
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