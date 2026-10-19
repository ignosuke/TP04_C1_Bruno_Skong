using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private UIPanel pausePanel;
    [SerializeField] private UIPanel settingsPanel;
    [SerializeField] private UIPanel creditsPanel;

    [Header("Pause Menu Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button pauseSettingsButton;
    [SerializeField] private Button pauseCreditsButton;
    [SerializeField] private Button pauseExitButton;
    [SerializeField] private Button pauseBackToMenuButton;
    [SerializeField] private Button closePauseMenuButton;

    [Header("Settings Menu Buttons")]
    [SerializeField] private Button settingsBackButton;

    [Header("Credits Menu Buttons")]
    [SerializeField] private Button creditsBackButton;

    private bool isPaused = false;
    private void Awake()
    {
        pausePanel.Close();
        settingsPanel.Close();
        creditsPanel.Close();

        // Listeners
        continueButton.onClick.AddListener(ResumeGame);
        pauseSettingsButton.onClick.AddListener(OpenSettings);
        pauseCreditsButton.onClick.AddListener(OpenCredits);
        pauseExitButton.onClick.AddListener(ExitGame);
        pauseBackToMenuButton.onClick.AddListener(BackToMenu);
        closePauseMenuButton.onClick.AddListener(ResumeGame);

        settingsBackButton.onClick.AddListener(CloseSettings);
        creditsBackButton.onClick.AddListener(CloseCredits);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Warning: el OnDestroy da de baja seis botones pero se saltea settingsBackButton y creditsBackButton, que se registran en el Awake (líneas 41 y 42).
    // Agregalos. Y ojo con RemoveAllListeners: borra también lo que alguien haya cableado desde el inspector; si vos registraste con AddListener, dá de baja con RemoveListener del mismo método.
    private void OnDestroy()
    {
        continueButton.onClick.RemoveAllListeners();
        pauseSettingsButton.onClick.RemoveAllListeners();
        pauseCreditsButton.onClick.RemoveAllListeners();
        pauseExitButton.onClick.RemoveAllListeners();
        pauseBackToMenuButton.onClick.RemoveAllListeners();
        closePauseMenuButton.onClick.RemoveAllListeners();
    }

    // Botones
    private void PauseGame()
    {
        pausePanel.Open();
        Time.timeScale = 0f;
        isPaused = true;
    }

    private void ResumeGame()
    {
        if (settingsPanel.IsOpen() || creditsPanel.IsOpen()) return; // Si hay un panel abierto, no se puede reanudar el juego

        pausePanel.Close();
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Los paneles se muestran encima y se ocultan al salir
    private void OpenSettings()
    {
        settingsPanel.Open();
    }

    private void OpenCredits()
    {
        creditsPanel.Open();
    }

    private void CloseSettings()
    {
        settingsPanel.Close();
    }

    private void CloseCredits()
    {
        creditsPanel.Close();
    }
    private void BackToMenu()
    {
        pausePanel.Close();
        isPaused = false;

        // Bug: se vuelve al menú desde la pausa, con Time.timeScale en 0, y acá no se lo devuelve a 1. MainMenuUIManager tampoco lo toca, así que la escena de menú queda congelada y, al apretar Play de nuevo, la partida arranca con el tiempo detenido.
        // Poné Time.timeScale = 1f antes del LoadScene, igual que hacés en ResumeGame.
        SceneManager.LoadScene("MainMenuScene");
    }

    private void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
