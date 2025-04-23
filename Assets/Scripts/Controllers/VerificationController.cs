using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VerificationController : MonoBehaviour
{
    public VerificationView verificationView;
    private string currentEmail;

    void Start()
    {
        // Get the email saved from the login screen
        currentEmail = PlayerPrefs.GetString("lastLoginEmail", "");
        
        if (string.IsNullOrEmpty(currentEmail))
        {
            // No email found, go back to login
            SceneManager.LoadScene("Login");
            return;
        }
        
        // Show verification screen and get security question
        StartCoroutine(GetSecurityQuestion(currentEmail));
        
        // Subscribe to view events
        verificationView.OnVerifyButtonClicked += HandleVerifyRequest;
        verificationView.OnBackToLoginButtonClicked += HandleBackToLogin;
    }

    private void OnDestroy()
    {
        // Unsubscribe from view events
        if (verificationView != null)
        {
            verificationView.OnVerifyButtonClicked -= HandleVerifyRequest;
            verificationView.OnBackToLoginButtonClicked -= HandleBackToLogin;
        }
    }

    private IEnumerator GetSecurityQuestion(string email)
    {
        verificationView.ShowMessage("Loading security question...");
        
        yield return StartCoroutine(NetworkManager.Instance.GetSecurityQuestion(
            email,
            OnSecurityQuestionResponse));
    }

    private void OnSecurityQuestionResponse(bool success, string question, string message)
    {
        if (success)
        {
            // Show the security question
            verificationView.SetSecurityQuestion(question);
            verificationView.ClearStatus();
        }
        else
        {
            // Show error and provide option to go back
            verificationView.ShowError("Failed to load security question. " + message);
        }
    }

    private void HandleVerifyRequest(string answer)
    {
        StartCoroutine(VerifyAnswer(verificationView.GetSecurityQuestion(), answer));
    }

    private IEnumerator VerifyAnswer(string question, string answer)
    {
        yield return StartCoroutine(NetworkManager.Instance.VerifyDevice(
            currentEmail,
            question,
            answer,
            OnVerificationResponse));
    }

    private void OnVerificationResponse(bool success, string message)
    {
        if (success)
        {
            // Save verification status
            PlayerPrefs.SetString("deviceIdentifier", SystemInfo.deviceUniqueIdentifier);
            PlayerPrefs.SetInt("deviceVerified_" + currentEmail, 1);
            // Mark user as logged in
            PlayerPrefs.SetInt("isLoggedIn", 1);
            // Store email as UserID for progress tracking
            PlayerPrefs.SetString("UserID", currentEmail);
            PlayerPrefs.Save();

            // Sync scores with server
            SyncUserProgressWithServer(currentEmail);

            // Show success message
            verificationView.ShowSuccess();
            // Wait for a moment before redirecting
            StartCoroutine(DelayedRedirect());
        }
        else
        {
            verificationView.ShowError(message);
        }
    }

    // Sync user progress with server
    private void SyncUserProgressWithServer(string email)
    {
        // Find the ProgressSynchronizer in the scene
        ProgressSynchronizer synchronizer = FindObjectOfType<ProgressSynchronizer>();
        
        // Start the synchronization process
        synchronizer.SyncProgressWithServer(email);
    }

    private IEnumerator DelayedRedirect()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);
        
        // Go to MainMenu
        SceneManager.LoadScene("MainMenu");
    }

    private void HandleBackToLogin()
    {
        SceneManager.LoadScene("Login");
    }
} 