using UnityEngine;

/// <summary>
/// Controlador del jugador. Maneja el movimiento y el disparo.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento del Jugador")]
    public float moveSpeed = 5f; // Velocidad de movimiento del jugador.
    
    [Header("Disparo")]
    public GameObject bulletPrefab; // Prefab de la bala que dispara el jugador.
    public Transform firePoint; // Punto desde donde se disparan las balas.
    public float bulletSpeed = 10f; // Velocidad de la bala al ser disparada.

    private Rigidbody rb; // Referencia al Rigidbody del jugador.
    private Vector3 moveDirection; // Dirección en la que el jugador se moverá.

    [Header("Cámara")]
    public Transform cameraTransform; // 🔹 Referencia a la cámara para ajustar el movimiento relativo a ella.

    /// <summary>
    /// Método Start: Se ejecuta al iniciar el juego. 
    /// Inicializa el Rigidbody y bloquea el cursor dentro de la ventana.
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Obtiene el Rigidbody del jugador.
        Cursor.lockState = CursorLockMode.Confined; // Bloquea el cursor dentro de la ventana del juego.
    }

    /// <summary>
    /// Método Update: Se ejecuta cada frame. 
    /// Maneja el movimiento del jugador y la detección del disparo.
    /// </summary>
    private void Update()
    {
        // 🔹 Captura la entrada de movimiento del teclado (WASD).
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // 🔹 Calcula la dirección de movimiento en función de la cámara.
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0; // Se ignora el eje Y para evitar que el jugador se incline.
        right.y = 0;

        moveDirection = (forward * moveZ + right * moveX).normalized; // Normaliza la dirección para un movimiento uniforme.

        // 🔹 Hace que el jugador siempre mire en la dirección en la que se mueve.
        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection;
        }

        // 🔹 Detecta si el jugador presiona "Espacio" para disparar.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    /// <summary>
    /// Método FixedUpdate: Se ejecuta en cada frame de física.
    /// Aplica el movimiento del jugador con Rigidbody.
    /// </summary>
    private void FixedUpdate()
    {
        // 🔹 Aplica el movimiento con la velocidad establecida, manteniendo la velocidad vertical.
        rb.linearVelocity = moveDirection * moveSpeed + new Vector3(0, rb.linearVelocity.y, 0);
    }

    /// <summary>
    /// Método Shoot: Crea y dispara una bala en la dirección en la que el jugador está mirando.
    /// </summary>
    private void Shoot()
    {
        // 🔹 Verifica que `firePoint` y `bulletPrefab` estén asignados antes de disparar.
        if (firePoint == null || bulletPrefab == null)
        {
            Debug.LogError("❌ Falta FirePoint o BulletPrefab en el PlayerController.");
            return;
        }

        // 🔹 Instancia la bala en `firePoint`.
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 🔹 Obtiene el Rigidbody de la bala y le aplica velocidad en la dirección del jugador.
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = transform.forward * bulletSpeed;
        }
    }
}
