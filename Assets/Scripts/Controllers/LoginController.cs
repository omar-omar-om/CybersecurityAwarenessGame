using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    public LoginView loginView;
    
    void Start()
    {
        // Check if user is already logged in
        if (IsUserLoggedIn())
        {
            // Try auto-login
            string email = PlayerPrefs.GetString("lastLoginEmail", "");
            if (!string.IsNullOrEmpty(email))
            {
                loginView.SetEmail(email);
                StartCoroutine(AttemptAutoLogin(email));
            }
        }
        
        // Subscribe to view events
        loginView.OnLoginButtonClicked += HandleLoginRequest;
        loginView.OnRegisterButtonClicked += HandleRegisterButton;
    }
    
    private bool IsUserLoggedIn()
    {
        return PlayerPrefs.GetInt("isLoggedIn", 0) == 1;
    }
    
    private IEnumerator AttemptAutoLogin(string email)
    {
        // Show message
        loginView.ShowMessage("Attempting auto-login...");
        
        // Try to login without password (offline mode)
        yield return StartCoroutine(NetworkManager.Instance.Login(
            email, 
            "", // Empty password for offline login attempt
            OnAutoLoginResponse));
    }
    
    private void OnAutoLoginResponse(bool success, string message, bool requiresVerification)
    {
        if (success && !requiresVerification)
        {
            // Auto-login successful
            loginView.ShowSuccess();
            StartCoroutine(DelayedRedirect());
        }
        else
        {
            // Auto-login failed, but don't show an error - just let the user enter credentials
            loginView.ClearStatus();
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from view events
        if (loginView != null)
        {
            loginView.OnLoginButtonClicked -= HandleLoginRequest;
            loginView.OnRegisterButtonClicked -= HandleRegisterButton;
        }
    }
    
    private void HandleLoginRequest(string email, string password)
    {
        // Call NetworkManager to handle login
        StartCoroutine(NetworkManager.Instance.Login(
            email, 
            password, 
            OnLoginResponse));
    }
    
    private void OnLoginResponse(bool success, string message, bool requiresVerification)
    {
        if (success)
        {
            if (requiresVerification)
            {
                // Save the email for the verification scene to use
                PlayerPrefs.SetString("lastLoginEmail", loginView.GetEmail());
                PlayerPrefs.Save();
                
                // Load the verification scene
                SceneManager.LoadScene("VerificationScene");
            }
            else
            {
                // Save email for later use
                PlayerPrefs.SetString("lastLoginEmail", loginView.GetEmail());
                // Mark user as logged in
                PlayerPrefs.SetInt("isLoggedIn", 1);
                PlayerPrefs.Save();
                
                // Login successful, show success message
                loginView.ShowSuccess();
                
                // Wait for 2 seconds then go to main menu
                StartCoroutine(DelayedRedirect());
            }
        }
        else
        {
            // Show error message
            loginView.ShowError(message);
        }
    }
    
    private IEnumerator DelayedRedirect()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);
        
        // Go to main menu scene
        SceneManager.LoadScene("MainMenu");
    }
    
    private void HandleRegisterButton()
    {
        // Go to register scene
        SceneManager.LoadScene("RegisterScene");
    }
} 