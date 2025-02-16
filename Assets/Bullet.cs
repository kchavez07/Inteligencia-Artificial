using UnityEngine;

/// <summary>
/// Clase que representa una bala en el juego.
/// Su principal función es detectar colisiones y aplicar daño.
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField]
    protected LayerMask mask; // Máscara de colisión que define qué objetos pueden ser impactados por la bala.

    [SerializeField]
    protected float damage; // Cantidad de daño que la bala inflige al impactar.

    /// <summary>
    /// Devuelve la cantidad de daño que hace la bala.
    /// </summary>
    /// <returns>Valor de daño de la bala.</returns>
    public float GetDamage() { return damage; }

    /// <summary>
    /// Detecta si la bala impacta contra un objeto dentro de la capa especificada.
    /// </summary>
    /// <param name="other">Collider del objeto con el que la bala colisiona.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Obtiene el valor de la capa del objeto con el que colisionó.
        var maskValue = 1 << other.gameObject.layer;
        var maskANDmaskValue = (maskValue & mask.value); // Comprueba si la capa está en la máscara de colisión.

        // Si el objeto está en una de las capas permitidas (Enemigos, Paredes, Obstáculos), se destruye la bala.
        if (maskANDmaskValue > 0)  
        {
            // Debug.Log("Choque con algo en la capa " + LayerMask.LayerToName(other.gameObject.layer));

            // Destruye la bala tras la colisión.
            Destroy(gameObject);
        }
    }

    // Métodos Start y Update no implementados actualmente.
    void Start()
    {
        // Método vacío por si en el futuro se requiere inicialización específica.
    }

    void Update()
    {
        // Método vacío por si en el futuro se requiere lógica en cada frame.
    }
}
