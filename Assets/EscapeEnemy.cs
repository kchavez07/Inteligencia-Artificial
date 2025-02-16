using UnityEngine;
using System.Collections;

/// <summary>
/// Clase que representa un enemigo escapista que huye cuando detecta al jugador.
/// Hereda de <see cref="BaseEnemy"/> y utiliza una lógica de detección y evasión.
/// </summary>
public class EscapeEnemy : BaseEnemy
{
    [SerializeField] private float fleeSpeed = 5f; // Velocidad con la que huye del jugador.
    [SerializeField] private float maxFleeDuration = 3f; // Tiempo máximo que puede huir antes de descansar.
    [SerializeField] private float restDuration = 2f; // Tiempo que descansa antes de volver a moverse.
    [SerializeField] private float predictionTime = 1.5f; // Tiempo de predicción usado para la evasión.
    [SerializeField] private LayerMask groundLayer; // Capa que define el suelo para mantener la posición del enemigo.

    private bool isFleeing = false; // Indica si el enemigo está huyendo.
    private bool isResting = false; // Indica si el enemigo está en periodo de descanso.
    private bool playerDetected = false; // Indica si el jugador ha sido detectado.

    /// <summary>
    /// Método Start: Llama al método base y comienza el ciclo de huida y descanso.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        StartCoroutine(FleeCycle());
    }

    /// <summary>
    /// Corrutina que maneja el ciclo de huida y descanso del enemigo.
    /// </summary>
    private IEnumerator FleeCycle()
    {
        while (true)
        {
            if (playerDetected)
            {
                isFleeing = true;
                yield return new WaitForSeconds(maxFleeDuration); // Huye por el tiempo máximo permitido.

                isFleeing = false;
                isResting = true;
                rb.linearVelocity = Vector3.zero; // Detiene el movimiento.
                yield return new WaitForSeconds(restDuration); // Descansa por el tiempo definido.

                isResting = false;
            }
            yield return null; // Espera hasta la siguiente iteración.
        }
    }

    /// <summary>
    /// Método FixedUpdate: Maneja la detección del jugador y la evasión.
    /// </summary>
    protected override void FixedUpdate()
    {
        // Si el jugador no está presente o el enemigo está descansando, no hace nada.
        if (player == null || isResting) return;

        CheckForPlayer(); // Comprueba si el jugador está dentro del rango de detección.

        // Si el jugador es detectado y el enemigo está en modo de huida.
        if (playerDetected && isFleeing)
        {
            Vector3 fleeDirection = (transform.position - player.position).normalized;
            fleeDirection.y = 0; // Mantiene el enemigo en el suelo.

            // Si el enemigo tiene SteeringBehaviors, usa su sistema para huir.
            if (steering != null)
            {
                steering.SetEnemyReference(player.gameObject);
            }
            else
            {
                rb.linearVelocity = fleeDirection * fleeSpeed;
            }
        }
        else
        {
            rb.linearVelocity = Vector3.zero; // Detiene el movimiento si no está huyendo.
        }

        StayOnGround(); // Asegura que el enemigo permanezca en el suelo.
    }

    /// <summary>
    /// Verifica si el jugador está dentro del rango de detección y si hay línea de visión directa.
    /// </summary>
    private void CheckForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Si el jugador está dentro del rango de detección.
        if (distanceToPlayer <= detectionRange)
        {
            RaycastHit hit;
            // Lanza un rayo en dirección al jugador para verificar si hay obstrucciones.
            if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, detectionRange))
            {
                if (hit.collider.CompareTag("Player")) // Si el rayo impacta directamente al jugador.
                {
                    playerDetected = true;
                    Debug.Log("👀 Enemigo detectó al jugador y comenzará a huir.");
                }
            }
        }
        else
        {
            playerDetected = false;
        }
    }

    /// <summary>
    /// Mantiene al enemigo en contacto con el suelo para evitar que flote.
    /// </summary>
    private void StayOnGround()
    {
        RaycastHit hit;
        // Lanza un rayo hacia abajo para detectar la posición exacta del suelo.
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, groundLayer))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
    }
}
