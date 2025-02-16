using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Clase que implementa diferentes comportamientos de Steering para el movimiento de un agente en Unity.
/// Permite al enemigo buscar (`Seek`), huir (`Flee`), predecir el movimiento del objetivo (`Pursuit` y `Evade`) 
/// y evitar obstáculos (`ObstacleAvoidance`).
/// </summary>
public class SteeringBehaviors : MonoBehaviour
{
    /// <summary>
    /// Enum que define las acciones de Steering que puede realizar el enemigo.
    /// </summary>
    public enum SteeringAction
    {
        Approach,  // Aproximación: incluye Seek y Pursuit.
        Escape     // Evasión: incluye Flee y Evade.
    }

    public SteeringAction currentSteeringAction = SteeringAction.Approach; // Acción actual del enemigo.

    // Variables de movimiento
    protected Vector3 currentVelocity = Vector3.zero; // Velocidad actual del agente.

    [SerializeField]
    protected float maxVelocity = 10.0f; // Velocidad máxima permitida.

    [SerializeField]
    protected float maxForce = 2.0f; // Fuerza máxima que se puede aplicar al agente.

    // Variables de referencia al objetivo
    protected GameObject ReferenciaObjetivo; // Referencia al objetivo a seguir o evadir.
    protected Rigidbody targetRB; // Rigidbody del objetivo (si tiene uno).

    public List<GameObject> obstacleList = new List<GameObject>(); // Lista de obstáculos detectados.

    public float RepelRadius = 3; // Radio de repelencia para evitar obstáculos.
    public float MaxRepelForce = 3; // Fuerza máxima de repulsión aplicada al enemigo.

    [SerializeField]
    protected Rigidbody rb; // Rigidbody del enemigo para manejar su movimiento.

    /// <summary>
    /// Asigna un nuevo objetivo al enemigo.
    /// </summary>
    /// <param name="enemyRef">Objeto a seguir o evadir.</param>
    public void SetEnemyReference(GameObject enemyRef)
    { 
        ReferenciaObjetivo = enemyRef;

        // Verifica si el objetivo tiene un Rigidbody.
        if (ReferenciaObjetivo != null)
        { 
            targetRB = ReferenciaObjetivo.GetComponent<Rigidbody>();
            if (targetRB == null)
            {
                Debug.Log("El enemigo referenciado actualmente no tiene Rigidbody. ¿Así debería ser?");
            }
        }
        else
        {
            targetRB = null;
        }
    }

    void Start()
    {
        // Inicialización del enemigo. (Actualmente vacío).
    }

    /// <summary>
    /// Calcula la fuerza de dirección hacia el objetivo (Seek).
    /// </summary>
    protected Vector3 Seek(Vector3 targetPosition)
    {
        Vector3 desiredDirection = (targetPosition - transform.position).normalized;
        Vector3 desiredVelocity = desiredDirection * maxVelocity;

        return desiredVelocity - rb.linearVelocity;
    }

    /// <summary>
    /// Calcula la fuerza de dirección opuesta al objetivo (Flee).
    /// </summary>
    protected Vector3 Flee(Vector3 targetPosition)
    {
        return -Seek(targetPosition);
    }

    /// <summary>
    /// Persigue un objetivo prediciendo su movimiento (Pursuit).
    /// </summary>
    protected Vector3 Pursuit(Vector3 targetPosition, Vector3 targetCurrentVelocity)
    {
        float LookAheadTime = (transform.position - targetPosition).magnitude / maxVelocity;
        Vector3 predictedPosition = targetPosition + targetCurrentVelocity * LookAheadTime;
        return Seek(predictedPosition);
    }

    /// <summary>
    /// Evade un objetivo prediciendo su movimiento (Evade).
    /// </summary>
    protected Vector3 Evade(Vector3 targetPosition, Vector3 targetCurrentVelocity)
    {
        float LookAheadTime = (transform.position - targetPosition).magnitude / maxVelocity;
        Vector3 predictedPosition = targetPosition + targetCurrentVelocity * LookAheadTime;
        return -Seek(predictedPosition);
    }

    /// <summary>
    /// Calcula una fuerza para evitar obstáculos cercanos.
    /// </summary>
    protected Vector3 ObstacleAvoidance(Vector3 obstaclePosition, float RepelRadius, float MaxRepelForce)
    {
        Vector3 outVector = Vector3.zero;
        Vector3 PuntaMenosCola = transform.position - obstaclePosition;
        Vector3 DireccionPuntaMenosCola = PuntaMenosCola.normalized;
        float distance = PuntaMenosCola.magnitude;

        if (distance - RepelRadius >= 0)
            return outVector;

        float intersectionDistance = RepelRadius - distance;
        float intersectionPercentage = intersectionDistance / RepelRadius;

        outVector = DireccionPuntaMenosCola * intersectionPercentage * MaxRepelForce;
        return outVector;
    }

    /// <summary>
    /// Método FixedUpdate: Se ejecuta en cada frame de física y aplica las fuerzas de Steering al enemigo.
    /// </summary>
    void FixedUpdate()
    {
        Vector3 steeringForce = Vector3.zero; // Inicializa la fuerza de dirección en cero.

        // Verifica si hay un objetivo a seguir o evadir.
        if (ReferenciaObjetivo != null)
        {
            // Si el objetivo tiene un Rigidbody, usa Pursuit o Evade según la acción actual.
            if (targetRB != null)
            {
                switch (currentSteeringAction)
                {
                    case SteeringAction.Approach:
                        steeringForce = Pursuit(ReferenciaObjetivo.transform.position, targetRB.linearVelocity);
                        break;
                    case SteeringAction.Escape:
                        steeringForce = Evade(ReferenciaObjetivo.transform.position, targetRB.linearVelocity);
                        break;
                }
            }
            // Si el objetivo no tiene un Rigidbody, usa Seek o Flee.
            else
            {
                switch (currentSteeringAction)
                {
                    case SteeringAction.Approach:
                        steeringForce = Seek(ReferenciaObjetivo.transform.position);
                        break;
                    case SteeringAction.Escape:
                        steeringForce = Flee(ReferenciaObjetivo.transform.position);
                        break;
                }
            }

            // Evita obstáculos en la lista.
            foreach (var obstacle in obstacleList)
            {
                steeringForce += ObstacleAvoidance(obstacle.transform.position, RepelRadius, MaxRepelForce);
            }

            // Limita la magnitud de la fuerza de Steering para evitar movimientos excesivos.
            steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);

            // Aplica la fuerza calculada al Rigidbody usando aceleración.
            rb.AddForce(steeringForce, ForceMode.Acceleration);

            // Limita la velocidad del enemigo para que no supere su velocidad máxima.
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxVelocity);

            // Debugging: Si la velocidad excede el máximo, muestra un warning en la consola.
            if (rb.linearVelocity.magnitude > maxVelocity)
                Debug.LogWarning(rb.linearVelocity);
        }
    }

    /// <summary>
    /// Método OnDrawGizmos: Dibuja indicadores visuales en la escena para depuración.
    /// </summary>
    private void OnDrawGizmos()
    {
        Vector3 targetPosition = Vector3.zero;
        Vector3 targetCurrentVelocity = Vector3.zero;

        // Si hay un objetivo, obtiene su posición y velocidad (si tiene un Rigidbody).
        if (ReferenciaObjetivo != null)
        {
            targetPosition = ReferenciaObjetivo.transform.position;
            if (targetRB != null)
            { 
                targetCurrentVelocity = targetRB.linearVelocity; 
            }
        }

        // Si el objetivo tiene un Rigidbody, dibuja la predicción de su posición futura.
        if (targetRB != null)  
        {
            Debug.Log(targetRB.gameObject.name);

            if (ReferenciaObjetivo == null)
            {
                Debug.LogError("ReferenciaObjetivo es null.");
            }

            float LookAheadTime = (transform.position - targetPosition).magnitude / maxVelocity;
            Vector3 predictedPosition = targetPosition + targetCurrentVelocity * LookAheadTime;

            Gizmos.DrawCube(predictedPosition, Vector3.one);
            Gizmos.DrawLine(transform.position, predictedPosition);
        }
        // Si solo hay un objetivo sin Rigidbody, dibuja una línea directa hacia él.
        else if (ReferenciaObjetivo != null)
        {
            Gizmos.DrawLine(transform.position, ReferenciaObjetivo.transform.position);
        }
    }
}
