using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    // Vida actual del enemigo.
    protected float currentHP;

    [SerializeField]
    protected float maxHP = 10; // Vida máxima del enemigo.

    [SerializeField]
    protected int attackDamage = 1; // Daño que inflige el enemigo al atacar.

    [SerializeField]
    protected float moveSpeed = 2f; // Velocidad de movimiento del enemigo.

    [SerializeField]
    protected float detectionRange = 50f; // Rango de detección del jugador.

    [SerializeField]
    private LayerMask bulletLayer; // **Nueva variable para detectar balas usando LayerMask**

    // Componentes del enemigo.
    protected Rigidbody rb; // Referencia al Rigidbody del enemigo.
    protected Transform player; // Referencia al jugador.
    protected SteeringBehaviors steering; // 🔹 Integración con SteeringBehaviors para el movimiento.

    /// <summary>
    /// Método Start: Inicializa el enemigo asignando sus componentes y verificando si el jugador existe en la escena.
    /// </summary>
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Evita que el Rigidbody rote automáticamente.
        rb.useGravity = false; // Desactiva la gravedad para enemigos flotantes o aéreos.

        currentHP = maxHP; // Inicializa la vida del enemigo.
        steering = GetComponent<SteeringBehaviors>(); // 🔹 Verifica si este enemigo usa SteeringBehaviors.

        // Busca al jugador en la escena utilizando la etiqueta "Player".
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform; // Asigna la referencia del jugador.
        }
        else
        {
            Debug.LogError("❌ No se encontró el jugador. Asegúrate de que el jugador tiene la etiqueta 'Player'.");
        }
    }

    /// <summary>
    /// Método FixedUpdate: Maneja el movimiento del enemigo en cada frame de física.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        if (player == null) return; // Si no hay jugador, no hace nada.

        // 🔹 Si el enemigo tiene SteeringBehaviors, delega el movimiento a ese script.
        if (steering != null)
        {
            steering.SetEnemyReference(player.gameObject); // Asegura que siga al jugador.
            return;
        }

        // 🔹 Si NO tiene SteeringBehaviors, usa el movimiento normal basado en la distancia al jugador.
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange) // Si el jugador está dentro del rango de detección.
        {
            Vector3 direction = (player.position - transform.position).normalized; // Calcula la dirección hacia el jugador.
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime); // Mueve al enemigo en esa dirección.
        }
    }

    /// <summary>
    /// Método para recibir daño.
    /// </summary>
    /// <param name="damage">Cantidad de daño recibido.</param>
    public void TakeDamage(float damage)
    {
        currentHP -= damage; // Reduce la vida del enemigo.
        Debug.Log($"{name} recibió {damage} de daño. HP restante: {currentHP}");

        if (currentHP <= 0) // Si la vida del enemigo llega a 0, muere.
        {
            Die();
        }
    }

    /// <summary>
    /// Método para manejar la muerte del enemigo.
    /// </summary>
    protected virtual void Die()
    {
        Debug.Log($"{name} ha sido destruido."); // Mensaje de depuración.
        Destroy(gameObject); // Elimina al enemigo de la escena.
    }

    /// <summary>
    /// Método llamado cuando otro objeto entra en el área de colisión del enemigo.
    /// </summary>
    /// <param name="other">Collider del objeto que entró en contacto.</param>
    private void OnTriggerEnter(Collider other)
    {
        // **1️⃣ Si es una bala, recibe daño**
        if (((1 << other.gameObject.layer) & bulletLayer) != 0)
        {
            Bullet bullet = other.GetComponent<Bullet>(); // Obtiene la referencia al script de la bala.
            if (bullet != null)
            {
                TakeDamage(bullet.GetDamage()); // Aplica el daño al enemigo.
                Destroy(other.gameObject); // Destruye la bala tras el impacto.
            }
        }

        // **2️⃣ Si el enemigo choca con el `Player`, inflige daño**
        if (other.CompareTag("Player")) // ⬅️ Verifica que el `Player` tiene el `Tag` correcto
        {
            Player playerScript = other.GetComponent<Player>(); // ⬅️ Obtiene el script `Player`
            if (playerScript != null)
            {
                playerScript.TakeDamage(attackDamage); // ✅ Inflige daño al jugador
                Debug.Log($"⚠️ {gameObject.name} golpeó al jugador. Le hizo {attackDamage} de daño.");
            }
        }
    }
}
