using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class NetworkManager : MonoBehaviour
{
    // Singleton instance of NetworkManager
    public static NetworkManager Instance { get; private set; }

    // Base URL 
    private string serverUrl = "http://localhost:3000";
    
    // Server connection status
    private bool _isServerReachable = false;
    public bool IsServerReachable => _isServerReachable;

    private void Awake()
    {
        // Ensure only one instance exists
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

    private void Start()
    {
        // check connection to the backend on startup
        StartCoroutine(TestServerConnection());
    }

    // Serializable request/response structures

    [Serializable] private class RegistrationRequest
    {
        public string email, password, securityQuestion, securityAnswer;
    }

    [Serializable] private class LoginRequest
    {
        public string email, password, deviceIdentifier;
    }

    [Serializable] private class VerifyDeviceRequest
    {
        public string userEmail, deviceIdentifier, securityAnswer;
    }
    
    [Serializable] private class GameProgressRequest
    {
        public string userEmail;
        public string bestScores;
    }

    [Serializable] private class ErrorResponse
    {
        public string error;
    }

    [Serializable] private class LoginResponse
    {
        public string message;
        public bool requiresVerification;
        public int userId;
    }

    [Serializable] private class SecurityQuestionResponse
    {
        public string question, message;
    }

    [Serializable] private class VerificationResponse
    {
        public bool success;
        public string message;
    }
    
    [Serializable] private class GameProgressResponse
    {
        public string message;
        public string bestScores;
    }

    // === Public API Methods

    // Register a new user
    public IEnumerator Register(string email, string password, string securityQuestion, string securityAnswer, Action<bool, string> callback)
    {
        // Check server connection first
        yield return TestServerConnection();
        
        if (_isServerReachable == false)
        {
            callback(false, "Server not reachable. Please check your internet connection.");
            yield break;
        }
        
        var request = new RegistrationRequest
        {
            email = email,
            password = password,
            securityQuestion = securityQuestion,
            securityAnswer = securityAnswer
        };

        yield return SendPostRequest("/api/register", request, (success, responseText) =>
        {
            if (success)
            {
                callback(true, "Registration successful!");
            }
            else
            {
                callback(false, ParseErrorMessage(responseText));
            }
        });
    }

    // Log in a user
    public IEnumerator Login(string email, string password, Action<bool, string, bool> callback)
    {
        // Check server connection first
        yield return TestServerConnection();
        
        // If server is not reachable, try offline login
        if (_isServerReachable == false)
        {
            bool offlineLoginSuccess = TryOfflineLogin(email);
            if (offlineLoginSuccess == true)
            {
                callback(true, "Offline login successful", false);
            }
            else
            {
                callback(false, "Server not reachable and this device is not verified for offline login", false);
            }
            yield break;
        }
        
        var request = new LoginRequest
        {
            email = email,
            password = password,
            deviceIdentifier = SystemInfo.deviceUniqueIdentifier
        };

        yield return SendPostRequest("/api/login", request, (success, responseText) =>
        {
            if (success)
            {
                var response = JsonUtility.FromJson<LoginResponse>(responseText);
                callback(true, response.message, response.requiresVerification);
            }
            else
            {
                callback(false, ParseErrorMessage(responseText), false);
            }
        });
    }
    
    // Try to login offline based on previously verified device
    private bool TryOfflineLogin(string email)
    {
        if (string.IsNullOrEmpty(email) == true)
        {
            return false;
        }
            
        // Check if this device has been verified for this email
        int isVerified = PlayerPrefs.GetInt("deviceVerified_" + email, 0);
        string savedDeviceId = PlayerPrefs.GetString("deviceIdentifier", "");
        
        // If device is verified and it's the same device that was previously used
        return isVerified == 1 && savedDeviceId == SystemInfo.deviceUniqueIdentifier;
    }

    // Fetch a user's security question by email
    public IEnumerator GetSecurityQuestion(string email, Action<bool, string, string> callback)
    {
        // Check server connection first
        yield return TestServerConnection();
        
        if (_isServerReachable == false)
        {
            callback(false, "", "Server not reachable. Please check your internet connection.");
            yield break;
        }
        
        using (UnityWebRequest www = UnityWebRequest.Get($"{serverUrl}/api/security-question/{email}"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<SecurityQuestionResponse>(www.downloadHandler.text);
                callback(true, response.question, response.message);
            }
            else
            {
                callback(false, "", "Failed to get security question.");
            }
        }
    }

    // Verify a device using a security answer
    public IEnumerator VerifyDevice(string email, string question, string answer, Action<bool, string> callback)
    {
        // Check server connection first
        yield return TestServerConnection();
        
        if (_isServerReachable == false)
        {
            callback(false, "Server not reachable. Please check your internet connection.");
            yield break;
        }
        
        var request = new VerifyDeviceRequest
        {
            userEmail = email,  // We're correctly sending email as userEmail
            deviceIdentifier = SystemInfo.deviceUniqueIdentifier,
            securityAnswer = answer.Trim() // Trim whitespace from answer
        };

        yield return SendPostRequest("/api/verify-device", request, (success, responseText) =>
        {
            if (success)
            {
                try {
                    var response = JsonUtility.FromJson<VerificationResponse>(responseText);
                    callback(response.success, response.message);
                }
                catch (System.Exception) {
                    callback(false, "Error processing verification response. Try again.");
                }
            }
            else
            {
                callback(false, $"Verification failed. Server response: {responseText}");
            }
        });
    }
    
    // Fetch user's game progress (best scores only)
    public IEnumerator GetGameProgress(string userEmail, Action<bool, string> callback)
    {
        // Check server connection first
        yield return TestServerConnection();
        
        if (_isServerReachable == false)
        {
            callback(false, "{}");
            yield break;
        }
        
        using (UnityWebRequest www = UnityWebRequest.Get($"{serverUrl}/api/progress/{userEmail}"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                try 
                {
                    // Parse the response
                    GameProgressResponse response = JsonUtility.FromJson<GameProgressResponse>(responseText);
                    callback(true, response.bestScores);
                }
                catch (Exception)
                {
                    // If parsing fails, try to extract manually (as a fallback)
                    string bestScores = ExtractField(responseText, "bestScores");
                    
                    if (!string.IsNullOrEmpty(bestScores))
                    {
                        callback(true, bestScores);
                    }
                    else
                    {
                        callback(false, "{}");
                    }
                }
            }
            else
            {
                callback(false, "{}");
            }
        }
    }
    
    // Helper method to extract field from JSON
    private string ExtractField(string json, string fieldName)
    {
        string searchPattern = $"\"{fieldName}\": ";
        int startIndex = json.IndexOf(searchPattern);
        if (startIndex < 0) return "";
        
        startIndex += searchPattern.Length;
        
        // Check if it's a string
        bool isString = json[startIndex] == '"';
        if (isString) startIndex++;
        
        // Find the end of the value
        int endIndex;
        if (isString)
        {
            endIndex = json.IndexOf('"', startIndex);
        }
        else
        {
            // For non-string values, look for comma or closing brace
            endIndex = json.IndexOfAny(new char[] { ',', '}' }, startIndex);
        }
        
        if (endIndex < 0) return "";
        
        return json.Substring(startIndex, endIndex - startIndex);
    }
    
    // Update best scores
    public IEnumerator UpdateGameProgressCoroutine(string userEmail, string bestScores, Action<bool, string> callback = null)
    {
        // Check server connection first
        yield return TestServerConnection();
        
        // If server is not reachable, store locally for later sync
        if (_isServerReachable == false)
        {
            // Store progress locally
            PlayerPrefs.SetString("PendingProgressUpdate_userEmail", userEmail);
            PlayerPrefs.SetString("PendingProgressUpdate_bestScores", bestScores);
            PlayerPrefs.Save();
            
            if (callback != null)
            {
                callback(true, "Progress saved locally (offline)");
            }
            
            yield break;
        }
        
        var request = new GameProgressRequest
        {
            userEmail = userEmail,
            bestScores = bestScores
        };

        yield return SendPostRequest("/api/progress", request, (success, responseText) =>
        {
            if (success)
            {
                if (callback != null)
                    callback(true, "Progress updated successfully");
            }
            else
            {
                if (callback != null)
                    callback(false, ParseErrorMessage(responseText));
            }
        });
    }
    
    // Overload without callback for simpler calls
    public IEnumerator UpdateGameProgressCoroutine(string userEmail, string bestScores)
    {
        yield return UpdateGameProgressCoroutine(userEmail, bestScores, null);
    }

    public IEnumerator TestServerConnection()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(serverUrl))
        {
            // Set short timeout
            www.timeout = 5;
            yield return www.SendWebRequest();

            _isServerReachable = (www.result == UnityWebRequest.Result.Success);
        }
    }

    // === Helper Methods

    // Sends a POST request with JSON data to the given endpoint
    private IEnumerator SendPostRequest(string endpoint, object payload, Action<bool, string> callback)
    {
        string jsonData = JsonUtility.ToJson(payload);

        using (UnityWebRequest www = new UnityWebRequest(serverUrl + endpoint, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            bool success = www.result == UnityWebRequest.Result.Success;
            callback(success, www.downloadHandler.text);
        }
    }

    // Parses a JSON error response and returns a user-friendly message
    private string ParseErrorMessage(string json)
    {
        try
        {
            var error = JsonUtility.FromJson<ErrorResponse>(json);
            if (string.IsNullOrEmpty(error.error))
            {
                return "An unknown error occurred.";
            }
            if (error.error.Contains("already exists"))
            {
                return "Email already registered.";
            }
            if (error.error.Contains("Invalid email or password"))
            {
                return "Incorrect email or password.";
            }
            if (error.error.Contains("Database error"))
            {
                return "Server error. Try again later.";
            }
            return error.error;
        }
        catch
        {
            return "Failed to parse error response.";
        }
    }
}
