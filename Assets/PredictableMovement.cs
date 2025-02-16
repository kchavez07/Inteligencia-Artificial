using UnityEngine;

public class PredictableMovement : SteeringBehaviors
{
    // Lista de waypoints que el agente visitará en orden
    [SerializeField]
    GameObject[] waypoints;

    // Índice del waypoint actual al que se dirige
    private int currentTargetWaypoint = 0;
    
    // Radio de aceptación para determinar si el agente ha llegado al waypoint
    [SerializeField]
    private float acceptanceRadius = 3;

    // Inicialización del script
    void Start()
    {
        // No se realiza ninguna acción en Start, ya que el comportamiento es gestionado en FixedUpdate
    }

    // Método llamado en cada frame de actualización de físicas
    void FixedUpdate()
    {
        // Verifica si el agente ha llegado al waypoint actual
        if( (transform.position - waypoints[currentTargetWaypoint].transform.position).magnitude < 
            acceptanceRadius)
        {
            // Cambia al siguiente waypoint en la secuencia
            currentTargetWaypoint++;
            
            // Si se llegó al final de la lista, vuelve al primer waypoint
            if (currentTargetWaypoint >= waypoints.Length)
            {
                currentTargetWaypoint = 0;
            }
        }

        // Calcula la fuerza de dirección hacia el waypoint actual
        Vector3 steeringForce = Seek(waypoints[currentTargetWaypoint].transform.position);

        // Aplica la fuerza de dirección al Rigidbody para mover el agente
        rb.AddForce(steeringForce, ForceMode.Acceleration);
    }

    // Dibuja un gizmo para visualizar el radio de aceptación de los waypoints en el editor
    void OnDrawGizmos()
    {
        if (waypoints.Length > 0)
        {
            Gizmos.DrawWireSphere(waypoints[currentTargetWaypoint].transform.position, acceptanceRadius);
        }
    }
}
