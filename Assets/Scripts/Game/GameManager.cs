using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameSettingsSo gameSettings;
    [SerializeField] private Ball ball;

    [Header("UI")]
    [SerializeField] private RulesPanel rulesPanel;

    [Header("Players")]
    [SerializeField] private Movement playerOneMovement;
    [SerializeField] private Movement playerTwoMovement;

    [Header("Goal Zones")]
    [SerializeField] private GoalZone goalZoneOne;
    [SerializeField] private GoalZone goalZoneTwo;

    private readonly MatchSettings matchSettings = new();

    private GameState state;
    private float serveTimer;
    private float goalTimer;  // Limite hasta decidir el gol segun la posicion de la pelota, se reinicia por ronda
    private float matchTimer; // Duracion para el modo por tiempo
    private float roundEndTimer; // Tiempo que sigue viajando la pelota despues del gol, se reinicia por ronda

    private int playerOneScore;
    private int playerTwoScore;
    private PlayerID nextServeTo;

    private void OnEnable()
    {
        goalZoneOne.OnGoalConceded += HandleGoal;
        goalZoneTwo.OnGoalConceded += HandleGoal;
        rulesPanel.OnRulesConfirmed += HandleRulesConfirmed;
    }

    private void OnDisable()
    {
        goalZoneOne.OnGoalConceded -= HandleGoal;
        goalZoneTwo.OnGoalConceded -= HandleGoal;
        rulesPanel.OnRulesConfirmed -= HandleRulesConfirmed;
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
        state = GameState.RuleSelection;

        ball.StopAndReset();
        SetPlayersEnabled(false);

        rulesPanel.Open();
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

        SetPlayersEnabled(true);

        // El primer saque es para un lado random
        BeginServe(Random.value < .5f ? PlayerID.One : PlayerID.Two);
    }

    private void SetPlayersEnabled(bool isEnabled)
    {
        playerOneMovement.enabled = isEnabled;
        playerTwoMovement.enabled = isEnabled;
    }

    private void BeginServe(PlayerID serveTo)
    {
        nextServeTo = serveTo;
        ball.StopAndReset();

        serveTimer = gameSettings.GetServeDelay();
        goalTimer = gameSettings.GetGoalTimeLimit(); // Se reinicia por ronda, no por golpe
        state = GameState.Serving;
    }

    private void UpdateServing()
    {
        serveTimer -= Time.deltaTime;

        if (serveTimer > 0f) return;

        LaunchBall();
        state = GameState.Playing;
    }

    private void UpdateMatchTimer()
    {
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

        // Gol automatico al jugador que tiene la pelota de su lado
        float ballX = ball.GetPositionX();

        PlayerID concedingPlayer;

        if (ballX < 0f)
            concedingPlayer = PlayerID.One; // P1 esta a la izquierda
        else if (ballX > 0f)
            concedingPlayer = PlayerID.Two;
        else
            concedingPlayer = gameSettings.GetTimeoutTiebreaker(); // Empate exacto en el centro

        Debug.Log($"Timeout de ronda: gol automatico en contra de {concedingPlayer}");
        ResolveGoal(concedingPlayer);
    }

    private void LaunchBall()
    {
        float directionX = nextServeTo == PlayerID.One ? -1f : 1f;
        float directionY = Random.value < .5f ? -1f : 1f;

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

        Debug.Log($"Gol de {scoringPlayer}. Score: {playerOneScore} - {playerTwoScore}");

        if (HasWonMatch(scoringPlayer))
        {
            EndMatch(scoringPlayer);
            return;
        }

        // No se toca la pelota todavia: sigue viajando hasta que expire roundEndTimer
        roundEndTimer = gameSettings.GetRoundEndDelay();
        nextServeTo = concedingPlayer; // Saca el que recibio el gol
        state = GameState.RoundEnd;
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
            // Empatados al final: se sigue jugando y corta el proximo gol
            Debug.Log("Tiempo cumplido con empate: muerte subita");
            return;
        }

        EndMatch(playerOneScore > playerTwoScore ? PlayerID.One : PlayerID.Two);
    }

    private void EndMatch(PlayerID winner)
    {
        SetPlayersEnabled(false);
        // No se resetea la pelota: termina de salir de cancha y ahi se queda
        // El proximo BeginServe la devuelve al centro
        state = GameState.MatchEnd;

        Debug.Log($"Gana {winner}. Score final: {playerOneScore} - {playerTwoScore}");
    }

    private void AddPoint(PlayerID playerId)
    {
        if (playerId == PlayerID.One)
            playerOneScore++;
        else
            playerTwoScore++;
    }

    public int GetScore(PlayerID playerId)
    {
        return playerId == PlayerID.One ? playerOneScore : playerTwoScore;
    }

    public GameState GetState()
    {
        return state;
    }

    private PlayerID GetOpponent(PlayerID playerId)
    {
        return playerId == PlayerID.One ? PlayerID.Two : PlayerID.One;
    }
}