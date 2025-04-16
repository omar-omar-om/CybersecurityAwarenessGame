using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class RegisterView : MonoBehaviour, IPointerClickHandler
{
 
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_Dropdown securityQuestionDropdown;
    public TMP_InputField securityAnswerInput;
   
    public Button registerButton;
    public Button backToLoginButton;
    
    public TMP_Text errorMessageText;
    
    // Events for controller to subscribe to
    public event Action<string, string, string, string, string> OnRegisterButtonClicked;
    public event Action OnBackToLoginButtonClicked;
    
    private void Start()
    {
        // Hide error message initially
        errorMessageText.gameObject.SetActive(false);
        
        // Add security questions to dropdown
        PopulateSecurityQuestions();
        
        // Add listeners to buttons
        registerButton.onClick.AddListener(HandleRegisterButtonClick);
        backToLoginButton.onClick.AddListener(HandleBackToLoginButtonClick);
    }
    
    private void PopulateSecurityQuestions()
    {
        securityQuestionDropdown.ClearOptions();
        
        // Create list for dropdown options
        var options = new List<string>();
        
        // Add placeholder as the first option
        options.Add("Security Question");
        
        // Add security questions one by one
        options.Add("What was your first pet's name?");
        options.Add("What is your mother's maiden name?");
        options.Add("In what city were you born?");
        options.Add("What was the name of your first school?");
        options.Add("What is your favorite movie?");
        
        // Add all options to dropdown
        securityQuestionDropdown.AddOptions(options);
        securityQuestionDropdown.value = 0; // Select the placeholder by default
    }
    
    private void HandleRegisterButtonClick()
    {
        // Validate inputs before proceeding
        if (ValidateInputs())
        {
            
            string securityQuestion = securityQuestionDropdown.options[securityQuestionDropdown.value].text;
            
            // Invoke the event for the controller to handle
            OnRegisterButtonClicked?.Invoke(
                usernameInput.text,
                emailInput.text,
                passwordInput.text,
                securityQuestion,
                securityAnswerInput.text
            );
            
            // Make panel semi-transparent to show processing
            MakePanelSemiTransparent();
        }
    }
    
    private void HandleBackToLoginButtonClick()
    {
        OnBackToLoginButtonClicked?.Invoke();
    }
    
    private bool ValidateInputs()
    {
        // Reset error message
        errorMessageText.gameObject.SetActive(false);
        
        // Check for empty fields
        if (string.IsNullOrEmpty(usernameInput.text) ||
            string.IsNullOrEmpty(emailInput.text) ||
            string.IsNullOrEmpty(passwordInput.text) ||
            string.IsNullOrEmpty(confirmPasswordInput.text) ||
            string.IsNullOrEmpty(securityAnswerInput.text))
        {
            ShowError("All fields are required");
            return false;
        }
        
        // Check if security question is selected (not the placeholder)
        if (securityQuestionDropdown.value == 0)
        {
            ShowError("Please select a security question");
            return false;
        }
        
        // Validate email format
        if (!IsValidEmail(emailInput.text))
        {
            ShowError("Please enter a valid email address");
            return false;
        }
        
        // Check if passwords match
        if (passwordInput.text != confirmPasswordInput.text)
        {
            ShowError("Passwords do not match");
            return false;
        }
        
        // Check password strength
        if (passwordInput.text.Length < 8)
        {
            ShowError("Password must be at least 8 characters long");
            return false;
        }
        
        return true;
    }
    
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    // Makes the panel semi-transparent
    private void MakePanelSemiTransparent()
    {
        // Get the panel's Image component
        Image panelImage = GetComponent<Image>();
        
        // Make panel semi-transparent
        Color panelColor = panelImage.color;
        panelColor.a = 0.8f;
        panelImage.color = panelColor;
    }
    
    // Makes the panel transparent again
    private void MakePanelTransparent()
    {
        // Get the panel's Image component
        Image panelImage = GetComponent<Image>();
        
        // Make panel transparent
        Color panelColor = panelImage.color;
        panelColor.a = 0f;
        panelImage.color = panelColor;
    }
    
    // Implement OnPointerClick for the IPointerClickHandler interface
    public void OnPointerClick(PointerEventData eventData)
    {
        // If panel is visible (showing an error), hide it on click
        Image panelImage = GetComponent<Image>();
        if (panelImage.color.a > 0)
        {
            HideError();
        }
    }
    
    
    public void ShowError(string message)
    {
        // Make panel semi-transparent
        MakePanelSemiTransparent();
        
        // Show error message
        errorMessageText.text = message;
        errorMessageText.gameObject.SetActive(true);
    }
    
    public void ShowSuccess()
    {
        // Make panel semi-transparent
        MakePanelSemiTransparent();
        
        // Show success message
        errorMessageText.color = Color.green;
        errorMessageText.text = "Registration successful!";
        errorMessageText.gameObject.SetActive(true);
        
        // Disable register button
        registerButton.interactable = false;
        
        // For success, don't dismiss on click since we're redirecting
    }
    
    public void HideError()
    {
        // Make panel transparent again
        MakePanelTransparent();
        
        // Hide error message
        errorMessageText.gameObject.SetActive(false);
    }
} 