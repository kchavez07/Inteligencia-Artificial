using UnityEngine; 

/// <summary>
/// Controlador del jugador en la nueva escena con NavMesh.
/// Maneja el movimiento y la cámara que lo sigue.
/// </summary>
public class PlayerNavMesh : MonoBehaviour
{
    [Header("Movimiento del Jugador")]
    public float moveSpeed = 5f; // 📌 Velocidad de movimiento del jugador.

    [Header("Cámara")]
    private Transform cameraTransform; // 📌 Cámara principal que sigue al jugador.
    public Vector3 cameraOffset = new Vector3(0f, 5f, -10f); // 📌 Distancia de la cámara respecto al jugador.
    public float smoothSpeed = 5f; // 📌 Velocidad de suavizado de la cámara.

    private Rigidbody rb; // 📌 Referencia al Rigidbody del jugador.
    private Vector3 moveDirection; // 📌 Dirección en la que se moverá el jugador.

    /// <summary>
    /// Método Start: Se ejecuta al inicio del juego. 
    /// Inicializa el Rigidbody y configura la referencia a la cámara.
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // 🔹 Obtiene el componente Rigidbody del jugador.

        if (rb == null)
        {
            Debug.LogError("❌ No se encontró un Rigidbody en el PlayerNavMesh. Agrega uno en el Inspector.");
            enabled = false; // 🔹 Desactiva el script si no hay Rigidbody.
            return;
        }

        // 🔹 Configuración del Rigidbody para evitar efectos no deseados.
        rb.useGravity = false; // 🔹 Evita que el jugador caiga por efecto de la gravedad.
        rb.freezeRotation = true; // 🔹 Evita que el jugador rote automáticamente.

        // 🔹 Encuentra la cámara principal automáticamente si no está asignada.
        cameraTransform = Camera.main?.transform;
        if (cameraTransform == null)
        {
            Debug.LogError("❌ No se encontró una cámara principal en la escena.");
        }
    }

    /// <summary>
    /// Método Update: Captura la entrada del jugador para el movimiento.
    /// </summary>
    private void Update()
    {
        // 🔹 Captura la entrada de movimiento en los ejes X y Z.
        float moveX = Input.GetAxisRaw("Horizontal"); // 📌 Movimiento lateral.
        float moveZ = Input.GetAxisRaw("Vertical"); // 📌 Movimiento hacia adelante y atrás.

        // 🔹 Calcula la dirección de movimiento en función de la cámara.
        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward; // 📌 Dirección "hacia adelante" de la cámara.
            Vector3 right = cameraTransform.right; // 📌 Dirección "derecha" de la cámara.

            forward.y = 0; // 📌 Ignora la inclinación en el eje Y para evitar efectos indeseados.
            right.y = 0;

            moveDirection = (forward * moveZ + right * moveX).normalized; // 📌 Direcciona el movimiento según la cámara.
        }

        // 🔹 Si hay movimiento, el jugador gira en la dirección en la que se mueve.
        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection; // 📌 Asegura que el jugador siempre mire en la dirección del movimiento.
        }
    }

    /// <summary>
    /// Método FixedUpdate: Aplica el movimiento del jugador utilizando físicas.
    /// Se ejecuta en cada frame de física del motor.
    /// </summary>
    private void FixedUpdate()
    {
        if (rb != null)
        {
            // 🔹 Aplica el movimiento con la velocidad establecida sin afectar la velocidad en Y.
            rb.linearVelocity = moveDirection * moveSpeed + new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    /// <summary>
    /// Método LateUpdate: Se ejecuta después de Update y FixedUpdate.
    /// Ajusta la posición de la cámara para que siga suavemente al jugador.
    /// </summary>
    private void LateUpdate()
    {
        if (cameraTransform != null)
        {
            // 📌 Calcula la posición deseada de la cámara en función de la posición del jugador.
            Vector3 desiredPosition = transform.position + cameraOffset;

            // 📌 Aplica interpolación para un movimiento suave de la cámara.
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
