using UnityEngine;

using UnityEngine.SceneManagement;



public class MainMenuManager : MonoBehaviour

{

    // --- Configuration ---



    [Header("Scene Management")]

    public string gameSceneName = "Player";



    [Header("UI References")]

    [Tooltip("Drag the parent Panel/GameObject holding the main menu buttons here.")]

    public GameObject mainMenuPanel;



    [Tooltip("Drag the parent Panel/GameObject holding the settings options here.")]

    public GameObject settingsPanel;



    [Tooltip("Drag the parent Panel/GameObject holding the credits options here.")]

    public GameObject creditsPanel;



    [Header("Animation")]

    public Animator cameraAnimator;

    private const string StartAnimationTrigger = "StartPlay";





    // --- Core Helper Function ---



    /// <summary>

    /// Deactivates all secondary panels (Settings, Credits) and 

    /// sets the specified panel's active state. The Main Menu is shown if no panel is active.

    /// </summary>

    /// <param name="panelToShow">The panel to be activated (e.g., settingsPanel or creditsPanel). 

    /// Pass null to just show the Main Menu.</param>

    private void SetPanelActive(GameObject panelToShow)

    {

        // 1. Deactivate all secondary panels first

        if (settingsPanel != null)

            settingsPanel.SetActive(false);

        if (creditsPanel != null)

            creditsPanel.SetActive(false);



        // 2. Activate the requested panel

        if (panelToShow != null)

        {

            panelToShow.SetActive(true);

        }



        // 3. Set Main Menu state based on whether a secondary panel is open

        // If panelToShow is null, it means we want to go back to the Main Menu.

        if (mainMenuPanel != null)

        {

            // The Main Menu is active ONLY if we are NOT showing another panel.

            mainMenuPanel.SetActive(panelToShow == null);

        }

    }





    // --- Menu Functions ---



    /// <summary>

    /// Starts the camera animation and hides the main menu UI.

    /// </summary>

    public void PlayGame()

    {

        // Hide all UI before the animation starts

        if (mainMenuPanel != null)

        {

            Debug.Log("Deactivating Main Menu Panel.");

            mainMenuPanel.SetActive(false);

        }



        // ... (Animation and loading logic remains the same)

        if (cameraAnimator != null)

        {

            Debug.Log("Starting Camera Animation...");

            cameraAnimator.SetTrigger(StartAnimationTrigger);

        }

        else

        {

            Debug.LogError("Camera Animator not assigned! Loading scene directly.");

            LoadGameScene();

        }

    }



    /// <summary>

    /// Loads the main game scene (called by the Animation Event).

    /// </summary>

    public void LoadGameScene()

    {

        Debug.Log("Loading the game scene: " + gameSceneName);

        SceneManager.LoadScene(gameSceneName);

    }



    /// <summary>

    /// Opens the Settings UI panel and hides the Main Menu.

    /// </summary>

    public void OpenSettings()

    {

        Debug.Log("Opening Settings Panel.");

        SetPanelActive(settingsPanel);

    }



    /// <summary>

    /// Opens the Credits UI panel and hides the Main Menu.

    /// </summary>

    public void ShowCredits()

    {

        Debug.Log("Opening Credits Panel.");

        SetPanelActive(creditsPanel);

    }



    /// <summary>

    /// Hides all secondary panels and shows the Main Menu.

    /// This is used for "Back" buttons in Settings or Credits.

    /// </summary>

    public void CloseSecondaryPanels()

    {

        Debug.Log("Closing secondary panel and returning to Main Menu.");

        SetPanelActive(null); // Passing null tells the helper function to show only the Main Menu

    }





    /// <summary>

    /// Quits the application.

    /// </summary>

    public void QuitGame()

    {

        Debug.Log("Quitting the game...");



        // Quits the application (works in a build)

        Application.Quit();



        // This line is for testing in the Unity Editor

#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#endif

    }

}