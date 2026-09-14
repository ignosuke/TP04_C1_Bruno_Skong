using UnityEngine;

[System.Serializable]
public class MatchSettings
{
    [SerializeField] private MatchMode mode = MatchMode.Rounds;
    [SerializeField] private int roundsToWin = 3;
    [SerializeField] private float duration = 30f;

    public MatchMode GetMode() => mode;
    public int GetRoundsToWin() => roundsToWin;
    public float GetDuration() => duration;

    // Determina los goles necesarios para ganar en el modo de Rondas
    public void SetRoundsMode(int bestOf)
    {
        mode = MatchMode.Rounds;
        roundsToWin = CalculateRoundsToWin(bestOf);
    }

    public static int CalculateRoundsToWin(int bestOf)
    {
        return (bestOf + 1) / 2;
    }

    public void SetTimedMode(float newDuration)
    {
        mode = MatchMode.Timed;
        duration = newDuration;
    }
}