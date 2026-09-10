using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private UIPanel mainMenuPanel;
    [SerializeField] private UIPanel settingsPanel;
    [SerializeField] private UIPanel creditsPanel;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button mainMenuSettingsButton;
    [SerializeField] private Button mainMenuCreditsButton;
    [SerializeField] private Button mainMenuExitButton;

    [Header("Settings Menu Buttons")]
    [SerializeField] private Button settingsBackButton;

    [Header("Credits Menu Buttons")]
    [SerializeField] private Button creditsBackButton;

    private void Awake()
    {
        // Main Menu abierto al inicio, el resto oculto
        mainMenuPanel.Open();
        settingsPanel.Close();
        creditsPanel.Close();

        // Listeners
        playButton.onClick.AddListener(Play);
        mainMenuSettingsButton.onClick.AddListener(OpenSettings);
        mainMenuCreditsButton.onClick.AddListener(OpenCredits);
        mainMenuExitButton.onClick.AddListener(ExitGame);

        settingsBackButton.onClick.AddListener(CloseSettings);
        creditsBackButton.onClick.AddListener(CloseCredits);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveAllListeners();
        mainMenuSettingsButton.onClick.RemoveAllListeners();
        mainMenuCreditsButton.onClick.RemoveAllListeners();
        mainMenuExitButton.onClick.RemoveAllListeners();

        settingsBackButton.onClick.RemoveAllListeners();
        creditsBackButton.onClick.RemoveAllListeners();
    }

    // Botones

    private void Play()
    {
        SceneManager.LoadScene("GameScene");
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

    private void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}