using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private BallDataSo ballData;

    // Componente minima en cada eje (sobre el vector normalizado).
    // Evita que la pelota quede rebotando en linea recta horizontal o vertical.
    private const float minDirectionComponent = .25f;

    private float currentSpeed;
    private int bounceCount;
    private int increasesApplied;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void StopAndReset()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = Vector2.zero;
        transform.position = Vector2.zero; // Corta la interpolacion hacia el centro

        currentSpeed = ballData.initialSpeed;
        bounceCount = 0;
        increasesApplied = 0;
    }

    public void Launch(Vector2 direction)
    {
        rb.AddForce(direction.normalized * currentSpeed, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bounceCount++;

        if (bounceCount >= ballData.bouncesToSpeedIncrease)
        {
            bounceCount = 0;
            IncreaseSpeed();
        }

        // Se aplica la velocidad actual para corregir un posible cambio cuando haya contacto con un player u otro RigidBody2D de tipo kinematic 
        ApplyCurrentSpeed();
    }

    private void IncreaseSpeed()
    {
        if (increasesApplied >= ballData.speedMultipliers.Length) return; // Ya llego al maximo

        currentSpeed *= ballData.speedMultipliers[increasesApplied];
        increasesApplied++;
    }

    private void ApplyCurrentSpeed()
    {
        // linearVelocity ya trae la direccion posterior al rebote, solo se le corrige el modulo
        Vector2 direction = ClampDirection(rb.linearVelocity.normalized);

        rb.linearVelocity = direction * currentSpeed;
    }

    // Forzamos un angulo minimo respecto de los dos ejes para evitar que la pelota quede rebotando en linea recta horizontal o vertical
    // Un minimo ni tan bajo como para parecer recto ni tan alto que se note demasiado artificial
    private Vector2 ClampDirection(Vector2 direction)
    {
        // Se separa signo de magnitud para poder levantar el minimo sin perder el sentido.
        // No se usa Mathf.Sign porque devuelve 0 cuando la componente es 0.
        float signX = direction.x >= 0f ? 1f : -1f;
        float signY = direction.y >= 0f ? 1f : -1f;

        float x = Mathf.Max(Mathf.Abs(direction.x), minDirectionComponent);
        float y = Mathf.Max(Mathf.Abs(direction.y), minDirectionComponent);

        return new Vector2(x * signX, y * signY).normalized;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public float GetPositionX()
    {
        return rb.position.x; // Para saber de qué lado de la cancha está la pelota y a quién se le debe contar el gol
    }
}