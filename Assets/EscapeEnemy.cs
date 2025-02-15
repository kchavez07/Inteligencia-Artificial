using UnityEngine;
using System.Collections;

public class EscapeEnemy : BaseEnemy
{
    [SerializeField] private float fleeSpeed = 5f;
    [SerializeField] private float maxFleeDuration = 3f;
    [SerializeField] private float restDuration = 2f;
    [SerializeField] private float predictionTime = 1.5f;
    [SerializeField] private LayerMask groundLayer;

    private bool isFleeing = false;
    private bool isResting = false;
    private bool playerDetected = false;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(FleeCycle());
    }

    private IEnumerator FleeCycle()
    {
        while (true)
        {
            if (playerDetected)
            {
                isFleeing = true;
                yield return new WaitForSeconds(maxFleeDuration);

                isFleeing = false;
                isResting = true;
                rb.linearVelocity = Vector3.zero;
                yield return new WaitForSeconds(restDuration);

                isResting = false;
            }
            yield return null;
        }
    }

    protected override void FixedUpdate()
    {
        if (player == null || isResting) return;

        CheckForPlayer();

        if (playerDetected && isFleeing)
        {
            Vector3 fleeDirection = (transform.position - player.position).normalized;
            fleeDirection.y = 0;

            if (steering != null) // ✅ Ahora usamos `steering` de `BaseEnemy.cs`
            {
                steering.SetEnemyReference(player.gameObject);
            }
            else
            {
                rb.linearVelocity = fleeDirection * fleeSpeed;
            }
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }

        StayOnGround();
    }

    private void CheckForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, detectionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerDetected = true;
                    Debug.Log("👀 Enemigo detectó al jugador y comenzará a huir.");
                }
            }
        }
        else
        {
            playerDetected = false;
        }
    }

    private void StayOnGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, groundLayer))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
    }
}
