using UnityEngine;

/// <summary>
/// Controlador de vida del jefe final.
/// Maneja daño, muerte y comunicación con la barra de vida.
/// </summary>
public class FinalBossController : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    private BossHealthBarController healthBar; // 🔥 Referencia para actualizar la barra

    private void Start()
    {
        currentHealth = maxHealth;

        // Buscar automáticamente la barra de vida si no está asignada
        healthBar = FindObjectOfType<BossHealthBarController>();
        if (healthBar == null)
        {
            Debug.LogWarning("⚠️ No se encontró un BossHealthBarController en la escena.");
        }
    }

    /// <summary>
    /// Aplica daño al jefe final.
    /// </summary>
    /// <param name="damage">Cantidad de daño recibido.</param>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"⚡ Jefe recibió {damage} de daño. Vida restante: {currentHealth}");

        // Actualizar la barra de vida cada vez que recibe daño
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Método que se llama cuando el jefe se queda sin vida.
    /// </summary>
    private void Die()
    {
        Debug.Log("💀 El jefe final ha muerto.");

        // 🔥 IMPORTANTE: También destruye el HUD de vida si existe
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject); // Destruye el objeto de la barra visual
        }

        Destroy(gameObject); // Después destruye el jefe
    }
}
