using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private PlayerData.ID playerId;

    private enum MovementKeys
    {
        WASD,
        ArrowKeys,
    }

    [Header("Movement Settings")]
    [SerializeField] private MovementKeys movementKeys = MovementKeys.WASD;
    private const float minSpeed = .5f;
    private const float maxSpeed = 20f;
    private float speed = 5f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        SetSpeed(PlayerData.GetSpeed(playerId)); // Lee el valor inicial una vez
    }


    // Suscribe y desuscribe al evento correspondiente de PlayerData cuando la configuración actualiza el valor
    private void OnEnable()
    {
        PlayerData.OnSpeedChanged += HandleSpeedChanged;
    }

    private void OnDisable()
    {
        PlayerData.OnSpeedChanged -= HandleSpeedChanged;
    }

    private void FixedUpdate() // Físicas
    {
        Move();
    }

    void Move()
    {
        float step = speed * Time.fixedDeltaTime;

        if (movementKeys == MovementKeys.WASD)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rb.MovePosition(rb.position + Vector2.up * step);
            }
            if (Input.GetKey(KeyCode.S))
            {
                rb.MovePosition(rb.position + Vector2.down * step);
            }
        }
        else if (movementKeys == MovementKeys.ArrowKeys)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                rb.MovePosition(rb.position + Vector2.up * step);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                rb.MovePosition(rb.position + Vector2.down * step);
            }
        }

        // Movimiento con rb.AddForce() como alternativa. En este caso prefiero sumar directamente a la posición del Rigidbody2D

        //if (movementKeys == MovementKeys.WASD)
        //{
        //    if (Input.GetKey(KeyCode.W))
        //    {
        //        rb.AddForce(Vector2.up * step);
        //    }
        //    if (Input.GetKey(KeyCode.S))
        //    {
        //        rb.AddForce(Vector2.down * step);
        //    }
        //}
        //else if (movementKeys == MovementKeys.ArrowKeys)
        //{
        //    if (Input.GetKey(KeyCode.UpArrow))
        //    {
        //        rb.AddForce(Vector2.up * step);
        //    }
        //    if (Input.GetKey(KeyCode.DownArrow))
        //    {
        //        rb.AddForce(Vector2.down * step);
        //    }
        //}
    }

    private void HandleSpeedChanged(PlayerData.ID id, float value)
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
