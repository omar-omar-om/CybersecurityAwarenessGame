using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2Controller : MonoBehaviour
{
    private void Awake()
    {
        // Force landscape orientation for Level 2
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        
        // Reset game states when level starts
        PlayerJump.canJump = true;
        ObstacleSpawner.canSpawn = true;
        ShieldSpawner.canSpawn = true;
    }

} 