using UnityEngine;

[CreateAssetMenu(fileName = "CourtBounds", menuName = "Data/Court/CourtBounds")]
public class CourtBoundsSo : ScriptableObject
{
    [Header("Límites verticales")]
    [SerializeField] private float topY = 4.6f;
    [SerializeField] private float bottomY = -4.6f;

    [Header("Límites horizontales")]
    [SerializeField] private float outerLimitX = 8f;
    [SerializeField] private float centerGapHalfWidth = 2f;
    
    public float GetTopY() => topY;                                 // Hasta donde puede subir
    public float GetBottomY() => bottomY;                           // Hasta donde puede bajar
    public float GetOuterLimitX() => outerLimitX;                   // Distancia permitida del centro a los extremos
    public float GetCenterGapHalfWidth() => centerGapHalfWidth;     // Distancia permitida hasta el centro desde el lado del jugador

    public float GetMinX(PlayerID playerId)
    {
        return playerId == PlayerID.One ? -outerLimitX : centerGapHalfWidth;    // Si es el jugador 1, el límite mínimo es -outerLimitX (izquierda), si es el jugador 2, es centerGapHalfWidth (derecha)
    }

    public float GetMaxX(PlayerID playerId)
    {
        return playerId == PlayerID.One ? -centerGapHalfWidth : outerLimitX;    // Si es el jugador 1, el límite máximo es -centerGapHalfWidth (izquierda), si es el jugador 2, es outerLimitX (derecha)
    }
}