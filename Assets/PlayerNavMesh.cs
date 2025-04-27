using UnityEngine;

/// <summary>
/// Controlador completo del jugador: movimiento relativo a la cámara + disparo + seguimiento de cámara + vida.
/// </summary>
public class PlayerFullController : MonoBehaviour
{
    [Header("Movimiento del Jugador")]
    public float moveSpeed = 5f; // 📌 Velocidad de movimiento del jugador.

    [Header("Disparo")]
    public GameObject bulletPrefab; // 📌 Prefab de la bala que dispara el jugador.
    public Transform firePoint; // 📌 Punto desde donde se disparan las balas.
    public float bulletSpeed = 10f; // 📌 Velocidad de la bala.

    [Header("Cámara")]
    private Transform cameraTransform; // 📌 Cámara principal que sigue al jugador.
    public Vector3 cameraOffset = new Vector3(0f, 5f, -10f); // 📌 Offset de la cámara respecto al jugador.
    public float smoothSpeed = 5f; // 📌 Velocidad de suavizado de la cámara.

    private Rigidbody rb; // 📌 Referencia al Rigidbody del jugador.
    private Vector3 moveDirection; // 📌 Dirección en la que se moverá el jugador.

    [Header("Vida del Jugador")]
    public int maxHealth = 100; // 💖 Vida máxima del jugador.
    private int currentHealth; // 💖 Vida actual del jugador.

    /// <summary>
    /// Inicializa componentes.
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("❌ No se encontró un Rigidbody en el Player.");
            enabled = false;
            return;
        }

        rb.useGravity = true;
        rb.freezeRotation = true;

        cameraTransform = Camera.main?.transform;
        if (cameraTransform == null)
        {
            Debug.LogError("❌ No se encontró la cámara principal.");
        }

        currentHealth = maxHealth;
    }

    /// <summary>
    /// Captura entrada de movimiento y disparo.
    /// </summary>
    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;

            moveDirection = (forward * moveZ + right * moveX).normalized;
        }

        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    /// <summary>
    /// Aplica movimiento en FixedUpdate.
    /// </summary>
    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * moveSpeed + new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    /// <summary>
    /// Maneja la cámara que sigue al jugador.
    /// </summary>
    private void LateUpdate()
    {
        if (cameraTransform != null)
        {
            Vector3 desiredPosition = transform.position + cameraOffset;
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Instancia y dispara una bala hacia adelante.
    /// </summary>
    private void Shoot()
    {
        if (firePoint == null || bulletPrefab == null)
        {
            Debug.LogError("❌ Faltan referencias de disparo (firePoint o bulletPrefab).");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            bulletRb.linearVelocity = transform.forward * bulletSpeed;
        }
    }

    /// <summary>
    /// 💥 Recibe daño y reduce la vida del jugador.
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"🔥 El jugador recibió {damageAmount} de daño. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 💀 Maneja la muerte del jugador.
    /// </summary>
    private void Die()
    {
        Debug.Log("💀 El jugador ha muerto.");
        // Aquí puedes reiniciar nivel o mostrar UI de Game Over si quieres
        Destroy(gameObject);
    }
}
