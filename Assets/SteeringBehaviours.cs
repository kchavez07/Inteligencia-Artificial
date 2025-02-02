using TMPro;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    protected Vector3 currentVelocity = Vector3.zero;
    [SerializeField] protected float maxVelocity = 10.0f;
    [SerializeField] protected float maxForce = 2.0f;
    protected GameObject ReferenciaObjetivo;
    protected Rigidbody targetRB;
    [SerializeField] protected Rigidbody rb;

    void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("No Rigidbody found on " + gameObject.name + ". Please add a Rigidbody.");
            }
        }
    }

    public void SetEnemyReference(GameObject enemyRef)
    {
        ReferenciaObjetivo = enemyRef;
        if (ReferenciaObjetivo != null)
        {
            targetRB = ReferenciaObjetivo.GetComponent<Rigidbody>();
        }
    }

    protected Vector3 Seek(Vector3 targetPosition)
    {
        if (rb == null) return Vector3.zero;
        
        Vector3 desiredDirection = (targetPosition - transform.position).normalized;
        Vector3 desiredVelocity = desiredDirection * maxVelocity;
        Vector3 steeringForce = desiredVelocity - rb.linearVelocity;
        return Vector3.ClampMagnitude(steeringForce, maxForce);
    }

    Vector3 Pursuit(Vector3 targetPosition, Vector3 targetCurrentVelocity)
    {
        float LookAheadTime = (transform.position - targetPosition).magnitude / maxVelocity;
        Vector3 predictedPosition = targetPosition + targetCurrentVelocity * LookAheadTime;
        return Seek(predictedPosition);
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody not assigned to " + gameObject.name);
            return;
        }

        Vector3 steeringForce = Vector3.zero;

        if (ReferenciaObjetivo != null)
        {
            if (targetRB != null)
            {
                steeringForce = Pursuit(ReferenciaObjetivo.transform.position, targetRB.linearVelocity);
            }
            else
            {
                steeringForce = Seek(ReferenciaObjetivo.transform.position);
            }
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }

        rb.AddForce(steeringForce, ForceMode.Acceleration);
    }

    private void OnDrawGizmos()
    {
        if (ReferenciaObjetivo != null)
        {
            Vector3 predictedPosition = ReferenciaObjetivo.transform.position;
            if (targetRB != null)
            {
                float LookAheadTime = (transform.position - predictedPosition).magnitude / maxVelocity;
                predictedPosition += targetRB.linearVelocity * LookAheadTime;
            }
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(predictedPosition, Vector3.one);
            Gizmos.DrawLine(transform.position, predictedPosition);
        }
    }
}
