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
    public delegate void VerifyButtonClicked(string answer);
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

        OnVerifyButtonClicked?.Invoke(answerInput.text);
    }

    private void HandleBackToLoginButtonClick()
    {
        OnBackToLoginButtonClicked?.Invoke();
    }

    public void SetSecurityQuestion(string question)
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
    
    public string GetSecurityQuestion()
    {
        if (securityQuestionDropdown.options.Count > 0)
        {
            return securityQuestionDropdown.options[securityQuestionDropdown.value].text;
        }
        return "";
    }

    public void ShowError(string message)
    {
        errorMessageText.color = Color.red; // Error messages are red
        errorMessageText.text = message;
        errorMessageText.gameObject.SetActive(true);
    }
    
    public void ShowMessage(string message)
    {
        errorMessageText.color = Color.white; // Info messages are white
        errorMessageText.text = message;
        errorMessageText.gameObject.SetActive(true);
    }
    
    public void ShowSuccess()
    {
        // Show success message with green color
        errorMessageText.color = Color.green;
        errorMessageText.text = "Verification successful!";
        errorMessageText.gameObject.SetActive(true);
        
        // Disable verify button to prevent multiple submissions
        verifyButton.interactable = false;
        backToLoginButton.interactable = false;
    }

    public void HideError()
    {
        errorMessageText.gameObject.SetActive(false);
    }
    
    public void ClearStatus()
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