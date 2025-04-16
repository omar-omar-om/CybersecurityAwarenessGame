using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    public LoginView loginView;
    
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
                // Show security question verification screen
                // We'll implement this later
                loginView.ShowError("Device verification required");
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