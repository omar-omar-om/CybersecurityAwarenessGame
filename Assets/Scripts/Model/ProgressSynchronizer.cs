using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ProgressSynchronizer : MonoBehaviour
{
   

    public void SyncProgressWithServer(string userEmail)
    {
        StartCoroutine(FetchAndSyncProgress(userEmail));
    }

    private IEnumerator FetchAndSyncProgress(string userEmail)
    {
        yield return StartCoroutine(NetworkManager.Instance.GetGameProgress(userEmail, 
            (success, bestScoresJson) => {
                if (success)
                {
                    // Parse the server best scores using Newtonsoft.Json
                    Dictionary<string, int> serverScores = JsonConvert.DeserializeObject<Dictionary<string, int>>(bestScoresJson);
                    if (serverScores == null)
                    {
                        serverScores = new Dictionary<string, int>();
                    }
                    
                    // Compare and update with local best scores
                    CompareAndUpdateBestScores(userEmail, serverScores);
                }
            }));
    }

    private void CompareAndUpdateBestScores(string userEmail, Dictionary<string, int> serverScores)
    {
        bool needServerUpdate = false;
        Dictionary<string, int> updatedScores = new Dictionary<string, int>();
        
        // For each server score, compare with local
        foreach (var entry in serverScores)
        {
            string level = entry.Key;
            int serverScore = entry.Value;
            
            // Get local score for this level 
            string localScoreKey = GetScoreKey(level);
            int localScore = PlayerPrefs.GetInt(localScoreKey, 0);
            
            // Keep the higher score
            if (serverScore > localScore)
            {
                // Update local with server score
                PlayerPrefs.SetInt(localScoreKey, serverScore);
                
                // If we're in this level update the ScoreManager
                UpdateScoreManagerIfNeeded(level, serverScore);
            }
            else if (localScore > serverScore)
            {
                // Local score is higher, update server later
                updatedScores[level] = localScore;
                needServerUpdate = true;
            }
        }
        
        // Also check for local scores that aren't on the server
        CheckAdditionalLocalScores(userEmail, serverScores, updatedScores, ref needServerUpdate);
        
        // Save local changes
        PlayerPrefs.Save();
        
        // If we need to update the server, do it now
        if (needServerUpdate && !string.IsNullOrEmpty(userEmail))
        {
            // Convert the dictionary to JSON using Newtonsoft.Json
            string bestScoresJson = JsonConvert.SerializeObject(updatedScores);
            
            // Send update to server (fire and forget)
            StartCoroutine(NetworkManager.Instance.UpdateGameProgressCoroutine(
                userEmail, bestScoresJson));
        }
    }

    
    private void CheckAdditionalLocalScores(string userEmail, Dictionary<string, int> serverScores, 
                                           Dictionary<string, int> updatedScores, ref bool needServerUpdate)
    {
        // Known level keys 
        string[] knownLevels = new string[] { "Level1", "Level2" };
        
        foreach (string level in knownLevels)
        {
            // Skip if this level is already in server scores
            if (serverScores.ContainsKey(level))
                continue;
            
            // Check if we have a local score for this level
            string localScoreKey = GetScoreKey(level);
            int localScore = PlayerPrefs.GetInt(localScoreKey, 0);
            
            // If there's a score, add it to updated scores
            if (localScore > 0)
            {
                updatedScores[level] = localScore;
                needServerUpdate = true;
            }
        }
    }

    
    private string GetScoreKey(string level)
    {
        string userId = PlayerPrefs.GetString("UserID", "");
        return $"{userId}_{level}BestScore";
    }

   
    private void UpdateScoreManagerIfNeeded(string level, int serverScore)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == level)
        {
            if (ScoreManager.Instance != null)
            {
                // Set the best score on the ScoreManager
                ScoreManager.Instance.SetBestScore(serverScore);
            }
        }
    }
} 