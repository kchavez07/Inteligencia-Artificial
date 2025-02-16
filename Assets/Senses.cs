using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Senses : MonoBehaviour
{
    // Lista de enemigos detectados dentro del radio de detección.
    protected List<GameObject> refEnemigosDetectados = new List<GameObject>();

    // Lista de obstáculos detectados dentro del radio de detección.
    protected List<GameObject> refObstaculosDetectados = new List<GameObject>();

    // Método para obtener la lista de obstáculos detectados.
    public List<GameObject> GetDetectedObstacles() { return refObstaculosDetectados; }

    // Radio de detección alrededor del dueño de este script.
    [SerializeField]
    protected float DetectionRadius = 12.5f;

    // Collider esférico utilizado para detectar objetos mediante colisiones.
    protected SphereCollider visionColliderSphere;

    // Variables para el estado de detección de enemigos.
    protected bool isEnemyDetected = false;
    protected GameObject detectedEnemy = null;

    // Método para obtener la referencia del enemigo detectado.
    public GameObject GetDetectedEnemyRef()
    {
        return detectedEnemy;
    }

    // Método para comprobar si un enemigo ha sido detectado.
    public bool IsEnemyDetected() { return isEnemyDetected; }

    // Corrutina para desalertar al enemigo después de un tiempo.
    private Coroutine CorrutinaDesalertar;
    bool IsAlerted = false;

    // Corrutina que espera 5 segundos antes de cambiar el estado a desalerta.
    private IEnumerator Desalertar()
    {
        yield return new WaitForSeconds(5);
        Debug.LogWarning("Pasamos a desalerta");
        IsAlerted = false;
    }

    // Método Start, llamado antes del primer frame.
    void Start()
    {
        // Obtiene el componente SphereCollider y ajusta su radio de detección.
        visionColliderSphere = GetComponent<SphereCollider>();
        if (visionColliderSphere != null)
        {
            visionColliderSphere.radius = DetectionRadius;
        }
    }

    // Método llamado cuando otro objeto entra en el área de detección.
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Entró a colisión con: {other.gameObject.name}");

        // Si el objeto detectado pertenece a la capa "Player", se agrega a la lista de enemigos detectados.
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            refEnemigosDetectados.Add(other.gameObject);

            // Si la corrutina de desalerta está activa, se detiene para evitar la desalerta prematura.
            if(CorrutinaDesalertar != null)
            {
                 StopCoroutine(CorrutinaDesalertar);
            }
            IsAlerted = true;
        }
        // Si el objeto pertenece a la capa "Waypoints", se agrega a la lista de enemigos detectados.
        else if(other.gameObject.layer == LayerMask.NameToLayer("Waypoints")) 
        {
            refEnemigosDetectados.Add(other.gameObject);
        }
        // Si el objeto pertenece a la capa "Obstacle", se agrega a la lista de obstáculos detectados.
        else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            refObstaculosDetectados.Add(other.gameObject);
        }
    }

    // Método llamado cuando un objeto sale del área de detección.
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Salió de colisión con: {other.gameObject.name}");

        // Si el objeto es un jugador, se elimina de la lista y se inicia la corrutina de desalerta.
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            refEnemigosDetectados.Remove(other.gameObject);
            CorrutinaDesalertar = StartCoroutine(Desalertar());
        }
        // Si el objeto es un Waypoint, se elimina de la lista de enemigos detectados.
        else if (other.gameObject.layer == LayerMask.NameToLayer("Waypoints"))
        {
            refEnemigosDetectados.Remove(other.gameObject);
        }
        // Si el objeto es un obstáculo, se elimina de la lista de obstáculos detectados.
        else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            refObstaculosDetectados.Remove(other.gameObject);
        }
    }

    // Método para verificar si un objeto está dentro del radio de detección.
    public bool IsInsideRadius(Vector3 pos1, Vector3 pos2, float radius)
    {
        return (pos1 - pos2).magnitude <= radius;
    }

    // Sobrecarga del método IsInsideRadius para aceptar GameObjects en lugar de posiciones Vector3.
    public bool IsInsideRadius(GameObject pos1, GameObject pos2, float radius)
    {
        return IsInsideRadius(pos1.transform.position, pos2.transform.position, radius);
    }

    // Método para obtener los objetos cercanos dentro de un radio específico.
    public List<GameObject> GetNearbyObjects(Vector3 originPosition, float radius)
    {
        List<GameObject> nearbyObjects = new List<GameObject>();

        // Encuentra todos los objetos en la escena.
        GameObject[] foundObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID);

        foreach (GameObject obj in foundObjects)
        {
            if (obj == this.gameObject) // Ignorar el dueño del script.
                continue;

            // Si el objeto está dentro del radio, se añade a la lista.
            if(IsInsideRadius(originPosition, obj.transform.position, radius))
            {
                nearbyObjects.Add(obj);
            }
        }

        return nearbyObjects;
    }

    private void FixedUpdate()
    {
        float bestDistance = float.MaxValue;
        GameObject nearestGameObj = null;

        // Busca el objeto más cercano dentro de los enemigos detectados.
        foreach (GameObject obj in refEnemigosDetectados)
        {
            float currentDistance = (transform.position - obj.transform.position).magnitude;
            if (currentDistance < bestDistance)
            {
                Debug.LogWarning("" + obj.name);
                bestDistance = currentDistance;
                nearestGameObj = obj;
            }
        }

        // Si hay un enemigo detectado, se marca como detectado, en caso contrario se limpia la detección.
        isEnemyDetected = nearestGameObj != null;
        detectedEnemy = nearestGameObj;
    }

    // Método para visualizar el radio de detección en la escena de Unity.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Si hay un enemigo detectado dentro del radio, cambia el color a rojo.
        if (detectedEnemy != null)
        {        
            if (IsInsideRadius(gameObject, detectedEnemy, DetectionRadius))
            {
                Gizmos.color = Color.red;
            }
        }

        // Dibuja el radio de detección.
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);
    }

    // Método Update vacío. Se puede utilizar para futuras actualizaciones.
    void Update()
    {

    }
}
