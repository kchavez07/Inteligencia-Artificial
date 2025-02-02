using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public float viewAngle = 45f; // Ángulo de visión
    public float viewDistance = 10f; // Distancia de visión
    public Color detectionColor = Color.red;
    public Color defaultColor = Color.green;
    private Senses senses;
    private SteeringBehaviors steering;
    private GameObject detectedEnemy = null;
    private Material visionMaterial;

    void Start()
    {
        senses = GetComponent<Senses>();
        steering = GetComponent<SteeringBehaviors>();
        visionMaterial = new Material(Shader.Find("Unlit/Color"));
        visionMaterial.color = defaultColor;
    }

    void Update()
    {
        detectedEnemy = GetTargetInVisionCone();
        visionMaterial.color = detectedEnemy != null ? detectionColor : defaultColor;
        
        if (detectedEnemy != null)
        {
            steering.SetEnemyReference(detectedEnemy);
        }
    }

    GameObject GetTargetInVisionCone()
    {
        if (senses == null || !senses.IsEnemyDetected()) return null;
        
        GameObject target = senses.GetDetectedEnemyRef();
        if (target == null) return null;
        
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        
        return angleToTarget < viewAngle ? target : null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = detectedEnemy != null ? detectionColor : defaultColor;
        Vector3 leftLimit = Quaternion.Euler(0, -viewAngle, 0) * transform.forward * viewDistance;
        Vector3 rightLimit = Quaternion.Euler(0, viewAngle, 0) * transform.forward * viewDistance;
        
        Gizmos.DrawLine(transform.position, transform.position + leftLimit);
        Gizmos.DrawLine(transform.position, transform.position + rightLimit);
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}
