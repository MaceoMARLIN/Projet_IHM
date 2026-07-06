using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private Canvas gameOverMenu;
    [SerializeField] private Canvas startMenu;
    [SerializeField] private Canvas InGameUI;
    [SerializeField] private Canvas questionnaireUI;
    [SerializeField] private GameObject FlecheUI; // Reference to the TargetArrowUI GameObject
    [SerializeField] private TMPro.TextMeshProUGUI timerText; // Reference to the timer TextMeshProUGUI
    [SerializeField] private GameObject panelScoreFinal; // Reference to the final score panel


    private bool gameOverShown;

    private void Start()
    {
        gameOverShown = false;
        // Ensure the game starts unpaused
        showStartMenu();
    }

    private void Update()
    {
        // Toggle pause menu with Tab key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (pauseMenu.enabled)
            {
                ResumeGame();
            }
            else
            {
                showPauseMenu();
            }
        }
        timerText.text = "Time: " + Mathf.FloorToInt(Time.timeSinceLevelLoad).ToString() + "s"; // Update the timer text

        if (!gameOverShown && !HasDechetsObjects())
        {
            gameOverShown = true;
            showGameOverMenu();
        }
    }

    private bool HasDechetsObjects()
    {
        int dechetsLayer = LayerMask.NameToLayer("Dechets");

        if (dechetsLayer == -1)
        {
            return false;
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == dechetsLayer)
            {
                return true;
            }
        }

        return false;
    }

    private void showPauseMenu()
    {
        pauseMenu.enabled = true;
        InGameUI.enabled = false;
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }

    private void showGameOverMenu()
    {
        gameOverMenu.enabled = true;
        InGameUI.enabled = false;
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }

    private void showStartMenu()
    {
        startMenu.enabled = true;
        InGameUI.enabled = false;
        pauseMenu.enabled = false;
        gameOverMenu.enabled = false;
        questionnaireUI.enabled = false;
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }

    public void ResumeGame()
    {
        pauseMenu.enabled = false;
        gameOverMenu.enabled = false;
        startMenu.enabled = false;
        InGameUI.enabled = true;
        questionnaireUI.enabled = false;
        Time.timeScale = 1f; // Resume the game
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
    }

    public void QuitGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void ToggleFlecheUI(bool isActive)
    {
        if (FlecheUI != null)
        {
            FlecheUI.SetActive(isActive);
        }
    }

    public void ShowQuestionnaireUI()
    {
        questionnaireUI.enabled = true;
        InGameUI.enabled = false;
        pauseMenu.enabled = false;
        gameOverMenu.enabled = false;
        startMenu.enabled = false;
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume the game
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}