using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] private PlayerData.ID playerId;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetColor(PlayerData.GetColor(playerId));
    }

    // Igual que en Movement, suscribe y desuscribe para actualizarse cuando Settings modifique PlayerData
    private void OnEnable()
    {
        PlayerData.OnColorChanged += HandleColorChanged;
    }

    private void OnDisable()
    {
        PlayerData.OnColorChanged -= HandleColorChanged;
    }

    private void HandleColorChanged(PlayerData.ID id, Color value)
    {
        if (id == playerId)
            SetColor(value);
    }

    public void SetColor(Color newColor)
    {
        sr.color = newColor;
    }
}