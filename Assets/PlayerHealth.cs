using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movimiento")] // Sección en el Inspector para organizar las variables de movimiento.
    [SerializeField] 
    private float moveSpeed = 5f; // Velocidad de movimiento del jugador.
    private Rigidbody rb; // Referencia al Rigidbody del jugador para aplicar movimiento.
    private Vector3 moveInput; // Vector que almacena la dirección de movimiento.

    [Header("Vida del Jugador")] // Sección en el Inspector para organizar las variables de vida.
    [SerializeField] 
    private int maxHP = 10; // Vida máxima del jugador.
    private int currentHP; // Vida actual del jugador.

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Obtiene la referencia al Rigidbody en el inicio.
        currentHP = maxHP; // Inicializa la vida del jugador con el valor máximo.
    }

    void Update()
    {
        // Captura la entrada del usuario en los ejes X y Z.
        float moveX = Input.GetAxis("Horizontal"); // Entrada del movimiento en el eje horizontal.
        float moveZ = Input.GetAxis("Vertical"); // Entrada del movimiento en el eje vertical.

        // Normaliza el vector de movimiento para mantener una velocidad uniforme en todas direcciones.
        moveInput = new Vector3(moveX, 0f, moveZ).normalized;
    }

    void FixedUpdate()
    {
        // Aplica el movimiento al Rigidbody, moviendo la posición del jugador en función de la entrada.
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    public void TakeDamage(int damage)
    {
        // Reduce la vida del jugador según la cantidad de daño recibido.
        currentHP -= damage;
        Debug.Log($"¡El jugador recibió {damage} de daño! Vida restante: {currentHP}");

        // Si la vida del jugador llega a 0 o menos, se ejecuta el método Die().
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto.");
        // Agregar lógica adicional como reiniciar el nivel o mostrar pantalla de Game Over.
    }
}
