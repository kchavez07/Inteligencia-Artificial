using UnityEngine;

/// <summary>
/// Script que maneja la vida del Final Boss.
/// Permite recibir daño y detectar la muerte del jefe.
/// </summary>
public class FinalBossHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [Tooltip("Vida máxima del jefe final.")]
    public int maxHealth = 500;

    private int currentHealth;

    [Header("Referencias Opcionales")]
    [Tooltip("Objeto que se destruirá o desactivará al morir (por ejemplo el modelo del jefe).")]
    public GameObject bossVisual;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Método para recibir daño.
    /// </summary>
    /// <param name="damageAmount">Cantidad de daño recibido.</param>
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0); // No bajar de 0.

        Debug.Log($"🩸 Boss recibió daño. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Método que maneja la muerte del jefe.
    /// </summary>
    private void Die()
    {
        Debug.Log("☠️ ¡El Final Boss ha sido derrotado!");

        if (bossVisual != null)
        {
            bossVisual.SetActive(false); // Desaparece al jefe visualmente.
        }

        // Aquí puedes agregar otras acciones: activar animación de muerte, sonido, partículas, etc.
        // Destroy(gameObject); // Si quieres destruir todo el objeto.
    }

    /// <summary>
    /// Método para obtener el porcentaje de vida (para la barra de vida).
    /// </summary>
    /// <returns>Porcentaje de vida restante (0 a 1).</returns>
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}
