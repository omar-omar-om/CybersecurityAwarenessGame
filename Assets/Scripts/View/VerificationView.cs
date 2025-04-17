using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class VerificationView : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Elements")]
    public TMP_Dropdown securityQuestionDropdown;
    public TMP_InputField answerInput;
    public Button verifyButton;
    public Button backToLoginButton;
    public TMP_Text errorMessageText;

    // Events
    public delegate void VerifyButtonClicked(string question, string answer);
    public event VerifyButtonClicked OnVerifyButtonClicked;
    public event System.Action OnBackToLoginButtonClicked;

    void Start()
    {
        // Hide error message initially
        errorMessageText.gameObject.SetActive(false);

        // Add button listeners
        verifyButton.onClick.AddListener(HandleVerifyButtonClick);
        backToLoginButton.onClick.AddListener(HandleBackToLoginButtonClick);
    }

    private void HandleVerifyButtonClick()
    {
        if (string.IsNullOrEmpty(answerInput.text))
        {
            ShowError("Please enter your answer");
            return;
        }

        string selectedQuestion = securityQuestionDropdown.options[securityQuestionDropdown.value].text;
        OnVerifyButtonClicked?.Invoke(selectedQuestion, answerInput.text);
    }

    private void HandleBackToLoginButtonClick()
    {
        OnBackToLoginButtonClicked?.Invoke();
    }

    public void SetQuestion(string question)
    {
        // Clear existing options
        securityQuestionDropdown.ClearOptions();
        
        // Add the question as the only option
        var options = new System.Collections.Generic.List<string>();
        options.Add(question);
        securityQuestionDropdown.AddOptions(options);
        
        // Set it as selected
        securityQuestionDropdown.value = 0;
    }

    public void ShowError(string message)
    {
        errorMessageText.text = message;
        errorMessageText.gameObject.SetActive(true);
    }

    public void HideError()
    {
        errorMessageText.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (errorMessageText.gameObject.activeSelf)
        {
            HideError();
        }
    }
} 