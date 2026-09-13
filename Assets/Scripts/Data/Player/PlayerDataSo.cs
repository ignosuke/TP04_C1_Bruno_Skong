using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player/PlayerData")]
public class PlayerDataSo : ScriptableObject
{
    public PlayerID playerId = PlayerID.One;
    public MovementKeys movementKeys = MovementKeys.WASD;
    public float speed = 5f; // Entre 0.5 y 20
    public float width = 2f; // Entre 1 y 3
    public Color color = Color.white; // R/G/B, C/Y/M o White
    public float colorFlashHoldDuration = .5f;
    public float colorFlashFadeDuration = .5f;
}
