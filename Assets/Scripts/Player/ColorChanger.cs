using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] private PlayerDataSo data;
    private PlayerID playerId;

    private SpriteRenderer sr;
    private Movement movement;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        movement = GetComponent<Movement>();
        playerId = data.playerId;
    }

    private void Start()
    {
        SetColor(PlayerPersistentData.GetColor(playerId));
    }

    // Igual que en Movement, suscribe y desuscribe para actualizarse cuando Settings modifique PlayerData
    private void OnEnable()
    {
        movement.OnClampedChanged += HandleClampedChanged;
        PlayerPersistentData.OnColorChanged += HandleColorChanged;
    }

    private void OnDisable()
    {
        movement.OnClampedChanged -= HandleClampedChanged;
        PlayerPersistentData.OnColorChanged -= HandleColorChanged;
    }

    private void HandleClampedChanged(bool isClamped)
    {
        sr.color = isClamped ? Color.black : PlayerPersistentData.GetColor(playerId);
    }

    private void HandleColorChanged(PlayerID id, Color value)
    {
        if (id == playerId)
            SetColor(value);
    }

    public void SetColor(Color newColor)
    {
        sr.color = newColor;
    }
}