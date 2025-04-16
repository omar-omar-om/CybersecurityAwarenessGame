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
            DontDestroyOnLoad(gameObject); // Make this persist between scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    // Register user
    public IEnumerator Register(string username, string email, string password, string securityQuestion, string securityAnswer, Action<bool, string> callback)
    {
        // Convert to JSON
        string jsonData = JsonUtility.ToJson(new RegisterData(username, email, password, securityQuestion, securityAnswer));

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

    // Class for registration data
    [Serializable]
    private class RegisterData
    {
        public string username;
        public string email;
        public string password;
        public string securityQuestion;
        public string securityAnswer;

        public RegisterData(string username, string email, string password, string securityQuestion, string securityAnswer)
        {
            this.username = username;
            this.email = email;
            this.password = password;
            this.securityQuestion = securityQuestion;
            this.securityAnswer = securityAnswer;
        }
    }

    // Class for error response
    [Serializable]
    private class ErrorResponse
    {
        public string error;
    }
} 