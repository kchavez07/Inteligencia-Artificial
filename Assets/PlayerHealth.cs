using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField]
    private float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 moveInput;

    [Header("Vida del Jugador")]
    [SerializeField]
    private int maxHP = 10;
    private int currentHP;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHP = maxHP;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        moveInput = new Vector3(moveX, 0f, moveZ).normalized;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"¡El jugador recibió {damage} de daño! Vida restante: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto.");
        // Agregar lógica como reiniciar nivel, mostrar pantalla de Game Over.
    }
}
