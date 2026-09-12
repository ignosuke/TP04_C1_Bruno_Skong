using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private PlayerDataSo data;
    [SerializeField] private CourtBoundsSo courtBounds;

    private PlayerID playerId;

    [Header("Movement Settings")]
    private MovementKeys movementKeys;
    private const float minSpeed = .5f;
    private const float maxSpeed = 20f;
    private float speed = 5f;

    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode leftKey;
    private KeyCode rightKey;

    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        playerId = data.playerId;
        movementKeys = data.movementKeys;

        SetupKeys();
    }

    private void Start()
    {
        SetSpeed(PlayerPersistentData.GetSpeed(playerId)); // Lee el valor inicial una vez
    }

    private void OnEnable()
    {
        PlayerPersistentData.OnSpeedChanged += HandleSpeedChanged;
    }

    private void OnDisable()
    {
        PlayerPersistentData.OnSpeedChanged -= HandleSpeedChanged;
    }

    private void FixedUpdate() // Físicas
    {
        Move();
    }

    private void SetupKeys()
    {
        if (movementKeys == MovementKeys.WASD)
        {
            upKey = KeyCode.W;
            downKey = KeyCode.S;
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
        }
        else
        {
            upKey = KeyCode.UpArrow;
            downKey = KeyCode.DownArrow;
            leftKey = KeyCode.LeftArrow;
            rightKey = KeyCode.RightArrow;
        }
    }

    private void Move()
    {
        Vector2 direction = Vector2.zero;

        if (Input.GetKey(upKey)) direction += Vector2.up;
        if (Input.GetKey(downKey)) direction += Vector2.down;
        if (Input.GetKey(rightKey)) direction += Vector2.right;
        if (Input.GetKey(leftKey)) direction += Vector2.left;

        if (direction == Vector2.zero) return;

        direction = direction.normalized; // Para evitar que el movimiento diagonal sea más rápido que el horizontal o vertical

        Vector2 targetPosition = rb.position + direction * speed * Time.fixedDeltaTime;

        // Se lee cada vez porque SizeChanger puede modificar el tamaño en runtime
        float halfWidth = col.bounds.extents.x;
        float halfHeight = col.bounds.extents.y;

        // El clamp se aplica una sola vez, sobre la posicion final
        // Los limites se achican por el tamaño de la paleta para que tope el borde del sprite y no el pivot que está en el centro
        targetPosition.x = Mathf.Clamp(targetPosition.x, courtBounds.GetMinX(playerId) + halfWidth, courtBounds.GetMaxX(playerId) - halfWidth);

        targetPosition.y = Mathf.Clamp(targetPosition.y, courtBounds.GetBottomY() + halfHeight, courtBounds.GetTopY() - halfHeight);

        rb.MovePosition(targetPosition);
    }

    private void HandleSpeedChanged(PlayerID id, float value)
    {
        if (id == playerId)
            SetSpeed(value);
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = Mathf.Clamp(newSpeed, minSpeed, maxSpeed);
    }
}