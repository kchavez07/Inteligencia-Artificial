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
    public float acceleration = 10f; // 🔹 Aceleración del NavMeshAgent
    public float maxSpeed = 3.5f;    // 🔹 Velocidad máxima del agente
    public float fleeDistance = 7f;  // 🔹 Qué tan lejos intenta escapar del jugador

    [Header("Estados")]
    public float activeDuration = 10f; // 🔹 Cuánto dura el estado activo (huye)
    public float tiredDuration = 5f;   // 🔹 Cuánto dura el estado cansado (se detiene)
    private bool isTired = false;      // 🔹 Indica si está en modo "cansado" o no

    [Header("Disparo")]
    public GameObject bulletPrefab;   // 🔫 Prefab de la bala que va a disparar
    public Transform firePoint;       // 🔫 Punto desde donde se disparan las balas
    public float bulletSpeed = 10f;   // 🔫 Velocidad de la bala
    public float normalShootCooldown = 2f;   // 🔫 Tiempo entre disparos en modo normal
    public float tiredShootCooldown = 3.5f;  // 🔫 Tiempo entre disparos en modo cansado
    private float lastShootTime = 0f;        // 🔫 Cuándo fue el último disparo

    [Header("Visual")]
    public Renderer enemyRenderer;     // 🎨 Renderer para cambiar el color del enemigo
    public Color activeColor = Color.white; // 🎨 Color en modo activo
    public Color tiredColor = Color.red;    // 🎨 Color en modo cansado

    [Header("Lógica de Visión")]
    public LayerMask visionMask; // 👁️ Capas que bloquean la visión (paredes, etc.)

    private NavMeshAgent agent;       // 👟 Referencia al agente de navegación
    private Vector3 fleePosition;     // 🏃‍♂️ Posición a la que intenta escapar

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

    protected override void FixedUpdate()
    {
        // 🚫 No usamos el movimiento heredado del BaseEnemy porque usamos NavMesh
    }

    private void Update()
    {
        if (isTired)
        {
            ShootAtPlayer(); // 🔫 Solo dispara cuando está cansado
        }
    }

    /// <summary>
    /// 🔁 Estado activo: intenta huir del jugador o acercarse si no lo ve.
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
            if (player == null)
            {
                Debug.LogWarning("⚠️ No se encontró al jugador.");
                yield break;
            }

            bool hasLineOfSight = HasLineOfSightToPlayer();

            if (!hasLineOfSight)
            {
                // 👀 Si no lo ve, se acerca
                agent.isStopped = false;
                agent.SetDestination(player.position);
                Debug.DrawLine(transform.position, player.position, Color.yellow);
                Debug.Log("👁️ No veo al jugador, me acerco.");
            }
            else
            {
                // 🏃‍♂️ Si lo ve, intenta huir en dirección opuesta
                Vector3 fleeDirection = (transform.position - player.position).normalized;
                fleePosition = transform.position + (fleeDirection * fleeDistance);

                if (!TryFindEscapePoint(fleePosition, out Vector3 validFleePosition))
                {
                    // 🔄 Si falla la dirección inicial, intenta la contraria
                    Vector3 reverseFlee = transform.position - (fleeDirection * fleeDistance);
                    TryFindEscapePoint(reverseFlee, out validFleePosition);
                }

                agent.isStopped = false;
                agent.SetDestination(validFleePosition);
                Debug.DrawLine(transform.position, validFleePosition, Color.cyan);
                Debug.Log("🏃‍♂️ Intentando escapar hacia: " + validFleePosition);
            }

            yield return new WaitForSeconds(activeDuration);
            StartCoroutine(TiredState());
            break;
        }
    }

    /// <summary>
    /// 💤 Estado cansado: se detiene y dispara al jugador.
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
    /// 🔫 Dispara una bala hacia el jugador si se cumplen condiciones.
    /// </summary>
    private void ShootAtPlayer()
    {
        if (player == null || bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("⚠️ No se puede disparar: faltan referencias.");
            return;
        }

        float cooldown = isTired ? tiredShootCooldown : normalShootCooldown;

        if (Time.time >= lastShootTime + cooldown)
        {
            lastShootTime = Time.time;

            firePoint.LookAt(player); // 👁️ Apunta al jugador

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * bulletSpeed; // 🚀 Dispara hacia adelante
                Debug.Log($"🟡 Bala disparada desde {firePoint.position} con dirección {firePoint.forward}");
            }
            else
            {
                Debug.LogWarning("⚠️ La bala no tiene Rigidbody.");
            }
        }
    }

    /// <summary>
    /// 🔍 Intenta encontrar una posición válida sobre el NavMesh.
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
    /// 👁️ Verifica si el enemigo puede ver al jugador con Raycast y LayerMask.
    /// </summary>
    private bool HasLineOfSightToPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, visionMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Debug.DrawRay(transform.position, direction * distance, Color.green);
                return true;
            }
        }

        Debug.DrawRay(transform.position, direction * distance, Color.red);
        return false;
    }

    /// <summary>
    /// 🎯 Dibuja una esfera y línea visual para debug del FirePoint.
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
