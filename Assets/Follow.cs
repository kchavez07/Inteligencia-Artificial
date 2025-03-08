using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public Vector3 offset = new Vector3(0f, 5f, -10f); // 📌 Distancia de la cámara al personaje
    public float smoothSpeed = 5f; // 📌 Velocidad de suavizado

    void LateUpdate()
    {
        if (player != null)
        {
            // 📌 La cámara sigue al jugador manteniendo el offset
            Vector3 desiredPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
    }
}

// ==============================
// 📌 TUTORIALES RELACIONADOS 
// ==============================
// Lo estuve haciendo saul eduardo 
// 1️⃣ Implementación de Patrullaje de Enemigos con NavMesh en Unity
// Cómo hacer que un enemigo patrulle utilizando NavMesh.
// 🔗 Enlace: 
// https://www.youtube.com/watch?v=rfajB5QTxxo

// 2️⃣ Detección de Objetos dentro de un Cono de Visión en Unity
// Cómo programar un cono de visión para detectar objetos en el campo de visión del enemigo.
// 🔗 Enlace: 
// https://www.youtube.com/watch?v=GgSp5drI-JA

// 3️⃣ Cómo Hacer que un Enemigo Persiga al Jugador usando NavMesh
// Cómo hacer que un enemigo detecte y persiga al jugador utilizando NavMeshAgent.
// 🔗 Enlace: 
// https://www.youtube.com/watch?v=UvDqnbjEEak

// 4️⃣ Creación de un Cono de Visión para Enemigos en Unity 3D
// Cómo hacer que los enemigos tengan un cono de visión visible en la escena.
// 🔗 Enlace: 
// https://www.youtube.com/watch?v=7Av9Wl3LY4w

// 5️⃣ Cómo Detectar un Personaje en un Campo de Visión - Tutorial Unity
// Cómo programar un sistema para que los enemigos detecten al jugador en un área de visión.
// 🔗 Enlace: 
// https://www.youtube.com/watch?v=lV47ED8h61k
