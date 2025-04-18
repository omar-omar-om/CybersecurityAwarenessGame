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
        public string userId, deviceIdentifier, securityAnswer;
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

    // === Public API Methods

    // Register a new user
    public IEnumerator Register(string email, string password, string securityQuestion, string securityAnswer, Action<bool, string> callback)
    {
        // Check server connection first
        yield return TestServerConnection();
        
        if (!_isServerReachable)
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
                callback(true, "Registration successful!");
            else
                callback(false, ParseErrorMessage(responseText));
        });
    }

    // Log in a user
    public IEnumerator Login(string email, string password, Action<bool, string, bool> callback)
    {
        // Check server connection first
        yield return TestServerConnection();
        
        // If server is not reachable, try offline login
        if (!_isServerReachable)
        {
            bool offlineLoginSuccess = TryOfflineLogin(email);
            if (offlineLoginSuccess)
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
        if (string.IsNullOrEmpty(email))
            return false;
            
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
        
        if (!_isServerReachable)
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
        
        if (!_isServerReachable)
        {
            Debug.LogError("Server not reachable during verify device");
            callback(false, "Server not reachable. Please check your internet connection.");
            yield break;
        }
        
        // Log request details for debugging
        Debug.Log($"Verifying device with email: '{email}', answer: '{answer}'");
        Debug.Log($"Device ID: {SystemInfo.deviceUniqueIdentifier}");
        
        var request = new VerifyDeviceRequest
        {
            userId = email,  // We're correctly sending email as userId
            deviceIdentifier = SystemInfo.deviceUniqueIdentifier,
            securityAnswer = answer.Trim() // Trim whitespace from answer
        };
        
        Debug.Log($"Sending verification request to: {serverUrl}/api/verify-device");

        yield return SendPostRequest("/api/verify-device", request, (success, responseText) =>
        {
            Debug.Log($"Verify device response - Success: {success}, Response: {responseText}");
            
            if (success)
            {
                try {
                    var response = JsonUtility.FromJson<VerificationResponse>(responseText);
                    Debug.Log($"Parsed response - success: {response.success}, message: {response.message}");
                    callback(response.success, response.message);
                }
                catch (System.Exception ex) {
                    Debug.LogError($"Error parsing verification response: {ex.Message}");
                    Debug.LogError($"Full response: {responseText}");
                    callback(false, "Error processing verification response. Try again.");
                }
            }
            else
            {
                callback(false, $"Verification failed. Server response: {responseText}");
            }
        });
    }

    
    public IEnumerator TestServerConnection()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(serverUrl))
        {
            // Set short timeout
            www.timeout = 5;
            yield return www.SendWebRequest();

            _isServerReachable = (www.result == UnityWebRequest.Result.Success);
            
            if (_isServerReachable)
                Debug.Log(" Connected to server.");
            else
                Debug.LogWarning(" Server not reachable. Check URL or server status.");
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
            if (string.IsNullOrEmpty(error.error)) return "An unknown error occurred.";
            if (error.error.Contains("already exists")) return "Email already registered.";
            if (error.error.Contains("Invalid email or password")) return "Incorrect email or password.";
            if (error.error.Contains("Database error")) return "Server error. Try again later.";
            return error.error;
        }
        catch
        {
            return "Failed to parse error response.";
        }
    }
}
