using UnityEngine;

/// <summary>
/// Controla el cono de visión de un enemigo, detectando si un objetivo está dentro de su rango de visión.
/// Se integra con el sistema de detección (Senses) y SteeringBehaviors para seguimiento.
/// </summary>
public class VisionCone : MonoBehaviour
{
    [Header("Configuración del Cono de Visión")]
    [Tooltip("Ángulo de visión en grados (hacia cada lado del frente del enemigo).")]
    public float viewAngle = 45f; 
    
    [Tooltip("Distancia máxima a la que el enemigo puede ver.")]
    public float viewDistance = 10f; 

    [Tooltip("Color del cono de visión cuando detecta un objetivo.")]
    public Color detectionColor = Color.red;

    [Tooltip("Color del cono de visión cuando no detecta nada.")]
    public Color defaultColor = Color.green;

    // 🔹 Referencias a otros componentes
    private Senses senses; // Sistema de detección general del enemigo
    private SteeringBehaviors steering; // Control de movimiento del enemigo
    private GameObject detectedEnemy = null; // Referencia al enemigo detectado en el cono de visión
    private Material visionMaterial; // Material usado para la representación visual del cono de visión

    /// <summary>
    /// Inicializa los componentes y configura el material del cono de visión.
    /// </summary>
    void Start()
    {
        senses = GetComponent<Senses>(); // Obtiene el sistema de detección
        steering = GetComponent<SteeringBehaviors>(); // Obtiene el comportamiento de movimiento

        // Configura el material del cono de visión
        visionMaterial = new Material(Shader.Find("Unlit/Color"));
        visionMaterial.color = defaultColor;
    }

    /// <summary>
    /// Actualiza la detección de enemigos en el cono de visión y ajusta su color.
    /// </summary>
    void Update()
    {
        detectedEnemy = GetTargetInVisionCone(); // Verifica si hay un enemigo en el cono de visión

        // Cambia el color del cono de visión si detecta un enemigo
        visionMaterial.color = detectedEnemy != null ? detectionColor : defaultColor;

        // Si detecta un enemigo, lo establece como objetivo en SteeringBehaviors
        if (detectedEnemy != null)
        {
            steering.SetEnemyReference(detectedEnemy);
        }
    }

    /// <summary>
    /// Determina si un objetivo está dentro del cono de visión del enemigo.
    /// </summary>
    /// <returns>El GameObject detectado si está dentro del cono de visión; null si no hay detección.</returns>
    GameObject GetTargetInVisionCone()
    {
        if (senses == null || !senses.IsEnemyDetected()) return null; // Si no hay detección, retorna null
        
        GameObject target = senses.GetDetectedEnemyRef();
        if (target == null) return null; // Si no hay objetivo, retorna null
        
        // Calcula la dirección hacia el objetivo detectado
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

        // Calcula el ángulo entre la dirección hacia el objetivo y la orientación actual del enemigo
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        // Retorna el objetivo si está dentro del ángulo de visión
        return angleToTarget < viewAngle ? target : null;
    }

    /// <summary>
    /// Dibuja el cono de visión en la escena para depuración.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = detectedEnemy != null ? detectionColor : defaultColor;

        // Calcula los límites izquierdo y derecho del cono de visión
        Vector3 leftLimit = Quaternion.Euler(0, -viewAngle, 0) * transform.forward * viewDistance;
        Vector3 rightLimit = Quaternion.Euler(0, viewAngle, 0) * transform.forward * viewDistance;

        // Dibuja las líneas del cono de visión
        Gizmos.DrawLine(transform.position, transform.position + leftLimit);
        Gizmos.DrawLine(transform.position, transform.position + rightLimit);

        // Dibuja la distancia máxima de detección
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}
