using UnityEngine;
using System.Collections;

/// <summary>
/// Enemigo tipo torreta que dispara cuando el jugador entra en su rango de detección.
/// Hereda de <see cref="BaseEnemy"/>.
/// </summary>
public class TurretEnemy : BaseEnemy
{
    [SerializeField] private float visionAngle = 360f; // **Ángulo de visión (Círculo completo).**
    [SerializeField] private float visionDistance = 45f; // **Distancia máxima de detección del jugador.**
    [SerializeField] private float detectCooldown = 3f; // **Tiempo antes de dejar de disparar tras perder al jugador.**
    [SerializeField] private GameObject bulletPrefab; // **Prefab de la bala que dispara la torreta.**
    [SerializeField] private Transform firePoint; // **Punto desde donde se disparan las balas.**
    [SerializeField] private LayerMask playerLayer; // **Capa del jugador para optimizar el Raycast.**

    private bool isTracking = false; // **Indica si la torreta está disparando al jugador.**
    private Coroutine detectionCoroutine; // **Referencia a la corrutina de disparo.**

    /// <summary>
    /// Método FixedUpdate: Detecta si el jugador está en su rango.
    /// </summary>
    protected override void FixedUpdate()
    {
        if (player == null) return; // **Si no hay jugador, no hace nada.**
        
        CheckForPlayer(); // **Verifica si el jugador está en su rango de disparo.**
    }

    /// <summary>
    /// Comprueba si el jugador está dentro del rango de detección.
    /// </summary>
    private void CheckForPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // **Si el jugador está dentro del rango y en el círculo de visión, empieza a disparar.**
        if (distanceToPlayer <= visionDistance && angleToPlayer <= visionAngle / 2)
        {
            if (!isTracking)
            {
                isTracking = true;
                StopAllCoroutines(); // **Detiene cualquier cooldown previo.**
                detectionCoroutine = StartCoroutine(ShootAtPlayer()); // **Empieza a disparar.**
            }
        }
        else if (isTracking)
        {
            // **Cuando el jugador sale del rango, inicia cooldown antes de dejar de disparar.**
            isTracking = false;
            StartCoroutine(StopShootingAfterCooldown());
        }
    }

    /// <summary>
    /// Corrutina que hace que la torreta dispare continuamente mientras el jugador esté en rango.
    /// </summary>
    private IEnumerator ShootAtPlayer()
    {
        while (isTracking)
        {
            if (bulletPrefab != null && firePoint != null)
            {
                // **Instancia la bala en el punto de disparo.**
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

                if (bulletRb != null)
                {
                    bulletRb.linearVelocity = firePoint.forward * 20f; // **Dispara en la dirección del FirePoint.**
                }
            }
            yield return new WaitForSeconds(1f); // **🔫 Dispara cada segundo.**
        }
    }

    /// <summary>
    /// Corrutina que espera antes de detener el disparo tras perder al jugador.
    /// </summary>
    private IEnumerator StopShootingAfterCooldown()
    {
        yield return new WaitForSeconds(detectCooldown);
        Debug.Log("🔄 Torreta perdió de vista al jugador. Dejando de disparar...");
    }

    /// <summary>
    /// Método OnDrawGizmos: Dibuja el círculo rojo en la escena para depuración.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionDistance); // **Dibuja el círculo de visión.**
    }
}
