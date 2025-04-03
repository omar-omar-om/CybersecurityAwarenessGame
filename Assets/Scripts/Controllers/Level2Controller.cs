using UnityEngine;

public class Level2Controller : MonoBehaviour
{
    private void Start()
    {
        // Force landscape orientation for Level 2
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        
        // Reset game states when level starts
        PlayerJump.canJump = true;
        ObstacleSpawner.canSpawn = true;
        ShieldSpawner.canSpawn = true;
    }
} 