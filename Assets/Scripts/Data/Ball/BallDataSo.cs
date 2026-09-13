using UnityEngine;

[CreateAssetMenu(fileName = "BallDataSo", menuName = "Data/Ball/BallData")]
public class BallDataSo : ScriptableObject
{
    public float initialSpeed = 8f;
    public int bouncesToSpeedIncrease = 5;
    public float[] speedMultipliers = { 1.3f, 1.2f, 1.1f }; // El tamaño del array determina la cantidad de veces que se puede aumentar la velocidad
}
