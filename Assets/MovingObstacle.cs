using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Obstáculo dinámico que rota sobre su eje y/o se mueve en línea recta.
/// Puede utilizarse para crear obstáculos interactivos que interfieren con el movimiento del jugador o enemigos.
/// </summary>
public class MovingObstacle : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    
    /// <summary>Si el obstáculo debe rotar continuamente.</summary>
    public bool rotate = true;

    /// <summary>Velocidad de rotación en grados por segundo.</summary>
    public float rotationSpeed = 90f;

    /// <summary>Si el obstáculo debe moverse en línea recta.</summary>
    public bool move = false;

    /// <summary>Dirección del movimiento lineal.</summary>
    public Vector3 moveDirection = Vector3.forward;

    /// <summary>Distancia máxima que el obstáculo recorrerá desde su posición inicial.</summary>
    public float moveDistance = 5f;

    /// <summary>Velocidad a la que se mueve el obstáculo.</summary>
    public float moveSpeed = 2f;

    /// <summary>Posición inicial del obstáculo.</summary>
    private Vector3 startPosition;

    /// <summary>Indica si el obstáculo se está moviendo hacia adelante o regresando.</summary>
    private bool movingForward = true;

    /// <summary>
    /// Inicializa la posición de partida del obstáculo al inicio del juego.
    /// </summary>
    private void Start()
    {
        startPosition = transform.position;
    }

    /// <summary>
    /// Actualiza cada frame la lógica de rotación y movimiento del obstáculo.
    /// </summary>
    private void Update()
    {
        if (rotate)
        {
            // 🔁 Gira constantemente en el eje Y
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        if (move)
        {
            MoveObstacle();
        }
    }

    /// <summary>
    /// Mueve el obstáculo entre dos puntos (posición inicial y final) de manera cíclica.
    /// </summary>
    private void MoveObstacle()
    {
        float distance = Vector3.Distance(startPosition, transform.position);

        // Cambia de dirección si llegó al límite de la distancia
        if (movingForward && distance >= moveDistance)
        {
            movingForward = false;
        }
        else if (!movingForward && distance <= 0.1f)
        {
            movingForward = true;
        }

        // Calcula el punto de destino y mueve el obstáculo hacia él
        Vector3 targetPosition = movingForward ? startPosition + moveDirection * moveDistance : startPosition;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 🔹 Dibuja Gizmos en la vista de escena para visualizar la trayectoria del obstáculo.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f); // 🔴 Representa la posición actual del obstáculo

        if (move)
        {
            Vector3 start = startPosition == Vector3.zero ? transform.position : startPosition;
            Vector3 end = start + (moveDirection.normalized * moveDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(start, end); // 🔵 Traza la línea de movimiento
            Gizmos.DrawWireSphere(end, 0.5f); // 🔵 Marca el destino del movimiento
        }

        if (rotate)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Vector3.up * 1.5f); // 🟢 Indica rotación
        }
    }
}
