using System;
using UnityEngine;

// Esto serviría de pseudo base de datos de momento
// Archivo de configuración simple que persiste entre escenas
// Los jugadores y sus scripts tienen un identificador ID con el que se diferencia qué modifica Settings y que lee cada jugador

public static class PlayerData
{
    public enum ID
    {
        One,
        Two
    }

    public static float playerOneSpeed = 5f; // Entre 0.5 y 20
    public static float playerOneWidth = 2f; // Entre 1 y 3
    public static Color playerOneColor = Color.white; // R/G/B, C/Y/M o White

    public static float playerTwoSpeed = 5f;
    public static float playerTwoWidth = 2f;
    public static Color playerTwoColor = Color.white;

    // Actions a las que suscribirán los respectivos scripts del jugador para actualizar sus valores en tiempo real
    public static event Action<ID, float> OnSpeedChanged;
    public static event Action<ID, float> OnWidthChanged;
    public static event Action<ID, Color> OnColorChanged;

    public static float GetSpeed(ID id) => id == ID.One ? playerOneSpeed : playerTwoSpeed;
    public static float GetWidth(ID id) => id == ID.One ? playerOneWidth : playerTwoWidth;
    public static Color GetColor(ID id) => id == ID.One ? playerOneColor : playerTwoColor;

    // Los setters serán llamados desde Settings y dispararán los Actions a los que suscriben los scripts de los jugadores
    public static void SetSpeed(ID id, float value)
    {
        if (id == ID.One) playerOneSpeed = value;
        else playerTwoSpeed = value;

        OnSpeedChanged?.Invoke(id, value);
    }

    public static void SetWidth(ID id, float value)
    {
        if (id == ID.One) playerOneWidth = value;
        else playerTwoWidth = value;

        OnWidthChanged?.Invoke(id, value);
    }

    public static void SetColor(ID id, Color value)
    {
        if (id == ID.One) playerOneColor = value;
        else playerTwoColor = value;

        OnColorChanged?.Invoke(id, value);
    }
}
