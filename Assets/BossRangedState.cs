using UnityEngine;
using System.Collections;

/// <summary>
/// Estado del jefe para ataques a distancia.
/// Alterna entre disparos básicos en ráfaga y un ataque especial (bola explosiva).
/// </summary>
public class BossRangedState : BossState
{
    [Header("Configuración de Disparo Básico (Ráfaga)")]
    public GameObject bulletPrefab; // Prefab de la bala normal
    public Transform firePoint; // Punto de disparo
    public int burstCount = 3; // Número de balas por ráfaga
    public float burstRate = 0.2f; // Tiempo entre disparos de la ráfaga
    public float bulletSpeed = 20f; // Velocidad de la bala normal

    [Header("Configuración de Disparo Especial (Bola Explosiva)")]
    public GameObject explosivePrefab; // Prefab de la bola explosiva
    public float explosiveSpeed = 15f; // Velocidad de la bola explosiva
    public float specialCooldown = 5f; // Tiempo de recarga del ataque especial

    private float lastSpecialTime = -999f; // Última vez que lanzó el especial
    private bool isAttacking = false;

    private Transform player;

    public override void EnterState()
    {
        Debug.Log("🔵 Boss entró en estado Ranged.");
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        isAttacking = false;
    }

    public override void ExitState()
    {
        Debug.Log("🔴 Boss salió del estado Ranged.");
        if (boss != null)
        {
            boss.StopAllCoroutines(); // ✅ Detiene cualquier Coroutine que estuviera activa en el Boss
        }
        isAttacking = false;
    }

    public override void UpdateState()
    {
        if (player == null) return;

        // 🔹 El jefe siempre mira al jugador
        Vector3 lookDirection = player.position - boss.transform.position;
        lookDirection.y = 0; // No rotar verticalmente
        boss.transform.forward = Vector3.Lerp(boss.transform.forward, lookDirection.normalized, 5f * Time.deltaTime);

        if (!isAttacking)
        {
            if (Time.time >= lastSpecialTime + specialCooldown)
            {
                if (boss != null)
                    boss.StartCoroutine(SpecialAttack()); // ✅ CORRECTO: usando el Boss para lanzar Coroutine
            }
            else
            {
                if (boss != null)
                    boss.StartCoroutine(BurstAttack()); // ✅ CORRECTO: usando el Boss para lanzar Coroutine
            }
        }
    }

    /// <summary>
    /// 🔹 Ráfaga de disparos básicos hacia el jugador.
    /// </summary>
    private IEnumerator BurstAttack()
    {
        isAttacking = true;

        Debug.Log("🔫 Ráfaga de disparos básicos!");

        for (int i = 0; i < burstCount; i++)
        {
            ShootBullet();
            yield return new WaitForSeconds(burstRate);
        }

        yield return new WaitForSeconds(1f); // Tiempo de recuperación después de la ráfaga
        isAttacking = false;
    }

    /// <summary>
    /// 🔹 Lanza una bola explosiva como ataque especial.
    /// </summary>
    private IEnumerator SpecialAttack()
    {
        isAttacking = true;
        lastSpecialTime = Time.time;

        Debug.Log("💥 Lanzando bola explosiva!");

        if (explosivePrefab != null && firePoint != null)
        {
            GameObject explosive = GameObject.Instantiate(explosivePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = explosive.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * explosiveSpeed;
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Faltan referencias para lanzar el ataque especial.");
        }

        yield return new WaitForSeconds(2f); // Tiempo de recuperación tras el especial
        isAttacking = false;
    }

    /// <summary>
    /// 🔹 Dispara una bala normal hacia el jugador.
    /// </summary>
    private void ShootBullet()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * bulletSpeed;
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Faltan referencias para disparar balas.");
        }
    }
}
