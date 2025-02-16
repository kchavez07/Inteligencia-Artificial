using UnityEngine;
using System.Collections;

public class TurretEnemy : BaseEnemy
{
    [SerializeField] private float visionAngle = 45f; // Ángulo del cono de visión
    [SerializeField] private float visionDistance = 10f; // Distancia del cono de visión
    [SerializeField] private float rotationSpeed = 30f; // Velocidad de rotación cuando busca
    [SerializeField] private float detectCooldown = 3f; // Tiempo antes de volver a girar tras perder al jugador
    [SerializeField] private GameObject bulletPrefab; // Prefab de la bala
    [SerializeField] private Transform firePoint; // Lugar desde donde dispara

    private bool isTracking = false; // Si está siguiendo al jugador
    private Coroutine detectionCoroutine;

    protected override void FixedUpdate()
    {
        if (player == null) return;

        if (!isTracking)
        {
            // Si no está siguiendo al jugador, rota en su lugar
            transform.Rotate(Vector3.up * rotationSpeed * Time.fixedDeltaTime);
        }

        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= visionDistance && angleToPlayer <= visionAngle / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, visionDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    if (!isTracking)
                    {
                        isTracking = true;
                        Debug.Log("🔫 Torreta detectó al jugador, deteniendo rotación y disparando...");
                        StopAllCoroutines();
                        detectionCoroutine = StartCoroutine(ShootAtPlayer());
                    }
                }
            }
        }
        else if (isTracking)
        {
            // Si el jugador sale de su visión, inicia el cooldown antes de volver a rotar
            isTracking = false;
            StartCoroutine(ResetTurret());
        }
    }

    private IEnumerator ShootAtPlayer()
    {
        while (isTracking)
        {
            if (bulletPrefab != null && firePoint != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

                if (bulletRb != null)
                {
                    Vector3 direction = (player.position - firePoint.position).normalized;
                    bulletRb.linearVelocity = direction * 20f; // Velocidad de la bala
                }
            }
            yield return new WaitForSeconds(1f); // 🔫 Dispara cada segundo
        }
    }

    private IEnumerator ResetTurret()
    {
        yield return new WaitForSeconds(detectCooldown);
        Debug.Log("🔄 Torreta no detecta al jugador, reanudando rotación...");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionDistance);

        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * visionDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * visionDistance);
    }
}
