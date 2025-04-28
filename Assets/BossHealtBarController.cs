using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlador que actualiza la barra de vida del jefe final.
/// </summary>
public class BossHealthBarController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Slider que representa la barra de vida del jefe.")]
    public Slider healthSlider;

    [Tooltip("Referencia al enemigo jefe (debe tener sistema de vida).")]
    public FinalBossController boss; // Cambia el nombre si tu jefe tiene otro script

    private void Start()
    {
        if (healthSlider == null)
        {
            Debug.LogWarning("⚠️ No se asignó el Slider en BossHealthBarController.");
            enabled = false;
            return;
        }

        if (boss == null)
        {
            Debug.LogWarning("⚠️ No se asignó el Boss en BossHealthBarController.");
            enabled = false;
            return;
        }

        // 🔹 Configura el valor máximo al iniciar
        healthSlider.maxValue = boss.maxHealth;
        healthSlider.value = boss.maxHealth;
    }

    /// <summary>
    /// Actualiza visualmente la barra de vida.
    /// </summary>
    /// <param name="currentHealth">Vida actual del jefe.</param>
    /// <param name="maxHealth">Vida máxima del jefe.</param>
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void Update()
    {
        // ✅ Agregado: si el jefe existe, actualiza la barra
        if (boss != null)
        {
            healthSlider.value = boss.currentHealth;
        }
    }
}
