using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Controlador de patrullaje para un enemigo con NavMeshAgent.
/// Hace que el enemigo se mueva entre puntos de patrulla predefinidos.
/// </summary>
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrullaje")]
    public Transform[] patrolPoints; // 📌 Lista de puntos de patrulla.
    public float waitTime = 2f; // 📌 Tiempo de espera en cada punto antes de continuar.

    private NavMeshAgent agent; // 📌 Referencia al componente NavMeshAgent.
    private int currentPointIndex = 0; // 📌 Índice del punto actual de patrulla.
    private bool isPatrolling = true; // 📌 Controla si el enemigo está patrullando.

    /// <summary>
    /// Método Start: Se ejecuta al inicio del juego.
    /// Inicializa el NavMeshAgent y comienza la patrulla si hay puntos definidos.
    /// </summary>
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // 🔹 Obtiene el NavMeshAgent del enemigo.

        if (patrolPoints.Length > 0)
        {
            GoToNextPoint(); // 🔹 Inicia el patrullaje en el primer punto.
        }
        else
        {
            Debug.LogError("⚠️ No hay puntos de patrulla asignados en " + gameObject.name);
        }
    }

    /// <summary>
    /// Método Update: Se ejecuta en cada frame.
    /// Controla cuándo el enemigo llega a un punto de patrulla y espera antes de moverse al siguiente.
    /// </summary>
    private void Update()
    {
        // 🔹 Si el enemigo está patrullando y ha llegado a un punto de patrulla...
        if (isPatrolling && agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            StartCoroutine(WaitAtPoint()); // 🔹 Inicia la espera antes de moverse al siguiente punto.
        }
    }

    /// <summary>
    /// Corrutina que detiene la patrulla por un tiempo en cada punto.
    /// </summary>
    private IEnumerator WaitAtPoint()
    {
        isPatrolling = false; // 🔹 Pausa el patrullaje temporalmente.
        yield return new WaitForSeconds(waitTime); // ⏳ Espera el tiempo definido.
        GoToNextPoint(); // 🔹 Continúa al siguiente punto de patrulla.
    }

    /// <summary>
    /// Método para mover al enemigo al siguiente punto de patrulla.
    /// </summary>
    private void GoToNextPoint()
    {
        if (patrolPoints.Length == 0) return; // 🔹 Verifica que haya puntos de patrulla.

        agent.SetDestination(patrolPoints[currentPointIndex].position); // 📌 Define el destino del enemigo.
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length; // 📌 Avanza al siguiente punto en la lista.
        isPatrolling = true; // 🔹 Reanuda el patrullaje.
    }

    /// <summary>
    /// Método para detener la patrulla del enemigo.
    /// </summary>
    public void StopPatrolling()
    {
        isPatrolling = false; // 🔹 Detiene el patrullaje.
        agent.isStopped = true; // 📌 Detiene el movimiento del NavMeshAgent.
    }

    /// <summary>
    /// Método para reanudar la patrulla del enemigo.
    /// </summary>
    public void StartPatrolling()
    {
        isPatrolling = true; // 🔹 Activa el patrullaje.
        agent.isStopped = false; // 📌 Reactiva el NavMeshAgent.
        GoToNextPoint(); // 🔹 Envía al enemigo al siguiente punto de patrulla.
    }
}
