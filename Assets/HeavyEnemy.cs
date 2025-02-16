using UnityEngine;

/// <summary>
/// Clase que representa un enemigo pesado con aceleración progresiva al detectar al jugador.
/// Hereda de <see cref="BaseEnemy"/>.
/// </summary>
public class HeavyEnemy : BaseEnemy
{
    [SerializeField] private float acceleration = 0.5f; // Aceleración gradual del enemigo al perseguir.
    [SerializeField] private float maxSpeed = 3f; // Velocidad máxima alcanzable.
    
    private bool isChasing = false; // Indica si el enemigo ha detectado al jugador y está en persecución.
    private float currentSpeed = 0f; // Velocidad actual del enemigo.

    /// <summary>
    /// Método FixedUpdate: Controla la detección del jugador y el movimiento del enemigo.
    /// </summary>
    protected override void FixedUpdate()
    {
        // Si el jugador no está presente, no se ejecuta nada.
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Si el jugador está dentro del rango de detección, comienza la persecución.
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }

        // Si el enemigo está persiguiendo al jugador, aumenta progresivamente su velocidad.
        if (isChasing)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            // Incrementa la velocidad actual gradualmente hasta el límite máximo.
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.fixedDeltaTime);

            // Mueve al enemigo en la dirección del jugador con la velocidad calculada.
            rb.MovePosition(rb.position + direction * currentSpeed * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Método OnCollisionEnter: Resetea la velocidad del enemigo si choca contra una pared.
    /// </summary>
    /// <param name="collision">Información de la colisión.</param>
    private void OnCollisionEnter(Collision collision)
    {
        // Si el enemigo colisiona con un objeto con la etiqueta "Wall", detiene su velocidad.
        if (collision.gameObject.CompareTag("Wall"))
        {
            currentSpeed = 0f;
        }
    }
}
