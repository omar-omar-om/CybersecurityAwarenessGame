using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class QuestionController : MonoBehaviour
{
    public QuestionModel model;
    public QuestionView view;
    
    public float timeBetweenQuestions = 20f;
    private float questionStartTime;
    private bool isWaitingForAnswer = false;
    private Coroutine feedbackCoroutine;
    private bool isLevel2 = false;
    private bool isShowingShieldChoice = false;
    private int currentShieldCount = 0;
    private int currentQuestionIndex = 0;
    private List<QuestionModel.QuestionData> questions = new List<QuestionModel.QuestionData>();

    private void Start()
    {
        // Check if we are in Level 2
        isLevel2 = SceneManager.GetActiveScene().name == "Level2";
        
        // Subscribe to events
        view.OnAnswerSelected += HandleAnswerSelected;
        model.OnQuestionSelected += HandleQuestionSelected;
        model.OnAnswerProcessed += HandleAnswerProcessed;
        model.OnQuestionsExhausted += HandleQuestionsExhausted;

        // Start the game loop
        StartCoroutine(WaitBeforeNextQuestion());
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        view.OnAnswerSelected -= HandleAnswerSelected;
        model.OnQuestionSelected -= HandleQuestionSelected;
        model.OnAnswerProcessed -= HandleAnswerProcessed;
        model.OnQuestionsExhausted -= HandleQuestionsExhausted;
    }

    private IEnumerator WaitBeforeNextQuestion()
    {
        yield return new WaitForSeconds(timeBetweenQuestions);
        StartQuestion();
    }

    private void StartQuestion()
    {
        if (isLevel2)
        {
            // Get current shield count
            currentShieldCount = ScoreManager.Instance.GetCurrentShieldCount();
            if (currentShieldCount > 0)
            {
                // For shield choice, only stop shields
                ShieldSpawner.canSpawn = false;
                DestroyAllShields();
                
                questionStartTime = Time.time;
                isWaitingForAnswer = true;
                
                // Show shield choice first
                ShowShieldChoice();
            }
            else
            {
                // No shields to bet, proceed with normal question
                StartNormalQuestion();
            }
        }
        else
        {
            // Level 1 behavior remains the same
            StartNormalQuestion();
        }
    }

    private void StartNormalQuestion()
    {
        // Stop both obstacles and shields for actual question
        ObstacleSpawner.canSpawn = false;
        ShieldSpawner.canSpawn = false;
        DestroyAllObstacles();
        DestroyAllShields();
        PlayerJump.canJump = false;
        
        questionStartTime = Time.time;
        isWaitingForAnswer = true;
        
        model.SelectNewQuestion();
        StartCoroutine(WaitForAnswer());
    }

    private IEnumerator WaitForAnswer()
    {
        float timer = 0f;
        bool obstaclesResumed = false;

        while (isWaitingForAnswer)
        {
            timer += Time.deltaTime;
            
            // Start obstacles again after 5 seconds
            if (timer >= 5f && !obstaclesResumed)
            {
                Debug.Log("Starting obstacles again");
                ObstacleSpawner.canSpawn = true;
                obstaclesResumed = true;
            }
            
            yield return null;
        }
    }

    private void HandleQuestionSelected(QuestionModel.QuestionData question)
    {
        view.DisplayQuestion(question);
    }

    private void HandleAnswerSelected(int selectedIndex)
    {
        if (isWaitingForAnswer == false) return;
        
        if (isShowingShieldChoice)
        {
            // Handle shield choice
            HandleShieldChoice(selectedIndex == 0); // 0 = Yes, 1 = No
            return;
        }
        
        isWaitingForAnswer = false;

        // Start obstacles right away if answered quickly
        if (Time.time - questionStartTime < 5f)
        {
            ObstacleSpawner.canSpawn = true;
        }

        model.ProcessAnswer(selectedIndex);
    }

    private void HandleAnswerProcessed(bool isCorrect)
    {
        // Skip color feedback for shield choices since there's no right/wrong answer
        if (!isShowingShieldChoice)
        {
            QuestionModel.QuestionData currentQuestion = model.GetCurrentQuestion();
            view.ShowAnswerFeedback(currentQuestion.correctAnswerIndex, isCorrect, currentQuestion.correctAnswerIndex);
        }
        
        // Process the question result in ScoreManager
        ScoreManager.Instance.ProcessQuestionResult(isCorrect);
        
        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
        }
        feedbackCoroutine = StartCoroutine(ShowFeedbackAndEndQuestion());
    }

    private void HandleQuestionsExhausted()
    {
        // Sync one final time before ending
        if (ScoreManager.Instance != null)
        {
            StartCoroutine(FinalSyncAndEndLevel());
        }
        else
        {
            EndLevel();
        }
    }

    private IEnumerator ShowFeedbackAndEndQuestion()
    {
        yield return new WaitForSeconds(1.5f);
        EndQuestion();
    }

    private void EndQuestion()
    {
        view.HideQuestion();
        PlayerJump.canJump = true;

        // Start spawning obstacles and shields again
        ObstacleSpawner.canSpawn = true;
        ShieldSpawner.canSpawn = true;
        
        // Start coroutine to sync score and wait before next question
        StartCoroutine(SyncAndWaitForNextQuestion());
    }

    private IEnumerator SyncAndWaitForNextQuestion()
    {
        // Sync the score first
        if (ScoreManager.Instance != null)
        {
            yield return StartCoroutine(ScoreManager.Instance.UpdateBestScoreCoroutine());
        }
        
        // Then wait before next question
        yield return new WaitForSeconds(2f);
        
        // Start next question
        StartCoroutine(WaitBeforeNextQuestion());
    }

    private void DestroyAllObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }
    }

    private void DestroyAllShields()
    {
        GameObject[] shields = GameObject.FindGameObjectsWithTag("Shield");
        foreach (GameObject shield in shields)
        {
            Destroy(shield);
        }
    }

    private void ShowShieldChoice()
    {
        if (isShowingShieldChoice == false)
        {
            isShowingShieldChoice = true;
            // Use the same question panel but with shield choice text
            QuestionModel.QuestionData shieldChoice = new QuestionModel.QuestionData
            {
                questionText = "Do you want to enter with all " + currentShieldCount + " shields you have so far?",
                answers = new string[] { "Yes", "No" },
                correctAnswerIndex = 0  
            };
            view.DisplayQuestion(shieldChoice);
            StartCoroutine(ShieldChoiceTimer());
        }
    }

    private IEnumerator ShieldChoiceTimer()
    {
        float timer = 0f;
        while (isShowingShieldChoice && timer < 5f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        // If no choice made within 5 seconds, default to "No"
        if (isShowingShieldChoice)
        {
            HandleShieldChoice(false);
        }
    }

    private void HandleShieldChoice(bool useAllShields)
    {
        isShowingShieldChoice = false;
        if (useAllShields)
        {
            // Player chose to bet all shields
            ScoreManager.Instance.SetBettingAllShields(true);
            // Reset shields since they're being bet
            ScoreManager.Instance.ResetShields();
        }
        else
        {
            // Player chose to save shields
            ScoreManager.Instance.SetBettingAllShields(false);
        }
        
        // Now show the actual question
        StartNormalQuestion();
    }

    private void EndLevel()
    {
        HeartManager heartManager = FindObjectOfType<HeartManager>();
        if (heartManager != null)
        {
            int currentHealth = heartManager.GetCurrentHealth();
            
            if (currentHealth > 0)
            {
                GameWonController.SetLastPlayedLevel(SceneManager.GetActiveScene().name);
                SceneManager.LoadScene("GameWon");
            }
            else
            {
                GameOverController.SetLastPlayedLevel(SceneManager.GetActiveScene().name);
                SceneManager.LoadScene("GameOver");
            }
        }
        else
        {
            GameOverController.SetLastPlayedLevel(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene("GameOver");
        }
    }

    private IEnumerator FinalSyncAndEndLevel()
    {
        yield return StartCoroutine(ScoreManager.Instance.UpdateBestScoreCoroutine());
        EndLevel();
    }
} 