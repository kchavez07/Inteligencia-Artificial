using UnityEngine;

/// <summary>
/// Clase base abstracta para los estados del jefe final.
/// Todos los estados específicos (Melee, Ranged, etc.) deben heredar de esta clase.
/// </summary>
public abstract class BossState : MonoBehaviour
{
    /// <summary>
    /// Referencia al script del jefe final que permite correr Coroutines y lógica.
    /// </summary>
    protected MonoBehaviour boss;

    /// <summary>
    /// Se llama para inicializar el estado con el jefe al que pertenece.
    /// </summary>
    public virtual void Initialize(MonoBehaviour bossReference)
    {
        boss = bossReference;
    }

    /// <summary>
    /// Se ejecuta cuando se entra a este estado (ej. Melee o Ranged).
    /// </summary>
    public abstract void EnterState();

    /// <summary>
    /// Se ejecuta cada frame mientras el jefe está en este estado.
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    /// Se ejecuta cuando se sale de este estado (por ejemplo, cambia de Melee a Ranged).
    /// </summary>
    public abstract void ExitState();
}
