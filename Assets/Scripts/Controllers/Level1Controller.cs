using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1Controller : MonoBehaviour
{
    private void awake()
    {
        // Force landscape orientation for Level 1
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        
        // Reset game states when level starts
        PlayerJump.canJump = true;
        ObstacleSpawner.canSpawn = true;
        ShieldSpawner.canSpawn = true;
    }
} 