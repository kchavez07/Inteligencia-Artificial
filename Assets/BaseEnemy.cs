using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    protected float currentHP;
    [SerializeField] protected float maxHP = 10;
    [SerializeField] protected int attackDamage = 1;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float detectionRange = 50f;

    protected Rigidbody rb;
    protected Transform player;
    protected SteeringBehaviors steering; // 🔹 Integración con SteeringBehaviors

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        currentHP = maxHP;
        steering = GetComponent<SteeringBehaviors>(); // 🔹 Verifica si este enemigo usa SteeringBehaviors

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("❌ No se encontró el jugador. Asegúrate de que el jugador tiene la etiqueta 'Player'.");
        }
    }

    protected virtual void FixedUpdate()
    {
        if (player == null) return;

        // 🔹 Si tiene SteeringBehaviors, deja que ese script maneje el movimiento
        if (steering != null)
        {
            steering.SetEnemyReference(player.gameObject); // Asegura que siga al jugador
            return;
        }

        // 🔹 Si NO tiene SteeringBehaviors, usa el movimiento normal
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
