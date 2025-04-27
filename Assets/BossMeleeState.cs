using UnityEngine;
using System.Collections;

/// <summary>
/// Estado Melee del jefe: ejecuta ataques físicos (golpe directo, embestida, ataque en área).
/// Ahora incluye un efecto de estela (Trail) durante la embestida y daño al jugador.
/// </summary>
public class BossMeleeState : BossState
{
    private int currentAttackIndex = 0;
    private bool isAttacking = false;
    private Transform player;
    private TrailRenderer trail;
    private Rigidbody bossRb;

    [Header("Configuración de Embestida")]
    public float chargeForce = 20f;
    public float chargeDuration = 0.5f;

    [Header("Daño de ataques")]
    public int directHitDamage = 20;
    public int chargeDamage = 30;
    public int areaDamage = 25;
    public float areaRadius = 5f; // Radio del área de golpe expansivo

    public override void EnterState()
    {
        Debug.Log("🟥 Entró al estado MELEE");

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        trail = boss.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.enabled = false;
        }

        bossRb = boss.GetComponent<Rigidbody>();
        if (bossRb == null)
        {
            Debug.LogWarning("⚠️ Boss no tiene Rigidbody asignado.");
        }

        StartCoroutine(MeleeAttackCycle());
    }

    public override void ExitState()
    {
        Debug.Log("⛔ Saliendo del estado MELEE");
        StopAllCoroutines();
        isAttacking = false;

        if (trail != null)
        {
            trail.enabled = false;
        }
    }

    public override void UpdateState()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - boss.transform.position).normalized;
            direction.y = 0;
            boss.transform.forward = Vector3.Lerp(boss.transform.forward, direction, 5f * Time.deltaTime);
        }
    }

    private IEnumerator MeleeAttackCycle()
    {
        while (true)
        {
            if (!isAttacking)
            {
                isAttacking = true;

                switch (currentAttackIndex)
                {
                    case 0:
                        yield return StartCoroutine(DirectHit());
                        break;
                    case 1:
                        yield return StartCoroutine(ChargeAttack());
                        break;
                    case 2:
                        yield return StartCoroutine(AreaSmash());
                        break;
                }

                currentAttackIndex = (currentAttackIndex + 1) % 3;
                isAttacking = false;

                yield return new WaitForSeconds(2f);
            }

            yield return null;
        }
    }

    private IEnumerator DirectHit()
    {
        Debug.Log("👊 Golpe directo");

        if (player != null)
        {
            float distance = Vector3.Distance(boss.transform.position, player.position);
            if (distance <= 2f)
            {
                player.GetComponent<PlayerFullController>()?.TakeDamage(directHitDamage);
            }
        }

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator ChargeAttack()
    {
        Debug.Log("🏃 Embestida");

        if (trail != null)
        {
            trail.enabled = true;
        }

        if (bossRb != null && player != null)
        {
            Vector3 chargeDirection = (player.position - boss.transform.position).normalized;
            chargeDirection.y = 0;

            bossRb.linearVelocity = chargeDirection * chargeForce;
        }

        yield return new WaitForSeconds(chargeDuration);

        if (bossRb != null)
        {
            bossRb.linearVelocity = Vector3.zero;
        }

        if (trail != null)
        {
            trail.enabled = false;
        }

        // ✅ Detectar colisión manualmente por distancia después de la embestida
        if (player != null)
        {
            float distance = Vector3.Distance(boss.transform.position, player.position);
            if (distance <= 2.5f)
            {
                player.GetComponent<PlayerFullController>()?.TakeDamage(chargeDamage);
            }
        }
    }

    private IEnumerator AreaSmash()
    {
        Debug.Log("🌋 Ataque en área");

        if (player != null)
        {
            float distance = Vector3.Distance(boss.transform.position, player.position);
            if (distance <= areaRadius)
            {
                player.GetComponent<PlayerFullController>()?.TakeDamage(areaDamage);
            }
        }

        yield return new WaitForSeconds(1.5f);
    }
}
