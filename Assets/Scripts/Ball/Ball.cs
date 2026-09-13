using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private BallDataSo ballData;

    private float currentSpeed;
    private int bounceCount;
    private int increasesApplied;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Launch();
    }

    public void Launch()
    {
        currentSpeed = ballData.initialSpeed;
        bounceCount = 0;
        increasesApplied = 0;

        rb.linearVelocity = Vector2.zero; // Sin esto el impulso se sumaria a la velocidad que traia

        Vector2 direction = new Vector2(
            Random.value < .5f ? -1f : 1f,
            1
        ).normalized;

        rb.AddForce(direction * currentSpeed, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bounceCount++;

        if (bounceCount >= ballData.bouncesToSpeedIncrease)
        {
            bounceCount = 0;
            IncreaseSpeed();
        }

        ApplyCurrentSpeed(); // Se aplica la velocidad actual para corregir un posible cambio cuando haya contacto con un player u otro RigidBody2D de tipo kinematic 
    }

    private void IncreaseSpeed()
    {
        if (increasesApplied >= ballData.speedMultipliers.Length) return; // Se alcanzo el limite de aumentos de velocidad

        currentSpeed *= ballData.speedMultipliers[increasesApplied];
        increasesApplied++;
    }

    private void ApplyCurrentSpeed()
    {
        rb.linearVelocity = rb.linearVelocity.normalized * currentSpeed;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}