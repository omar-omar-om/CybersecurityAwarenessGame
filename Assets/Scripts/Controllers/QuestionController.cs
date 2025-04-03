using UnityEngine;
using System.Collections;
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

    private void Start()
    {
        // Check if we're in Level 2
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
        Debug.Log("Level complete! Checking health...");
        
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
            Debug.Log("Oops, couldn't find health manager. Going to game over.");
            GameOverController.SetLastPlayedLevel(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene("GameOver");
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
        isShowingShieldChoice = true;
        
        // Create a simple shield choice question
        QuestionModel.QuestionData shieldChoice = CreateShieldChoiceQuestion();
        
        // Show the question to the player
        view.DisplayQuestion(shieldChoice);
        
        // Start the timer for the shield choice
        StartCoroutine(ShieldChoiceTimer());
    }

    private QuestionModel.QuestionData CreateShieldChoiceQuestion()
    {
        // Create a new question for shield choice
        QuestionModel.QuestionData shieldChoice = new QuestionModel.QuestionData();
        
        // Set the question text
        shieldChoice.questionText = "Do you want to enter with all " + currentShieldCount + " shields you have so far?";
        
        // Set the answer options
        shieldChoice.answers = new string[] { "Yes", "No" };
        
        // Set correct answer index to 0 to show both buttons
        shieldChoice.correctAnswerIndex = 0;
        
        return shieldChoice;
    }

    private void HandleShieldChoice(bool choseYes)
    {
        isShowingShieldChoice = false;
        
        if (choseYes)
        {
            // Player chose to bet all shields
            ScoreManager.Instance.SetBettingAllShields(true);
        }
        
        // Start the actual question
        StartNormalQuestion();
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
} 