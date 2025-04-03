using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestionModel : MonoBehaviour
{
    // Class to hold data for each question
    [System.Serializable]
    public class QuestionData
    {
        public string questionText;        // The actual question
        public string[] answers;           // List of possible answers
        public int correctAnswerIndex;     // Which answer is correct (0 = first answer, 1 = second answer, etc.)
    }

    // List of all questions in the game
    public List<QuestionData> questions = new List<QuestionData>();
    
    // Questions that haven't been asked yet
    private List<QuestionData> remainingQuestions = new List<QuestionData>();

    // Events to tell other parts of game when things happen
    public event Action<QuestionData> OnQuestionSelected;    // When new question is picked
    public event Action<bool> OnAnswerProcessed;            // When player answers
    public event Action OnQuestionsExhausted;               // When all questions used

    // Current question being asked
    private QuestionData currentQuestion;
    
    // Is there an active question?
    public bool IsQuestionActive { get; private set; }
    
    // Called when game starts
    void Start()
    {
        ResetQuestions();
    }

    // Reset the list of remaining questions
    public void ResetQuestions()
    {
        // Copy all questions back to remaining questions
        remainingQuestions = new List<QuestionData>(questions);
        IsQuestionActive = false;
    }

    // Pick a new random question
    public void SelectNewQuestion()
    {
        // Check if we have any questions left
        if (remainingQuestions.Count == 0)
        {
            // Tell game we're out of questions
            if (OnQuestionsExhausted != null)
            {
                OnQuestionsExhausted();
            }
            return;
        }

        // Pick a random question from remaining ones
        int randomIndex = UnityEngine.Random.Range(0, remainingQuestions.Count);
        currentQuestion = remainingQuestions[randomIndex];
        
        // Remove it from remaining questions
        remainingQuestions.RemoveAt(randomIndex);
        
        IsQuestionActive = true;
        
        // Tell UI to show the question
        if (OnQuestionSelected != null)
        {
            OnQuestionSelected(currentQuestion);
        }
    }

    // Check if player's answer is correct
    public void ProcessAnswer(int selectedIndex)
    {
        // Ignore if no question is active
        if (!IsQuestionActive) return;
        
        // Check if answer is correct
        bool isCorrect = selectedIndex == currentQuestion.correctAnswerIndex;
        IsQuestionActive = false;
        
        // Tell game if answer was correct or not
        if (OnAnswerProcessed != null)
        {
            OnAnswerProcessed(isCorrect);
        }
    }

    // Get the current question data
    public QuestionData GetCurrentQuestion()
    {
        return currentQuestion;
    }
} 