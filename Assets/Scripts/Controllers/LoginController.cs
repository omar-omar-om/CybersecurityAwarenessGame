using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    public LoginView loginView;
    
    private string currentEmail;
    
    void Start()
    {
        // Subscribe to view events
        loginView.OnLoginButtonClicked += HandleLoginRequest;
        loginView.OnRegisterButtonClicked += HandleRegisterButton;
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
        // Save email for potential verification later
        this.currentEmail = email;
        
        // Check if we have offline data for this email
        bool isVerified = PlayerPrefs.GetInt("deviceVerified_" + email, 0) == 1;
        
        // Try offline login if we have connectivity issues
        if (!Application.internetReachable && isVerified)
        {
            HandleOfflineLogin(email);
            return;
        }
        
        // Online login - proceed as normal
        StartCoroutine(NetworkManager.Instance.Login(
            email, 
            password, 
            OnLoginResponse));
    }
    
    private void HandleOfflineLogin(string email)
    {
        // Set logged in status
        PlayerPrefs.SetInt("isLoggedIn_" + email, 1);
        PlayerPrefs.Save();
        
        // Show offline login message
        loginView.ShowSuccess("Offline login successful");
        
        // Wait and redirect to main menu
        StartCoroutine(DelayedRedirect());
    }
    
    private void OnLoginResponse(bool success, string message, bool requiresVerification)
    {
        if (success)
        {
            if (requiresVerification)
            {
                // Save email for verification
                PlayerPrefs.SetString("lastLoginEmail", currentEmail);
                PlayerPrefs.Save();
                
                // Redirect to verification scene
                SceneManager.LoadScene("VerificationScene");
            }
            else
            {
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
        
        // Go to login scene
        SceneManager.LoadScene("Login");
    }
    
    private void HandleRegisterButton()
    {
        // Go to register scene
        SceneManager.LoadScene("RegisterScene");
    }
} 