using UnityEngine;

public class HeavyEnemy : BaseEnemy
{
    [SerializeField] private float acceleration = 0.5f;
    [SerializeField] private float maxSpeed = 3f;
    private bool isChasing = false;
    private float currentSpeed = 0f;

    protected override void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }

        if (isChasing)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.fixedDeltaTime);
            rb.MovePosition(rb.position + direction * currentSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            currentSpeed = 0f;
        }
    }
}
