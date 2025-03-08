using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f; // Velocidad de movimiento del jugador.
    private Rigidbody rb;
    private Vector3 moveInput;

    [Header("Vida del Jugador")]
    [SerializeField] private int maxHP = 10; // Vida máxima del jugador.
    private int currentHP;

    [Header("Disparo")]
    [SerializeField] private GameObject bulletPrefab; // **Prefab de la bala**
    [SerializeField] private Transform firePoint; // **Punto desde donde se disparan las balas**
    [SerializeField] private float bulletSpeed = 20f; // **Velocidad de la bala**

    private Quaternion lastRotation; // **Última rotación en la que miraba el jugador**

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHP = maxHP;
        lastRotation = transform.rotation; // **Guarda la rotación inicial del jugador**
    }

    void Update()
    {
        // Captura la entrada del usuario en los ejes X y Z.
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // **Genera el vector de movimiento**
        moveInput = new Vector3(moveX, 0f, moveZ).normalized;

        // **Si el jugador se mueve, rota en esa dirección**
        if (moveInput.magnitude > 0)
        {
            transform.forward = moveInput; // **Rota el jugador hacia donde se mueve**
            lastRotation = transform.rotation; // **Actualiza la última rotación**
        }

        // **Si el jugador presiona Espacio, dispara**
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        // **Mueve al jugador sin afectar la rotación**
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Método para disparar en la dirección en la que el jugador está viendo.
    /// </summary>
    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // **Instancia la bala en la posición del firePoint con la rotación actual**
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, lastRotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {
                bulletRb.linearVelocity = bullet.transform.forward * bulletSpeed; // **Dispara en la dirección en la que el jugador está viendo**
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"¡El jugador recibió {damage} de daño! Vida restante: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto.");
    }
}