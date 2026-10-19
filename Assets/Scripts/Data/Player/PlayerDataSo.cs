using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player/PlayerData")]
public class PlayerDataSo : ScriptableObject
{
    // Warning: los siete campos del SO son public. Acá el riesgo es menor porque PlayerPersistentData copia los valores una sola vez al arrancar, pero mientras sean public cualquier script puede escribir el asset en runtime y en el editor eso queda grabado.
    // [SerializeField] private más un getter por campo, igual que hiciste en CourtBoundsSo, que es el ejemplo bien resuelto del proyecto.
    public PlayerID playerId = PlayerID.One;
    public MovementKeys movementKeys = MovementKeys.WASD;
    public float speed = 5f; // Entre 0.5 y 20
    public float width = 2f; // Entre 1 y 3
    public Color color = Color.white; // R/G/B, C/Y/M o White
    public float colorFlashHoldDuration = .5f;
    public float colorFlashFadeDuration = .5f;
}
