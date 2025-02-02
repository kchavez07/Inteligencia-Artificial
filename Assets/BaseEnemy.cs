using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField]
    protected int currentHP;

    [SerializeField]
    protected int maxHP;

    // Radio de detección 
    [SerializeField]
    protected Senses detectionSenses;

    // Velocidad de movimiento
    [SerializeField]
    protected SteeringBehaviors steeringBehaviors;

    [SerializeField]
    protected int attackDamage;

    void Awake()
    {
        // Verifica si `detectionSenses` y `steeringBehaviors` están asignados
        if (detectionSenses == null)
        {
            detectionSenses = GetComponent<Senses>();
            if (detectionSenses == null)
            {
                Debug.LogError("Senses no asignado en " + gameObject.name);
            }
        }

        if (steeringBehaviors == null)
        {
            steeringBehaviors = GetComponent<SteeringBehaviors>();
            if (steeringBehaviors == null)
            {
                Debug.LogError("SteeringBehaviors no asignado en " + gameObject.name);
            }
        }
    }

    private void FixedUpdate()
    {
        if (detectionSenses != null && steeringBehaviors != null)
        {
            GameObject detectedEnemy = detectionSenses.GetDetectedEnemyRef();
            if (detectedEnemy != null)
            {
                steeringBehaviors.SetEnemyReference(detectedEnemy); // Ahora usa la instancia correcta
            }
        }
    }
}
