using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Announcement : MonoBehaviour
{
    public GameObject textPrefab;
    public GameObject pausePanel;
    public bool isPause = false;

    [Header("Input Settings")]
    [Tooltip("Drag the 'Pause' Action from your Input Action Asset here")]
    public InputActionReference pauseAction;

    public static Announcement Instance { get; private set; }
    public NPCQuest villageTutorial;
    public ExtractionTutorial extractionTutorial;
    public StoreTutorial storeTutorial;
    public ExploreTutorial exploreTutorial;
    public GameObject gameOverPanel;
    public Teleport teleport;
    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPauseInput;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnPauseInput;
        }
    }
    private void OnPauseInput(InputAction.CallbackContext context)
    {
        PauseGame();
    }

    public void PauseGame()
    {
        isPause = !isPause;
        if (isPause)
            Pause();
        else
            Resume();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        isPause = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        isPause = false;
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void RestartDay()
    {
        teleport.DelayedTeleportPlayer();

        FindAnyObjectByType<PlayerStats>().RestoreHealth(100);
    }

    public void SetAnnouncement(string message)
    {
        if (textPrefab == null) return;
        TextMeshProUGUI text = Instantiate(textPrefab, transform).GetComponent<TextMeshProUGUI>();
        text.text = message;
        Destroy(text.gameObject, 2f);
    }

    public void QuitGame(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void ShowKeyBinds()
    {

    }

    public void ShowLoopPanel()
    {

    }

    public void SkipTutorial()
    {
        if (QuestSystem.Instance != null) QuestSystem.Instance.SkipTutorial();
        Destroy(FindFirstObjectByType<ExtractionTutorial>());
        Destroy(FindFirstObjectByType<StoreTutorial>());
        Destroy(FindFirstObjectByType<ExploreTutorial>());
        Destroy(villageTutorial);
        FindAnyObjectByType<PlayerStats>().UpdateBullets(100);
 
        Resume();
    }
}