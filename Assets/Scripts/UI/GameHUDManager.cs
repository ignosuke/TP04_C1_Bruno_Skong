using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHud : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameSettingsSo gameSettings;

    [Header("Score")]
    [SerializeField] private TMP_Text playerOneScoreText;
    [SerializeField] private TMP_Text playerTwoScoreText;

    [Header("Match Info")]
    [SerializeField] private TMP_Text matchTimerText;
    [SerializeField] private TMP_Text matchModeText;
    [SerializeField] private Slider goalLimitSlider;
    [SerializeField] private TMP_Text goalLimitText;

    [Header("Goal Limit Colors")]
    [SerializeField] private Image goalLimitFill;
    [SerializeField] private Color safeColor = new Color(.6f, .8f, 1f);     // 100%
    [SerializeField] private Color warningColor = new Color(1f, .85f, .3f); // 50%
    [SerializeField] private Color dangerColor = new Color(.9f, .25f, .25f); // 20%

    [Header("Center Message")]
    [SerializeField] private GameObject centerMessagePanel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text infoText;

    private void OnEnable()
    {
        gameManager.OnScoreChanged += RefreshScore;
        gameManager.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        gameManager.OnScoreChanged -= RefreshScore;
        gameManager.OnStateChanged -= HandleStateChanged;
    }

    private void Update()
    {
        // Los relojes cambian todos los frames: no tiene sentido un evento para esto
        GameState state = gameManager.GetState();

        if (state == GameState.Playing)
        {
            RefreshMatchTimer();
            RefreshGoalLimit();
        }
        else if (state == GameState.Serving)
        {
            RefreshServeCountdown();
        }
    }

    private void RefreshScore()
    {
        playerOneScoreText.text = gameManager.GetScore(PlayerID.One).ToString();
        playerTwoScoreText.text = gameManager.GetScore(PlayerID.Two).ToString();
    }

    private void HandleStateChanged(GameState state)
    {
        RefreshMatchInfo();

        switch (state)
        {
            case GameState.RuleSelection:
                centerMessagePanel.SetActive(false);
                break;

            case GameState.Serving:
                centerMessagePanel.SetActive(true);
                messageText.text = "GET READY";
                ResetGoalLimit();
                break;

            case GameState.Playing:
                centerMessagePanel.SetActive(false);
                break;

            case GameState.RoundEnd:
                centerMessagePanel.SetActive(true);
                messageText.text = $"{GetPlayerName(gameManager.GetLastScorer())} SCORES";
                infoText.text = string.Empty;
                break;

            case GameState.MatchEnd:
                centerMessagePanel.SetActive(false);
                gameObject.SetActive(false);
                break;
        }
    }

    private void RefreshMatchInfo()
    {
        MatchSettings settings = gameManager.GetMatchSettings();

        if (settings.GetMode() == MatchMode.Timed)
        {
            matchModeText.text = "TIMED MATCH";
        }
        else
        {
            // El "mejor de" se reconstruye desde las rondas necesarias para ganar
            int bestOf = settings.GetRoundsToWin() * 2 - 1;
            matchModeText.text = $"BEST OF {bestOf}";
        }

        RefreshMatchTimer();
    }

    // En modo por tiempo cuenta hacia atras, en modo por rondas cuenta el tiempo jugado
    private void RefreshMatchTimer()
    {
        MatchSettings settings = gameManager.GetMatchSettings();

        float seconds = settings.GetMode() == MatchMode.Timed
            ? gameManager.GetMatchTimer()
            : gameManager.GetElapsedTime();

        matchTimerText.text = FormatTime(seconds);
    }

    private void RefreshGoalLimit()
    {
        float remaining = gameManager.GetGoalTimer();
        float progress = remaining / gameSettings.GetGoalTimeLimit();

        goalLimitSlider.value = progress;
        goalLimitFill.color = GetGoalLimitColor(progress);
        goalLimitText.text = $"AUTOMATIC GOAL IN {Mathf.CeilToInt(remaining)}S";
    }

    private void ResetGoalLimit()
    {
        goalLimitSlider.value = 1f;
        goalLimitFill.color = safeColor;
        goalLimitText.text = $"AUTOMATIC GOAL IN {Mathf.CeilToInt(gameSettings.GetGoalTimeLimit())}S";
    }

    // Dos tramos: de 100% a 50% va de azul a amarillo, de 50% a 20% de amarillo a rojo.
    // Por debajo de 20% se queda en rojo.
    private Color GetGoalLimitColor(float progress)
    {
        if (progress >= .5f)
            return Color.Lerp(warningColor, safeColor, (progress - .5f) / .5f);

        if (progress >= .2f)
            return Color.Lerp(dangerColor, warningColor, (progress - .2f) / .3f);

        return dangerColor;
    }

    private void RefreshServeCountdown()
    {
        infoText.text = $"SERVE FOR {GetPlayerName(gameManager.GetNextServeTo())}";
    }

    private string FormatTime(float seconds)
    {
        int total = Mathf.CeilToInt(seconds);

        return $"{total / 60:00}:{total % 60:00}";
    }

    private string GetPlayerName(PlayerID playerId)
    {
        return playerId == PlayerID.One ? "PLAYER ONE" : "PLAYER TWO";
    }
}