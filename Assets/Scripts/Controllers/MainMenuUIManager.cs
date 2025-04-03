using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    // drag these buttons in from unity
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button playButton;
    
    private string selectedScene = "";
    // these control how big/small buttons get when selected
    private Vector3 normalScale = Vector3.one;
    private Vector3 selectedScale = new Vector3(1.3f, 1.3f, 1.3f);  // makes button 30% bigger
    private Vector3 unselectedScale = new Vector3(0.8f, 0.8f, 0.8f);  // makes other buttons 20% smaller

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;  // force landscape mode
    }

    private void Start()
    {
        // play button starts disabled until player picks a level
        if (playButton != null)
        {
            playButton.interactable = false;
        }
    }

    // these methods handle when player clicks level buttons
    public void SelectLevel1()
    {
        selectedScene = "Level1";
        UpdateButtonScales(level1Button);
    }

    public void SelectLevel2()
    {
        selectedScene = "Level2";
        UpdateButtonScales(level2Button);
    }

    public void SelectSettings()
    {
        selectedScene = "Settings";
        UpdateButtonScales(settingsButton);
    }

    // handles the button scaling animation when selecting levels
    private void UpdateButtonScales(Button selectedButton)
    {
        // first reset all buttons to normal size
        if (level1Button != null) 
        {
            level1Button.transform.localScale = normalScale;
        }
        if (level2Button != null) 
        {
            level2Button.transform.localScale = normalScale;
        }
        if (settingsButton != null) 
        {
            settingsButton.transform.localScale = normalScale;
        }

        // then make selected button bigger and others smaller
        if (selectedButton != null)
        {
            selectedButton.transform.localScale = selectedScale;
            
            if (level1Button != null && level1Button != selectedButton) 
            {
                level1Button.transform.localScale = unselectedScale;
            }
            if (level2Button != null && level2Button != selectedButton) 
            {
                level2Button.transform.localScale = unselectedScale;
            }
            if (settingsButton != null && settingsButton != selectedButton) 
            {
                settingsButton.transform.localScale = unselectedScale;
            }
        }

        // enable play button once player picks something
        if (playButton != null)
        {
            playButton.interactable = true;
        }
    }

    // loads the selected level when play button is clicked
    public void PlaySelected()
    {
        if (!string.IsNullOrEmpty(selectedScene))
        {
            StartCoroutine(LoadLevelRoutine(selectedScene));
        }
    }

    private IEnumerator LoadLevelRoutine(string sceneName)
    {
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
} 