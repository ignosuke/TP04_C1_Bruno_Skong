using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float initialSpeed = 8f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Launch();
    }

    private void Launch()
    {
        Vector2 direction = new Vector2(
            Random.value < 0.5f ? -1f : 1f,
            1
        ).normalized;

        rb.AddForce(direction * initialSpeed, ForceMode2D.Impulse);
    }
}