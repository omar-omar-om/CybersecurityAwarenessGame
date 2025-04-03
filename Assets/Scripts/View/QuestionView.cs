using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class QuestionView : MonoBehaviour
{
    // UI elements we need to show questions
    public GameObject questionPanel;  // The panel that holds the question
    public TextMeshProUGUI questionText;  // Where we show the question text
    public Transform answerButtonContainer;  // Where we put the answer buttons
    public GameObject answerButtonPrefab;  // The button template we'll copy

    // Event that fires when player picks an answer
    public event Action<int> OnAnswerSelected;

    private void Start()
    {
        // Hide question panel when game starts
        HideQuestion();
    }

    public void DisplayQuestion(QuestionModel.QuestionData question)
    {
        // Show the question panel and set the text
        questionPanel.SetActive(true);
        questionText.text = question.questionText;
        
        // Clean up old buttons and create new ones
        ClearAnswerButtons();
        SpawnAnswerButtons(question);
    }

    public void HideQuestion()
    {
        // Hide the question panel and clean up buttons
        questionPanel.SetActive(false);
        ClearAnswerButtons();
    }

    private void ClearAnswerButtons()
    {
        // Remove all existing answer buttons
        for (int i = 0; i < answerButtonContainer.childCount; i++)
        {
            Destroy(answerButtonContainer.GetChild(i).gameObject);
        }
    }

    private void SpawnAnswerButtons(QuestionModel.QuestionData question)
    {
        // Set up button positions
        float topButtonY = 140f;
        float bottomButtonY = -90f;
        
        // Randomly decide if correct answer goes top or bottom
        int correctPos = UnityEngine.Random.Range(0, 2);
        int wrongPos;
        if (correctPos == 0)
        {
            wrongPos = 1;
        }
        else
        {
            wrongPos = 0;
        }

        // Create a button for each answer
        for (int i = 0; i < question.answers.Length; i++)
        {
            // Make a copy of our button template
            GameObject newButton = Instantiate(answerButtonPrefab, answerButtonContainer);
            RectTransform buttonTransform = newButton.GetComponent<RectTransform>();
            
            // Position button based on whether it's correct or wrong
            float yPos;
            if (i == question.correctAnswerIndex)
            {
                if (correctPos == 0)
                {
                    yPos = topButtonY;
                }
                else
                {
                    yPos = bottomButtonY;
                }
            }
            else
            {
                if (wrongPos == 0)
                {
                    yPos = topButtonY;
                }
                else
                {
                    yPos = bottomButtonY;
                }
            }
            
            buttonTransform.anchoredPosition = new Vector2(0, yPos);

            // Set the button text
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = question.answers[i];
            }

            // Add my custom component to track which answer this is
            AnswerButton answerButton = newButton.AddComponent<AnswerButton>();
            answerButton.answerIndex = i;

          
            int index = i;
            newButton.GetComponent<Button>().onClick.AddListener(() => OnAnswerSelected?.Invoke(index));
        }
    }

    public void ShowAnswerFeedback(int selectedIndex, bool isCorrect, int correctIndex)
    {
        // Go through all buttons and color them based on the answer
        for (int i = 0; i < answerButtonContainer.childCount; i++)
        {
            Transform child = answerButtonContainer.GetChild(i);
            Button button = child.GetComponent<Button>();
            AnswerButton answerButton = child.GetComponent<AnswerButton>();
            
            // Color correct answer green, wrong answer red
            if (answerButton.answerIndex == correctIndex)
            {
                button.GetComponent<Image>().color = Color.green;
            }
            else if (answerButton.answerIndex == selectedIndex && !isCorrect)
            {
                button.GetComponent<Image>().color = Color.red;
            }
        }
    }
} 