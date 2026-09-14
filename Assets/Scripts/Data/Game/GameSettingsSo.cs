using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Data/Game/GameSettings")]
public class GameSettingsSo : ScriptableObject
{
    [Header("Best Of Rounds Preset")]
    public int[] bestOfPresets = { 1, 3, 5, 7 };

    [Header("Match Time Presets")]
    public float[] durationPresets = { 15f, 30f, 45f, 60f };

    [Header("Rules")]
    public float goalTimeLimit = 20f; // Tiempo que permanece la pelota, al expirar se cuenta un gol para el jugador que no la tenga de su lado
    public float serveDelay = 1f;   // Ventana de tiempo desde que la pelota reaparece hasta que se mueve
    public float roundEndDelay = 1f;   // Cuanto tiempo sigue viajando la pelota despues del gol
    public PlayerID timeoutTiebreaker = PlayerID.One; // Si la pelota esta exactamente en el centro al expirar el timer, el gol va para este jugador

    public int[] GetBestOfPresets() => bestOfPresets;
    public float[] GetDurationPresets() => durationPresets;

    public float GetGoalTimeLimit() => goalTimeLimit;
    public float GetServeDelay() => serveDelay;
    public float GetRoundEndDelay() => roundEndDelay;
    public PlayerID GetTimeoutTiebreaker() => timeoutTiebreaker;

    public int GetDefaultBestOf() => bestOfPresets.Length > 0 ? bestOfPresets[0] : 5;
}