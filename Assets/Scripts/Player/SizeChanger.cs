using UnityEngine;

public class SizeChanger : MonoBehaviour
{
    [SerializeField] private PlayerDataSo data;
    private PlayerID playerId;

    private const float minWidth = 1f;
    private const float maxWidth = 3f;
    private float width = 2f;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        playerId = data.playerId;
    }

    private void Start()
    {
        SetWidth(PlayerPersistentData.GetWidth(playerId));
    }

    // Igual que en Movement, suscribe y desuscribe para actualizarse cuando Settings modifique PlayerData
    private void OnEnable()
    {
        PlayerPersistentData.OnWidthChanged += HandleWidthChanged;
    }

    private void OnDisable()
    {
        PlayerPersistentData.OnWidthChanged -= HandleWidthChanged;
    }

    private void HandleWidthChanged(PlayerID id, float value)
    {
        if (id == playerId)
            SetWidth(value);
    }

    public void SetWidth(float newWidth)
    {
        width = Mathf.Clamp(newWidth, minWidth, maxWidth);
        sr.size = new Vector2(width, sr.size.y);
    }
}