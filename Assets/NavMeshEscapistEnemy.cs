using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Enemigo que usa NavMesh para escapar del jugador.
/// Alterna entre un estado activo (huye) y un estado cansado (se detiene y dispara).
/// </summary>
public class NavMeshEscapistEnemy : BaseEnemy
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Aceleración del NavMeshAgent.")]
    public float acceleration = 10f;

    [Tooltip("Velocidad máxima del agente.")]
    public float maxSpeed = 3.5f;

    [Tooltip("Distancia mínima para intentar escapar del jugador.")]
    public float fleeDistance = 7f;

    [Header("Estados")]
    [Tooltip("Duración del estado activo (huida).")]
    public float activeDuration = 10f;

    [Tooltip("Duración del estado cansado (disparo).")]
    public float tiredDuration = 5f;

    private bool isTired = false;

    [Header("Disparo")]
    [Tooltip("Prefab de la bala a instanciar.")]
    public GameObject bulletPrefab;

    [Tooltip("Punto desde donde se instancian las balas.")]
    public Transform firePoint;

    [Tooltip("Velocidad de la bala disparada.")]
    public float bulletSpeed = 10f;

    [Tooltip("Cooldown entre disparos en estado normal.")]
    public float normalShootCooldown = 2f;

    [Tooltip("Cooldown entre disparos en estado cansado.")]
    public float tiredShootCooldown = 3.5f;

    private float lastShootTime = 0f;

    [Header("Visual")]
    [Tooltip("Renderer para cambiar el color del enemigo.")]
    public Renderer enemyRenderer;

    [Tooltip("Color del enemigo cuando está activo.")]
    public Color activeColor = Color.white;

    [Tooltip("Color del enemigo cuando está cansado.")]
    public Color tiredColor = Color.red;

    private NavMeshAgent agent;
    private Vector3 fleePosition;

    /// <summary>
    /// Inicializa referencias y comienza el estado activo.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = acceleration;
        agent.speed = maxSpeed;
        agent.autoBraking = false;
        agent.updateRotation = true;

        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = activeColor;
        }

        StartCoroutine(ActiveState());
    }

    /// <summary>
    /// Se sobreescribe para ignorar el movimiento heredado de BaseEnemy.
    /// </summary>
    protected override void FixedUpdate()
    {
        // Se ignora la lógica de movimiento del BaseEnemy
    }

    /// <summary>
    /// En estado cansado, permite disparar al jugador.
    /// </summary>
    private void Update()
    {
        if (isTired)
        {
            ShootAtPlayer();
        }
    }

    /// <summary>
    /// Comportamiento del estado activo: el enemigo huye del jugador.
    /// </summary>
    private IEnumerator ActiveState()
    {
        isTired = false;

        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = activeColor;
        }

        Debug.Log("🟢 Estado Activo: Intentando escapar del jugador.");

        while (!isTired)
        {
            if (player != null)
            {
                // Calcula la dirección opuesta al jugador
                Vector3 fleeDirection = (transform.position - player.position).normalized;
                fleePosition = transform.position + (fleeDirection * fleeDistance);

                // Intenta encontrar un punto válido en el NavMesh para huir
                if (TryFindEscapePoint(fleePosition, out Vector3 validFleePosition))
                {
                    agent.isStopped = false;
                    agent.SetDestination(validFleePosition);
                    Debug.DrawLine(transform.position, validFleePosition, Color.blue, 2f);
                }
            }

            yield return new WaitForSeconds(activeDuration);
            StartCoroutine(TiredState());
            yield break;
        }
    }

    /// <summary>
    /// Comportamiento del estado cansado: el enemigo se detiene y dispara.
    /// </summary>
    private IEnumerator TiredState()
    {
        isTired = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = tiredColor;
        }

        Debug.Log("🔴 Estado Cansado: Detenido y disparando al jugador.");

        yield return new WaitForSeconds(tiredDuration);

        agent.isStopped = false;
        StartCoroutine(ActiveState());
    }

    /// <summary>
    /// Dispara una bala hacia el jugador si se cumplen las condiciones.
    /// </summary>
    private void ShootAtPlayer()
    {
        if (player == null || bulletPrefab == null || firePoint == null)
            return;

        float cooldown = isTired ? tiredShootCooldown : normalShootCooldown;

        if (Time.time >= lastShootTime + cooldown)
        {
            lastShootTime = Time.time;

            // Apunta al jugador
            firePoint.LookAt(player);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * bulletSpeed;
                Debug.Log($"🟡 Bala disparada desde {firePoint.position} con dirección {firePoint.forward}");
            }
            else
            {
                Debug.LogWarning("⚠️ La bala no tiene Rigidbody.");
            }
        }
    }

    /// <summary>
    /// Intenta encontrar un punto válido del NavMesh cerca de la posición deseada.
    /// </summary>
    private bool TryFindEscapePoint(Vector3 targetPosition, out Vector3 validPosition)
    {
        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            validPosition = hit.position;
            return true;
        }

        validPosition = transform.position;
        return false;
    }

    /// <summary>
    /// Dibuja gizmos para el FirePoint (visual en escena).
    /// </summary>
    private void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(firePoint.position, 0.1f);
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 2);
        }
    }
}
