using UnityEngine;

/// <summary>
/// Selector de estados del jefe final.
/// Se encarga de cambiar entre los estados Melee y Ranged
/// dependiendo de la distancia con el jugador.
/// </summary>
public class BossStateSelector : MonoBehaviour
{
    [Header("Rango de decisión de ataque")]
    [Tooltip("Distancia mínima para cambiar al estado Melee.")]
    public float meleeRange = 7f;

    [Tooltip("Distancia máxima para cambiar al estado Ranged.")]
    public float rangedRange = 12f;

    [Header("Referencias de estado")]
    public BossState meleeState;
    public BossState rangedState;

    private BossState currentState;
    private Transform player;

    private void Start()
    {
        // Busca al jugador en la escena
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("❌ No se encontró un objeto con el tag 'Player'.");
        }

        // Inicializa ambos estados con referencia al jefe
        meleeState.Initialize(this);
        rangedState.Initialize(this);

        // Inicia en Melee por defecto (puedes cambiarlo)
        ChangeState(meleeState);
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Cambiar al estado Melee si el jugador está muy cerca
        if (distance <= meleeRange && currentState != meleeState)
        {
            ChangeState(meleeState);
        }
        // Cambiar al estado Ranged si el jugador se aleja lo suficiente
        else if (distance > rangedRange && currentState != rangedState)
        {
            ChangeState(rangedState);
        }

        // Actualizar el estado actual
        currentState?.UpdateState();
    }

    /// <summary>
    /// Cambia de un estado a otro limpiamente.
    /// </summary>
    private void ChangeState(BossState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }

        currentState = newState;
        currentState.EnterState();

        Debug.Log($"🔁 Cambio de estado a: {currentState.GetType().Name}");
    }
}
