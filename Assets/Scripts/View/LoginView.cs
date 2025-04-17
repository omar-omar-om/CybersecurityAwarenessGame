using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LoginView : MonoBehaviour, IPointerClickHandler
{
    // UI Elements
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button registerButton;
    public TMP_Text errorMessageText;

    // Events for controller to subscribe to
    public event Action<string, string> OnLoginButtonClicked;
    public event Action OnRegisterButtonClicked;

    private void Start()
    {
        // Hide error message initially
        errorMessageText.gameObject.SetActive(false);
        
        // Add listeners to buttons
        loginButton.onClick.AddListener(HandleLoginButtonClick);
        registerButton.onClick.AddListener(HandleRegisterButtonClick);
    }

    private void HandleLoginButtonClick()
    {
        // Validate inputs before proceeding
        if (ValidateInputs())
        {
            // Invoke the event for the controller to handle
            OnLoginButtonClicked?.Invoke(
                emailInput.text,
                passwordInput.text
            );
            
            // Make panel semi-transparent to show processing
            MakePanelSemiTransparent();
        }
    }

    private void HandleRegisterButtonClick()
    {
        OnRegisterButtonClicked?.Invoke();
    }

    private bool ValidateInputs()
    {
        // Reset error message
        errorMessageText.gameObject.SetActive(false);
        
        // Check for empty fields
        if (string.IsNullOrEmpty(emailInput.text) ||
            string.IsNullOrEmpty(passwordInput.text))
        {
            ShowError("All fields are required");
            return false;
        }
        
        // Validate email format
        if (!IsValidEmail(emailInput.text))
        {
            ShowError("Please enter a valid email address");
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
        Image panelImage = GetComponent<Image>();
        Color panelColor = panelImage.color;
        panelColor.a = 0.8f;
        panelImage.color = panelColor;
    }
    
    // Makes the panel transparent again
    private void MakePanelTransparent()
    {
        Image panelImage = GetComponent<Image>();
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
        errorMessageText.text = "Login successful!";
        errorMessageText.gameObject.SetActive(true);
        
        // Disable login button
        loginButton.interactable = false;
    }

    public void HideError()
    {
        // Make panel transparent again
        MakePanelTransparent();
        
        // Hide error message
        errorMessageText.gameObject.SetActive(false);
    }
} 