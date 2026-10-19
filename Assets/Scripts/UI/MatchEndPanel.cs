using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchEndPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private TMP_Text scoreText;

    [Header("Buttons")]
    [SerializeField] private Button rematchSameRulesButton;
    [SerializeField] private Button rematchNewRulesButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    public event Action<MatchEndOptions> OnOptionSelected;

    private void Awake()
    {
        // Warning: los cuatro listeners son lambdas y no hay OnDestroy que los saque. Una lambda no se puede quitar con RemoveListener porque no tenés la referencia al delegado.
        // Guardá los cuatro métodos con nombre (OnRematchSameRules, etc.) y dalos de baja en OnDestroy, como sí hacés en PlayerSettings.
        rematchSameRulesButton.onClick.AddListener(() => Select(MatchEndOptions.RematchSameRules));
        rematchNewRulesButton.onClick.AddListener(() => Select(MatchEndOptions.RematchNewRules));
        mainMenuButton.onClick.AddListener(() => Select(MatchEndOptions.MainMenu));
        quitButton.onClick.AddListener(() => Select(MatchEndOptions.QuitGame));
    }

    public void Open(PlayerID winner, int playerOneScore, int playerTwoScore)
    {
        gameObject.SetActive(true);

        winnerText.text = winner == PlayerID.One ? "PLAYER ONE WINS" : "PLAYER TWO WINS";
        scoreText.text = $"{playerOneScore} - {playerTwoScore}";
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void Select(MatchEndOptions option)
    {
        Close();
        OnOptionSelected?.Invoke(option);
    }
}