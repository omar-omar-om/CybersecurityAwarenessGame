using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class NetworkManager : MonoBehaviour
{
    // Singleton instance
    public static NetworkManager Instance { get; private set; }

    // Server base URL
    private string serverUrl = "http://localhost:3000";

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    // Register user
    public IEnumerator Register(string username, string email, string password, string securityQuestion, string securityAnswer, Action<bool, string> callback)
    {
        // Create a dictionary with the registration data
        var registrationData = new Dictionary<string, string>
        {
            { "username", username },
            { "email", email },
            { "password", password },
            { "securityQuestion", securityQuestion },
            { "securityAnswer", securityAnswer }
        };

        // Convert dictionary to JSON string
        string jsonData = JsonUtility.ToJson(registrationData);

        // Send request to server
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(serverUrl + "/api/register", ""))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                callback(true, "Registration successful!");
            }
            else
            {
                // Handle error response
                string errorMessage = "Registration failed: ";
                try
                {
                    // Try to get error message from JSON response
                    ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(www.downloadHandler.text);
                    if (!string.IsNullOrEmpty(error.error))
                    {
                        errorMessage += error.error;
                    }
                    else
                    {
                        errorMessage += www.error;
                    }
                }
                catch
                {
                    errorMessage += www.error;
                }
                callback(false, errorMessage);
            }
        }
    }

    // Login user
    public IEnumerator Login(string email, string password, Action<bool, string, bool> callback)
    {
        // Create login data
        var loginData = new Dictionary<string, string>
        {
            { "email", email },
            { "password", password },
            { "deviceIdentifier", SystemInfo.deviceUniqueIdentifier }
        };

        // Convert to JSON
        string jsonData = JsonUtility.ToJson(loginData);

        // Send request to server
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(serverUrl + "/api/login", ""))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Parse response
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
                
                // If login successful and doesn't require verification, set login status
                if (!response.requiresVerification)
                {
                    PlayerPrefs.SetString("lastLoginEmail", email);
                    PlayerPrefs.SetInt("isLoggedIn_" + email, 1);
                    PlayerPrefs.Save();
                }
                
                callback(true, response.message, response.requiresVerification);
            }
            else
            {
                callback(false, "Login failed: " + www.error, false);
            }
        }
    }

    public IEnumerator GetSecurityQuestion(string email, Action<bool, string, string> callback)
    {
        string url = serverUrl + "/get-security-question";
        WWWForm form = new WWWForm();
        form.AddField("email", email);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<SecurityQuestionResponse>(www.downloadHandler.text);
                callback(true, response.question, response.message);
            }
            else
            {
                callback(false, "", "Failed to get security question: " + www.error);
            }
        }
    }

    public IEnumerator VerifyDevice(string email, string question, string answer, Action<bool, string> callback)
    {
        string url = serverUrl + "/verify-device";
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("question", question);
        form.AddField("answer", answer);
        form.AddField("deviceId", SystemInfo.deviceUniqueIdentifier);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<VerificationResponse>(www.downloadHandler.text);
                callback(response.success, response.message);
            }
            else
            {
                callback(false, "Verification failed: " + www.error);
            }
        }
    }

    // Class for error response
    [Serializable]
    private class ErrorResponse
    {
        public string error;
    }

    // Class for login response
    [Serializable]
    private class LoginResponse
    {
        public string message;
        public bool requiresVerification;
        public int userId;
    }

    [System.Serializable]
    private class SecurityQuestionResponse
    {
        public string question;
        public string message;
    }

    [System.Serializable]
    private class VerificationResponse
    {
        public bool success;
        public string message;
    }
} 