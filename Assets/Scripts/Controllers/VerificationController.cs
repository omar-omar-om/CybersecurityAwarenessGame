using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VerificationController : MonoBehaviour
{
    public VerificationView verificationView;
    private string currentEmail;

    void Start()
    {
        // Subscribe to view events
        verificationView.OnVerifyButtonClicked += HandleVerifyRequest;
        verificationView.OnBackToLoginButtonClicked += HandleBackToLogin;

        // Get the security question from NetworkManager
        StartCoroutine(GetSecurityQuestion());
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

    private IEnumerator GetSecurityQuestion()
    {
        // Get the email from PlayerPrefs (set during login)
        currentEmail = PlayerPrefs.GetString("lastLoginEmail", "");
        Debug.Log("Getting security question for email: " + currentEmail);
        
        if (string.IsNullOrEmpty(currentEmail))
        {
            verificationView.ShowError("No email found. Please login again.");
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("Login");
            yield break;
        }

        // Call NetworkManager to get the security question
        yield return StartCoroutine(NetworkManager.Instance.GetSecurityQuestion(
            currentEmail,
            OnQuestionReceived));
    }

    private void OnQuestionReceived(bool success, string question, string message)
    {
        if (success)
        {
            Debug.Log("Received security question: " + question);
            verificationView.SetQuestion(question);
        }
        else
        {
            Debug.LogError("Failed to get security question: " + message);
            verificationView.ShowError(message);
        }
    }

    private void HandleVerifyRequest(string question, string answer)
    {
        Debug.Log("Attempting to verify with answer: " + answer);
        StartCoroutine(VerifyAnswer(question, answer));
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
        Debug.Log("Verification response: " + (success ? "Success" : "Failed") + " - " + message);
        
        if (success)
        {
            // Save verification status
            PlayerPrefs.SetString("deviceIdentifier", SystemInfo.deviceUniqueIdentifier);
            PlayerPrefs.SetInt("deviceVerified_" + currentEmail, 1);
            // Mark user as logged in
            PlayerPrefs.SetInt("isLoggedIn", 1);
            PlayerPrefs.Save();

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