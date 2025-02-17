using UnityEngine;

/// <summary>
/// Clase que representa una bala en el juego.
/// Su principal función es detectar colisiones y aplicar daño.
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private LayerMask mask; // **Capa de colisión para la bala (Detecta Enemigos)**

    [SerializeField]
    private float damage = 10f; // **💥 Daño de la bala (Configurable en el Inspector)**

    [SerializeField]
    private float speed = 20f; // **Velocidad de la bala**

    [SerializeField]
    private float lifeTime = 5f; // **Duración antes de autodestruirse**

    private Rigidbody rb;

    /// <summary>
    /// Método Start: Inicializa la bala y le asigna una velocidad.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("⚠️ La bala no tiene un Rigidbody.");
            return;
        }

        rb.useGravity = false; // **La bala no es afectada por la gravedad**
        rb.linearVelocity = transform.forward * speed; // **🚀 Se mueve en la dirección en la que fue disparada**
        
        Destroy(gameObject, lifeTime); // **🕒 Se autodestruye después de X segundos**
    }

    /// <summary>
    /// Detecta si la bala impacta contra un objeto dentro de la capa especificada.
    /// </summary>
    /// <param name="other">Collider del objeto con el que la bala colisiona.</param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("💥 Impacto con: " + other.gameObject.name + " en la capa " + LayerMask.LayerToName(other.gameObject.layer));

        // **Si el objeto impactado está en la capa correcta, aplica daño**
        if (((1 << other.gameObject.layer) & mask.value) != 0)
        {
            Debug.Log("🔥 Bala impactó un objetivo válido.");

            // **Aplica daño si impacta un enemigo**
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // ✅ Aplica daño al enemigo
            }

            Destroy(gameObject); // **💣 Destruye la bala tras impactar**
        }
    }

    /// <summary>
    /// Devuelve la cantidad de daño que hace la bala.
    /// </summary>
    /// <returns>Valor de daño de la bala.</returns>
    public float GetDamage()
    {
        return damage;
    }
}
