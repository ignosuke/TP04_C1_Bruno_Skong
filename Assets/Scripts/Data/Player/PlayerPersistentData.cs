using System;
using UnityEngine;

// Esto serviría de pseudo base de datos de momento
// Archivo de configuración simple que persiste entre escenas
// Los jugadores y sus scripts tienen un identificador ID con el que se diferencia qué modifica Settings y que lee cada jugador

public static class PlayerPersistentData
{
    public static bool isInitialized = false; // Para que solo se inicialice una vez por partida

    public static float playerOneSpeed = 5f; // Entre 0.5 y 20
    public static float playerOneWidth = 2f; // Entre 1 y 3
    public static Color playerOneColor = Color.white; // R/G/B, C/Y/M o White

    public static float playerTwoSpeed = 5f;
    public static float playerTwoWidth = 2f;
    public static Color playerTwoColor = Color.white;

    // Actions a las que suscribirán los respectivos scripts del jugador para actualizar sus valores en tiempo real
    public static event Action<PlayerID, float> OnSpeedChanged;
    public static event Action<PlayerID, float> OnWidthChanged;
    public static event Action<PlayerID, Color> OnColorChanged;

    // Copia los valores de diseño del SO al estado runtime, solo la primera vez que se llama para que no se pierdan los cambios de Settings en runtime
    public static void InitializeFromData(PlayerDataSo playerOneData, PlayerDataSo playerTwoData)
    {
        if (isInitialized) return;

        ApplyInitialData(playerOneData);
        ApplyInitialData(playerTwoData);

        isInitialized = true;
    }

    private static void ApplyInitialData(PlayerDataSo data)
    {
        if (data.playerId == PlayerID.One)
        {
            playerOneSpeed = data.speed;
            playerOneWidth = data.width;
            playerOneColor = data.color;
        }
        else
        {
            playerTwoSpeed = data.speed;
            playerTwoWidth = data.width;
            playerTwoColor = data.color;
        }
    }

    public static float GetSpeed(PlayerID id) => id == PlayerID.One ? playerOneSpeed : playerTwoSpeed;
    public static float GetWidth(PlayerID id) => id == PlayerID.One ? playerOneWidth : playerTwoWidth;
    public static Color GetColor(PlayerID id) => id == PlayerID.One ? playerOneColor : playerTwoColor;

    // Los setters serán llamados desde Settings y dispararán los Actions a los que suscriben los scripts de los jugadores
    public static void SetSpeed(PlayerID id, float value)
    {
        if (id == PlayerID.One) playerOneSpeed = value;
        else playerTwoSpeed = value;

        OnSpeedChanged?.Invoke(id, value);
    }

    public static void SetWidth(PlayerID id, float value)
    {
        if (id == PlayerID.One) playerOneWidth = value;
        else playerTwoWidth = value;

        OnWidthChanged?.Invoke(id, value);
    }

    public static void SetColor(PlayerID id, Color value)
    {
        if (id == PlayerID.One) playerOneColor = value;
        else playerTwoColor = value;

        OnColorChanged?.Invoke(id, value);
    }
}
