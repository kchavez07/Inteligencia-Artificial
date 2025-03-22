using UnityEngine;

/// <summary>
/// Bala usada por el enemigo escapista.
/// Se mueve en una dirección específica y aplica daño al jugador.
/// Aplica diferente cantidad de daño dependiendo del estado del enemigo (activo o cansado).
/// </summary>
public class BulletEnemy : MonoBehaviour
{
    [Header("Daño de la Bala")]
    [Tooltip("Daño cuando el enemigo está en estado activo.")]
    public float damageActivo = 5f;

    [Tooltip("Daño cuando el enemigo está en estado cansado.")]
    public float damageCansado = 2f;

    [Header("Configuración de Vida")]
    [Tooltip("Tiempo de vida de la bala antes de destruirse automáticamente.")]
    public float lifeTime = 5f;

    /// <summary>Velocidad de la bala en el espacio.</summary>
    private Vector3 velocity;

    /// <summary>Indica si el enemigo que disparó la bala estaba cansado (afecta el daño).</summary>
    private bool isCansado = false;

    /// <summary>
    /// Se llama al iniciar. Destruye automáticamente la bala después de cierto tiempo.
    /// </summary>
    void Start()
    {
        Destroy(gameObject, lifeTime); // ⏱️ Autodestruye la bala después del tiempo establecido
    }

    /// <summary>
    /// Configura la dirección y velocidad de la bala, además del estado del enemigo.
    /// </summary>
    /// <param name="direction">Dirección en la que se moverá la bala.</param>
    /// <param name="speed">Velocidad de desplazamiento.</param>
    /// <param name="cansado">Si el enemigo estaba en estado cansado.</param>
    public void SetDirection(Vector3 direction, float speed, bool cansado)
    {
        velocity = direction.normalized * speed;
        isCansado = cansado;
    }

    /// <summary>
    /// Mueve la bala en cada frame según su velocidad.
    /// </summary>
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }

    /// <summary>
    /// Detecta colisiones con otros objetos y aplica daño si impacta al jugador.
    /// </summary>
    /// <param name="other">Collider del objeto con el que colisionó.</param>
    private void OnTriggerEnter(Collider other)
    {
        // ✅ Si la bala impacta al jugador
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                // Aplica el daño según el estado del enemigo que disparó
                float damage = isCansado ? damageCansado : damageActivo;
                player.TakeDamage(Mathf.RoundToInt(damage)); // 🔁 Convierte el daño a entero

                Debug.Log($"🔥 Bala impactó al jugador. Daño: {damage}");
            }

            Destroy(gameObject); // 💥 Se destruye después de impactar
        }

        // ✅ Si impacta con cualquier objeto que NO sea otro enemigo, también se destruye
        if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
