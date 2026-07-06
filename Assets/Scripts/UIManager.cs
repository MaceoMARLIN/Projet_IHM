using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private Canvas gameOverMenu;
    [SerializeField] private Canvas startMenu;
    [SerializeField] private Canvas InGameUI;
    [SerializeField] private GameObject FlecheUI; // Reference to the TargetArrowUI GameObject
    [SerializeField] private TMPro.TextMeshProUGUI timerText; // Reference to the timer TextMeshProUGUI

    private void Start()
    {
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


}