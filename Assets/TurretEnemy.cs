using UnityEngine;
using System.Collections;

/// <summary>
/// Enemigo tipo torreta que gira en busca del jugador y dispara cuando lo detecta.
/// Hereda de <see cref="BaseEnemy"/>.
/// </summary>
public class TurretEnemy : BaseEnemy
{
    [SerializeField] private float visionAngle = 45f; // Ángulo del cono de visión.
    [SerializeField] private float visionDistance = 10f; // Distancia máxima de detección del jugador.
    [SerializeField] private float rotationSpeed = 30f; // Velocidad de rotación cuando busca al jugador.
    [SerializeField] private float detectCooldown = 3f; // Tiempo antes de volver a girar tras perder al jugador.
    [SerializeField] private GameObject bulletPrefab; // Prefab de la bala que dispara la torreta.
    [SerializeField] private Transform firePoint; // Punto desde donde se disparan las balas.

    private bool isTracking = false; // Indica si la torreta está siguiendo al jugador.
    private Coroutine detectionCoroutine; // Referencia a la corrutina de disparo.

    /// <summary>
    /// Método FixedUpdate: Controla la rotación y detección del jugador.
    /// </summary>
    protected override void FixedUpdate()
    {
        if (player == null) return; // Si no hay jugador, no hace nada.

        if (!isTracking)
        {
            // Si no ha detectado al jugador, rota buscando objetivos.
            transform.Rotate(Vector3.up * rotationSpeed * Time.fixedDeltaTime);
        }

        CheckForPlayer(); // Verifica si el jugador está en su campo de visión.
    }

    /// <summary>
    /// Comprueba si el jugador está dentro del rango de detección y sin obstáculos en la línea de visión.
    /// </summary>
    private void CheckForPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Verifica si el jugador está dentro del rango y del ángulo de visión de la torreta.
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
            // Si el jugador sale de su visión, inicia el cooldown antes de volver a girar.
            isTracking = false;
            StartCoroutine(ResetTurret());
        }
    }

    /// <summary>
    /// Corrutina que hace que la torreta dispare continuamente mientras detecte al jugador.
    /// </summary>
    private IEnumerator ShootAtPlayer()
    {
        while (isTracking)
        {
            if (bulletPrefab != null && firePoint != null)
            {
                // Instancia la bala en el punto de disparo.
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

                if (bulletRb != null)
                {
                    Vector3 direction = (player.position - firePoint.position).normalized;
                    bulletRb.linearVelocity = direction * 20f; // Asigna velocidad a la bala.
                }
            }
            yield return new WaitForSeconds(1f); // 🔫 Dispara cada segundo.
        }
    }

    /// <summary>
    /// Corrutina que reinicia la torreta después de perder al jugador.
    /// </summary>
    private IEnumerator ResetTurret()
    {
        yield return new WaitForSeconds(detectCooldown);
        Debug.Log("🔄 Torreta no detecta al jugador, reanudando rotación...");
    }

    /// <summary>
    /// Método OnDrawGizmos: Dibuja el cono de visión en la escena para depuración.
    /// </summary>
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
