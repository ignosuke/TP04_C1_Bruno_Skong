using System.Collections;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] private PlayerDataSo playerData;

    private PlayerID playerId;
    private Movement movement;
    private Color baseColor; // El de Settings: el unico cambio que persiste
    private SpriteRenderer spriteRenderer;
    private Coroutine flashRoutine;
    private float colorFlashHoldDuration;
    private float colorFlashFadeDuration;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movement = GetComponent<Movement>();
        playerId = playerData.playerId;
        colorFlashHoldDuration = playerData.colorFlashHoldDuration;
        colorFlashFadeDuration = playerData.colorFlashFadeDuration;
    }

    private void Start()
    {
        SetBaseColor(PlayerPersistentData.GetColor(playerId)); // Lee el valor inicial una vez
    }

    private void OnEnable()
    {
        PlayerPersistentData.OnColorChanged += HandleColorChanged;
        movement.OnClampedChanged += HandleClampedChanged;
        movement.OnBallHit += HandleBallHit;
    }

    private void OnDisable()
    {
        PlayerPersistentData.OnColorChanged -= HandleColorChanged;
        movement.OnClampedChanged -= HandleClampedChanged;
        movement.OnBallHit -= HandleBallHit;

        flashRoutine = null;
        spriteRenderer.color = baseColor;
    }

    private void HandleColorChanged(PlayerID id, Color color)
    {
        if (id == playerId)
            SetBaseColor(color);
    }

    private void SetBaseColor(Color color)
    {
        baseColor = color;

        // Si hay un flash corriendo no se pisa: el fade ya apunta al nuevo baseColor
        if (flashRoutine == null)
            spriteRenderer.color = baseColor;
    }

    private void HandleClampedChanged(bool isClamped)
    {
        if (isClamped)
            Flash(Color.black);
    }

    private void HandleBallHit()
    {
        Flash(GetRandomColor());
    }

    private void Flash(Color flashColor)
    {
        // Si ya habia un flash, se descarta y arranca el nuevo desde cero
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(flashColor));
    }

    private IEnumerator FlashRoutine(Color flashColor)
    {
        spriteRenderer.color = flashColor;

        yield return new WaitForSeconds(colorFlashHoldDuration);

        float elapsed = 0f;

        while (elapsed < colorFlashFadeDuration)
        {
            elapsed += Time.deltaTime;
            // baseColor se lee cada frame: si Settings lo cambia a mitad del fade, termina en el nuevo
            spriteRenderer.color = Color.Lerp(flashColor, baseColor, elapsed / colorFlashFadeDuration);
            yield return null;
        }

        spriteRenderer.color = baseColor;
        flashRoutine = null;
    }

    private Color GetRandomColor()
    {
        // Saturacion y brillo acotados para evitar confundir con el color al colisionar con muros
        return Random.ColorHSV(0f, 1f, .6f, 1f, .7f, 1f);
    }
}