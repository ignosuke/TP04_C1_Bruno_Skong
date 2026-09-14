using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameSettingsSo gameSettings;
    [SerializeField] private Ball ball;

    [Header("Goal Zones")]
    [SerializeField] private GoalZone goalZoneOne;
    [SerializeField] private GoalZone goalZoneTwo;

    [Header("UI")]
    [SerializeField] private RulesPanel rulesPanel;
    [SerializeField] private MatchEndPanel matchEndPanel;
    [SerializeField] private GameObject gameHUD;

    [Header("Players")]
    [SerializeField] private Movement playerOneMovement;
    [SerializeField] private Movement playerTwoMovement;

    private readonly MatchSettings matchSettings = new();

    public event Action OnScoreChanged;
    public event Action<GameState> OnStateChanged;

    private GameState state;
    private float serveTimer;
    private float goalTimer;  // Limite hasta decidir el gol segun la posicion de la pelota, se reinicia por ronda
    private float matchTimer; // Tiempo restante para el modo por tiempo
    private float elapsedTime; // Tiempo transcurrido para el modo por rondas
    private float roundEndTimer; // Tiempo que sigue viajando la pelota despues del gol, se reinicia por ronda

    private int playerOneScore;
    private int playerTwoScore;
    private PlayerID nextServeTo;
    private PlayerID lastScorer;
    private PlayerID winner;

    private void OnEnable()
    {
        goalZoneOne.OnGoalConceded += HandleGoal;
        goalZoneTwo.OnGoalConceded += HandleGoal;
        rulesPanel.OnRulesConfirmed += HandleRulesConfirmed;
        matchEndPanel.OnOptionSelected += HandleMatchEndOptions;
    }

    private void OnDisable()
    {
        goalZoneOne.OnGoalConceded -= HandleGoal;
        goalZoneTwo.OnGoalConceded -= HandleGoal;
        rulesPanel.OnRulesConfirmed -= HandleRulesConfirmed;
        matchEndPanel.OnOptionSelected -= HandleMatchEndOptions;
    }

    private void Start()
    {
        EnterRuleSelection();
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.RuleSelection:
                break; // Se espera a que el panel confirme

            case GameState.Serving:
                UpdateServing(); // El matchTimer no corre: la espera del saque no consume tiempo de partida
                break;

            case GameState.Playing:
                UpdateMatchTimer();
                UpdateGoalTimer();
                break;

            case GameState.RoundEnd:
                UpdateRoundEnd(); // La pelota se mueve sola, aca solo se espera
                break;

            case GameState.MatchEnd:
                break;
        }
    }

    private void EnterRuleSelection()
    {
        ball.StopAndReset();
        SetPlayersEnabled(false);

        rulesPanel.Open();
        SetState(GameState.RuleSelection);
    }

    private void HandleRulesConfirmed(MatchMode mode, int presetIndex)
    {
        if (mode == MatchMode.Rounds)
            matchSettings.SetRoundsMode(gameSettings.GetBestOfPresets()[presetIndex]);
        else
            matchSettings.SetTimedMode(gameSettings.GetDurationPresets()[presetIndex]);

        StartMatch();
    }

    private void StartMatch()
    {
        playerOneScore = 0;
        playerTwoScore = 0;
        matchTimer = matchSettings.GetDuration();
        elapsedTime = 0f;

        SetPlayersEnabled(true);
        gameHUD.SetActive(true);

        OnScoreChanged?.Invoke();

        // El primer saque es para un lado random
        BeginServe(UnityEngine.Random.value < .5f ? PlayerID.One : PlayerID.Two);
    }

    private void BeginServe(PlayerID serveTo)
    {
        nextServeTo = serveTo;
        ball.StopAndReset();

        serveTimer = gameSettings.GetServeDelay();
        goalTimer = gameSettings.GetGoalTimeLimit(); // Se reinicia por ronda, no por golpe
        SetState(GameState.Serving);
    }

    private void UpdateServing()
    {
        serveTimer -= Time.deltaTime;

        if (serveTimer > 0f) return;

        LaunchBall();
        SetState(GameState.Playing);
    }

    private void UpdateMatchTimer()
    {
        elapsedTime += Time.deltaTime; // Corre en los dos modos

        if (matchSettings.GetMode() != MatchMode.Timed) return;

        matchTimer -= Time.deltaTime;

        if (matchTimer > 0f) return;

        matchTimer = 0f;
        EndTimedMatch();
    }

    private void UpdateGoalTimer()
    {
        goalTimer -= Time.deltaTime;

        if (goalTimer > 0f) return;

        PlayerID concedingPlayer = GetPlayerOnBallSide();

        ResolveGoal(concedingPlayer);
    }

    private void LaunchBall()
    {
        float directionX = nextServeTo == PlayerID.One ? -1f : 1f;
        float directionY = UnityEngine.Random.value < .5f ? -1f : 1f;

        ball.Launch(new Vector2(directionX, directionY));
    }

    // Suscrito a los eventos de las GoalZones, recibe el PlayerID del jugador que recibio el gol
    private void HandleGoal(PlayerID concedingPlayer)
    {
        // Si la partida ya termino o la pelota todavia no salio, se ignora
        if (state != GameState.Playing) return;

        ResolveGoal(concedingPlayer);
    }

    private void ResolveGoal(PlayerID concedingPlayer)
    {
        PlayerID scoringPlayer = GetOpponent(concedingPlayer);
        AddPoint(scoringPlayer);

        lastScorer = scoringPlayer;
        OnScoreChanged?.Invoke();

        if (HasWonMatch(scoringPlayer))
        {
            EndMatch(scoringPlayer);
            return;
        }

        // No se toca la pelota todavia: sigue viajando hasta que expire roundEndTimer
        roundEndTimer = gameSettings.GetRoundEndDelay();
        nextServeTo = concedingPlayer; // Saca el que recibio el gol
        SetState(GameState.RoundEnd);
    }

    private void UpdateRoundEnd()
    {
        roundEndTimer -= Time.deltaTime;

        if (roundEndTimer > 0f) return;

        BeginServe(nextServeTo);
    }

    private bool HasWonMatch(PlayerID playerId)
    {
        // En modo por tiempo no se gana por puntos: lo resuelve el timer de partida
        if (matchSettings.GetMode() != MatchMode.Rounds) return false;

        return GetScore(playerId) >= matchSettings.GetRoundsToWin();
    }

    private void EndTimedMatch()
    {
        if (playerOneScore == playerTwoScore)
        {
            PlayerID concedingPlayer = GetPlayerOnBallSide();
            AddPoint(GetOpponent(concedingPlayer));

            lastScorer = GetOpponent(concedingPlayer);
            OnScoreChanged?.Invoke();
        }

        EndMatch(playerOneScore > playerTwoScore ? PlayerID.One : PlayerID.Two);
    }

    private void EndMatch(PlayerID matchWinner)
    {
        // No se resetea la pelota: termina de salir de cancha y ahi se queda
        // El proximo BeginServe la devuelve al centro
        winner = matchWinner;

        SetPlayersEnabled(false);
        SetState(GameState.MatchEnd);

        matchEndPanel.Open(winner, playerOneScore, playerTwoScore);
    }

    private void HandleMatchEndOptions(MatchEndOptions option)
    {
        switch (option)
        {
            case MatchEndOptions.RematchSameRules:
                StartMatch();
                break;

            case MatchEndOptions.RematchNewRules:
                EnterRuleSelection();
                break;

            case MatchEndOptions.MainMenu:
                SceneManager.LoadScene("MainMenuScene");
                break;

            case MatchEndOptions.QuitGame:
                Application.Quit();
                break;
        }
    }

    private void SetState(GameState newState)
    {
        state = newState;
        OnStateChanged?.Invoke(state);
    }

    private void SetPlayersEnabled(bool isEnabled)
    {
        playerOneMovement.enabled = isEnabled;
        playerTwoMovement.enabled = isEnabled;
    }

    private void AddPoint(PlayerID playerId)
    {
        if (playerId == PlayerID.One)
            playerOneScore++;
        else
            playerTwoScore++;
    }

    private PlayerID GetPlayerOnBallSide()
    {
        float ballX = ball.GetPositionX();

        if (ballX < 0f) return PlayerID.One; // P1 esta a la izquierda
        if (ballX > 0f) return PlayerID.Two;

        return gameSettings.GetTimeoutTiebreaker(); // Empate exacto en el centro
    }

    public int GetScore(PlayerID playerId)
    {
        return playerId == PlayerID.One ? playerOneScore : playerTwoScore;
    }

    public GameState GetState()
    {
        return state;
    }

    public MatchSettings GetMatchSettings()
    {
        return matchSettings;
    }

    public float GetMatchTimer()
    {
        return matchTimer;
    }
    public float GetElapsedTime()
    {
        return elapsedTime;
    }
    public float GetGoalTimer()
    {
        return goalTimer;
    }

    public float GetServeTimer()
    {
        return serveTimer;
    }

    public PlayerID GetNextServeTo()
    {
        return nextServeTo;
    }

    public PlayerID GetLastScorer()
    {
        return lastScorer;
    }

    public PlayerID GetWinner()
    {
        return winner;
    }

    private PlayerID GetOpponent(PlayerID playerId)
    {
        return playerId == PlayerID.One ? PlayerID.Two : PlayerID.One;
    }
}