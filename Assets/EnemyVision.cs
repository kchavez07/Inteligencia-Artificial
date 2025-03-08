using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement; // 📌 Para reiniciar la escena.
using System.Collections;

/// <summary>
/// Controlador de visión del enemigo. Detecta al jugador, lo persigue y 
/// maneja su patrullaje cuando pierde de vista al jugador.
/// Además, permite la reaparición de enemigos destruidos y el reinicio de la escena.
/// </summary>
public class EnemyVision : MonoBehaviour
{
    [Header("Configuración de Visión")]
    public float visionRange = 15f; // 📌 Rango de visión del enemigo en unidades.
    public float visionAngle = 90f; // 📌 Ángulo del cono de visión del enemigo.
    public float chaseDuration = 3f; // 📌 Tiempo que el enemigo persigue al jugador antes de volver a patrullar.

    [Header("Referencias")]
    public Transform player; // 📌 Referencia al jugador.
    public LayerMask playerLayer; // 📌 Capa del jugador para la detección.
    public LayerMask obstaclesLayer; // 📌 Capa de obstáculos que bloquean la visión.

    private NavMeshAgent agent; // 📌 Componente NavMeshAgent para el movimiento del enemigo.
    private EnemyPatrol enemyPatrol; // 📌 Referencia al script de patrullaje del enemigo.
    private Vector3 lastKnownPosition; // 📌 Última posición conocida del jugador.
    private bool isChasing = false; // 📌 Indica si el enemigo está persiguiendo al jugador.
    private float lostSightTime; // 📌 Tiempo en el que el enemigo perdió de vista al jugador.

    // **🔹 Variables para la reaparición de enemigos**
    private static Vector3 respawnPosition; // 📌 Posición de reaparición del enemigo.
    private static GameObject lastDestroyedEnemy; // 📌 Último enemigo destruido.

    /// <summary>
    /// Método Start: Se ejecuta al iniciar el juego. 
    /// Inicializa componentes y establece la posición de reaparición.
    /// </summary>
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // 🔹 Obtiene el componente NavMeshAgent.
        enemyPatrol = GetComponent<EnemyPatrol>(); // 🔹 Obtiene el script de patrullaje del enemigo.

        // 🔹 Si no se ha asignado manualmente el jugador, lo busca en la escena.
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogError("⚠️ No se encontró al jugador. Asegúrate de que tiene la etiqueta 'Player'.");
            }
        }

        // **🔹 Guardamos la posición inicial para la reaparición.**
        respawnPosition = transform.position;
    }

    /// <summary>
    /// Método Update: Se ejecuta en cada frame. 
    /// Controla la detección del jugador y maneja la persecución o patrullaje del enemigo.
    /// </summary>
    private void Update()
    {
        // 📌 Si el enemigo ve al jugador, comienza la persecución.
        if (CanSeePlayer())
        {
            lastKnownPosition = player.position;
            if (!isChasing)
            {
                StartChasing();
            }
            lostSightTime = Time.time + chaseDuration;
        }
        // 📌 Si el enemigo está persiguiendo y pierde de vista al jugador, regresa a patrullar.
        else if (isChasing && Time.time >= lostSightTime)
        {
            ReturnToPatrol();
        }

        // **🔹 Si está en persecución, seguir actualizando la posición del jugador.**
        if (isChasing)
        {
            agent.SetDestination(player.position);
        }

        // **🔹 Si presionamos "R", reaparecemos el último enemigo destruido o reiniciamos la escena.**
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (lastDestroyedEnemy != null)
            {
                RespawnEnemy();
            }
            else
            {
                RestartScene();
            }
        }
    }

    /// <summary>
    /// Verifica si el jugador está dentro del campo de visión del enemigo.
    /// </summary>
    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 📌 Verifica si el jugador está dentro del cono de visión y sin obstáculos de por medio.
        if (distanceToPlayer <= visionRange && angleToPlayer <= visionAngle / 2)
        {
            if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstaclesLayer))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Inicia la persecución del jugador.
    /// </summary>
    private void StartChasing()
    {
        isChasing = true;
        enemyPatrol?.StopPatrolling(); // 🔹 Detiene el patrullaje si el enemigo tiene ese script.
        agent.isStopped = false; // 🔹 Activa el NavMeshAgent.
        agent.SetDestination(player.position); // 📌 Asigna al jugador como el objetivo.
        Debug.Log("🔥 Iniciando persecución...");
    }

    /// <summary>
    /// Detiene la persecución y regresa a patrullar.
    /// </summary>
    private void ReturnToPatrol()
    {
        isChasing = false;
        enemyPatrol?.StartPatrolling(); // 🔹 Reactiva el patrullaje si el enemigo lo tiene.
        agent.isStopped = false; // 🔹 Permite que el NavMeshAgent se mueva nuevamente.
        Debug.Log("🔵 Volviendo a patrullar...");
    }

    /// <summary>
    /// Detecta cuando el enemigo colisiona con otro enemigo y lo destruye.
    /// Guarda una referencia al último enemigo destruido.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("⚠ Enemigo destruido al chocar con otro enemigo.");
            lastDestroyedEnemy = other.gameObject; // 📌 Guarda el último enemigo destruido.
            Destroy(other.gameObject); // 🔥 Elimina al enemigo que colisionó.
        }
    }

    /// <summary>
    /// Reaparece el último enemigo destruido en su posición original.
    /// </summary>
    private void RespawnEnemy()
    {
        if (lastDestroyedEnemy != null)
        {
            Instantiate(lastDestroyedEnemy, respawnPosition, Quaternion.identity); // 📌 Crea un nuevo enemigo en la posición guardada.
            lastDestroyedEnemy = null; // 📌 Resetea la variable después de reaparecer al enemigo.
            Debug.Log("🟢 Enemigo reaparecido.");
        }
    }

    /// <summary>
    /// Reinicia la escena actual cuando se presiona "R" y no hay enemigo destruido para reaparecer.
    /// </summary>
    private void RestartScene()
    {
        Debug.Log("🔄 Reiniciando la escena...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 📌 Recarga la escena actual.
    }
}
