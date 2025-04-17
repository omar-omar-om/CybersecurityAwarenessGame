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
            verificationView.SetQuestion(question);
        }
        else
        {
            verificationView.ShowError(message);
        }
    }

    private void HandleVerifyRequest(string question, string answer)
    {
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
        if (success)
        {
            // Save verification status
            PlayerPrefs.SetInt("deviceVerified_" + currentEmail, 1);
            PlayerPrefs.Save();

            // Go to MainMenu
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            verificationView.ShowError(message);
        }
    }

    private void HandleBackToLogin()
    {
        SceneManager.LoadScene("Login");
    }
} 