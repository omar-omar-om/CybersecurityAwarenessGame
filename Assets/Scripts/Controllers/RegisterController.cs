using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegisterController : MonoBehaviour
{
    public RegisterView registerView;
    
    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to view events
        registerView.OnRegisterButtonClicked += HandleRegisterRequest;
        registerView.OnBackToLoginButtonClicked += HandleBackToLogin;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from view events
        if (registerView != null)
        {
            registerView.OnRegisterButtonClicked -= HandleRegisterRequest;
            registerView.OnBackToLoginButtonClicked -= HandleBackToLogin;
        }
    }
    
    private void HandleRegisterRequest(string email, string password, string securityQuestion, string securityAnswer)
    {
        // Call NetworkManager directly with a single coroutine
        StartCoroutine(NetworkManager.Instance.Register(
            email, 
            password, 
            securityQuestion, 
            securityAnswer, 
            OnRegisterResponse));
    }
    
    private void OnRegisterResponse(bool success, string message)
    {
        if (success)
        {
            // Show success message
            registerView.ShowSuccess();
            
            // Wait for 2 seconds then go to login screen
            StartCoroutine(DelayedRedirect());
        }
        else
        {
            // Show error message
            registerView.ShowError(message);
        }
    }
    
    private IEnumerator DelayedRedirect()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);
        
        // Go to login scene (create this scene next)
        HandleBackToLogin();
    }
    
    private void HandleBackToLogin()
    {
       
        SceneManager.LoadScene("Login");
    }
} 