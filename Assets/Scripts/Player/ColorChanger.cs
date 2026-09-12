using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] private PlayerDataSo data;
    private PlayerID playerId;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        playerId = data.playerId;
    }

    private void Start()
    {
        SetColor(PlayerPersistentData.GetColor(playerId));
    }

    // Igual que en Movement, suscribe y desuscribe para actualizarse cuando Settings modifique PlayerData
    private void OnEnable()
    {
        PlayerPersistentData.OnColorChanged += HandleColorChanged;
    }

    private void OnDisable()
    {
        PlayerPersistentData.OnColorChanged -= HandleColorChanged;
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