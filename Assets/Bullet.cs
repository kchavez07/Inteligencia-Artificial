using UnityEngine;

/// <summary>
/// Clase que representa una bala en el juego.
/// Su principal función es detectar colisiones y aplicar daño.
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private LayerMask mask; // **Capa de colisión para la bala (Detecta Enemigos y Jefe)**

    [SerializeField]
    private float damage = 10f; // **💥 Daño de la bala (Configurable en el Inspector)**

    [SerializeField]
    private float speed = 20f; // **Velocidad de la bala**

    [SerializeField]
    private float lifeTime = 5f; // **Duración antes de autodestruirse**

    private Rigidbody rb;
    private Vector3 moveDirection = Vector3.forward; // Dirección por defecto

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("⚠️ La bala no tiene un Rigidbody.");
            return;
        }

        rb.useGravity = false;
        rb.isKinematic = true; // ✅ Movimiento manual

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World); 
        // ✅ Mueve la bala en la dirección que le hayas asignado
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("💥 Impacto con: " + other.gameObject.name + " en la capa " + LayerMask.LayerToName(other.gameObject.layer));

        if (((1 << other.gameObject.layer) & mask.value) != 0)
        {
            Debug.Log("🔥 Bala impactó un objetivo válido.");

            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            FinalBossController boss = other.GetComponent<FinalBossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }
        }

        if (!other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Devuelve la cantidad de daño que hace la bala.
    /// </summary>
    public float GetDamage()
    {
        return damage;
    }

    /// <summary>
    /// Permite asignar la dirección de la bala externamente.
    /// </summary>
    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }
}
