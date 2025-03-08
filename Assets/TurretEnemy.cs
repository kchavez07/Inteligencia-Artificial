using UnityEngine;
using System.Collections;

public class TurretEnemy : BaseEnemy
{
    [SerializeField] private float visionAngle = 360f; // Visión en todas direcciones.
    [SerializeField] private float visionDistance = 20f; // Distancia del cono de visión.
    [SerializeField] private float rotationSpeed = 30f; // Velocidad de rotación cuando busca.
    [SerializeField] private float detectCooldown = 3f; // Tiempo antes de volver a girar tras perder al jugador.
    [SerializeField] private GameObject bulletPrefab; // Prefab de la bala.
    [SerializeField] private Transform firePoint; // Lugar desde donde dispara.
    [SerializeField] private LayerMask playerLayer; // Capa del jugador para optimizar el Raycast.

    private bool isTracking = false; // Si está siguiendo al jugador.
    private Coroutine detectionCoroutine;

    protected override void FixedUpdate()
    {
        if (player == null) return;

        if (!isTracking)
        {
            // 🔄 Si no está siguiendo al jugador, rota en su lugar.
            transform.Rotate(Vector3.up * rotationSpeed * Time.fixedDeltaTime);
        }

        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ✅ Se eliminó la restricción del ángulo de visión (ahora es 360°).
        if (distanceToPlayer <= visionDistance)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, visionDistance, playerLayer))
            {
                // ✅ Se usa LayerMask en lugar de CompareTag() para optimizar la detección.
                if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
                {
                    if (!isTracking)
                    {
                        isTracking = true;
                        Debug.Log("🔫 Torreta detectó al jugador, deteniendo rotación y disparando...");
                        StopCoroutineIfExists(detectionCoroutine);
                        detectionCoroutine = StartCoroutine(ShootAtPlayer());
                    }
                }
            }
        }
        else if (isTracking)
        {
            // ✅ Cuando el jugador sale del área de visión, inicia un cooldown antes de dejar de disparar.
            StartCoroutine(StopShootingAfterCooldown());
        }
    }

    private IEnumerator ShootAtPlayer()
    {
        while (isTracking)
        {
            if (bulletPrefab != null && firePoint != null)
            {
                // ✅ La torreta ahora ajusta su dirección antes de disparar.
                firePoint.LookAt(player);

                // ✅ Se instancia la bala y se asegura de que viaje en la dirección correcta.
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

                if (bulletRb != null)
                {
                    bulletRb.linearVelocity = (player.position - firePoint.position).normalized * 20f; // 🔫 La bala sigue al jugador en tiempo real.
                }
            }
            yield return new WaitForSeconds(1f); // 🔫 Dispara cada segundo.
        }
    }

    private IEnumerator StopShootingAfterCooldown()
    {
        yield return new WaitForSeconds(detectCooldown);
        Debug.Log("🔄 Torreta perdió de vista al jugador. Dejando de disparar...");
        isTracking = false; // ✅ Se asegura de que la torreta deje de seguir al jugador.
        StopCoroutineIfExists(detectionCoroutine);
    }

    private void StopCoroutineIfExists(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionDistance); // 🔴 Dibuja el área de visión de la torreta.
    }
}
