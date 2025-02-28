using UnityEngine;
using System.Collections.Generic;

public class TileGrid : MonoBehaviour
{
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 5;
    [SerializeField] private Vector2Int beginNodePos = new Vector2Int(0, 0);
    [SerializeField] private Vector2Int goalNodePos = new Vector2Int(4, 4);
    private Node[][] nodeGrid;
    private List<Node> pathNodes = new List<Node>(); // Almacena la ruta en el orden correcto

    void Start()
    {
        // 🚀 Validación extra para evitar errores si `goalNodePos` está fuera de la cuadrícula
        if (goalNodePos.x < 0 || goalNodePos.y < 0 || goalNodePos.x >= width || goalNodePos.y >= height)
        {
            Debug.LogError("❌ La posición del nodo objetivo está fuera de los límites.");
            return;
        }

        InitializeGrid();

        Debug.Log("Antes de llamar a BFS...");

        Node beginNode = nodeGrid[beginNodePos.y][beginNodePos.x];
        Node goalNode = nodeGrid[goalNodePos.y][goalNodePos.x];
        
        beginNode.isWalkable = true;
        goalNode.isWalkable = true;
        beginNode.parentRef = beginNode;

        bool BFSResult = BreadthFirstSearch(beginNode, goalNode);
        if (BFSResult)
        {
            Debug.Log("¡Sí hubo camino!");
            MarkPath(goalNode);
        }
        else
        {
            Debug.Log("No hubo camino");
        }
    }

    void InitializeGrid()
    {
        nodeGrid = new Node[height][];
        for (int y = 0; y < height; y++)
        {
            nodeGrid[y] = new Node[width];
            for (int x = 0; x < width; x++)
            {
                nodeGrid[y][x] = new Node(x, y);
                if (Random.value < 0.3f) // 30% de probabilidades de ser obstáculo
                {
                    nodeGrid[y][x].isWalkable = false;
                }
            }
        }

        // 🚀 Asegurar que el nodo de inicio y el objetivo siempre sean transitables:
        nodeGrid[beginNodePos.y][beginNodePos.x].isWalkable = true;
        nodeGrid[goalNodePos.y][goalNodePos.x].isWalkable = true;

        Debug.Log("Nodo grid inicializado");
    }

    bool BreadthFirstSearch(Node origin, Node goal)
    {
        Queue<Node> openNodes = new Queue<Node>();
        HashSet<Node> closedNodes = new HashSet<Node>(); 
        openNodes.Enqueue(origin);
        origin.parentRef = origin; 

        while (openNodes.Count > 0)
        {
            Node currentNode = openNodes.Dequeue();

            // 🚀 Verifica si llegamos al objetivo
            if (currentNode == goal)
            {
                return true;
            }

            int x = currentNode.x;
            int y = currentNode.y;

            // 🚀 Explorar los vecinos en 4 direcciones y asegurarse de que se agreguen correctamente
            if (y < height - 1) TryAddNode(nodeGrid[y + 1][x], currentNode, ref openNodes, ref closedNodes);
            if (x < width - 1) TryAddNode(nodeGrid[y][x + 1], currentNode, ref openNodes, ref closedNodes);
            if (y > 0) TryAddNode(nodeGrid[y - 1][x], currentNode, ref openNodes, ref closedNodes);
            if (x > 0) TryAddNode(nodeGrid[y][x - 1], currentNode, ref openNodes, ref closedNodes);
        }
        return false; // ❌ Si salimos del bucle sin encontrar el objetivo, no hay camino
    }

    bool TryAddNode(Node enqueuedNode, Node currentNode, ref Queue<Node> openNodes, ref HashSet<Node> closedNodes)
    {
        if (enqueuedNode.parentRef == null && enqueuedNode.isWalkable && !closedNodes.Contains(enqueuedNode))
        {
            enqueuedNode.parentRef = currentNode;
            openNodes.Enqueue(enqueuedNode);
            closedNodes.Add(enqueuedNode); // 🚀 Añadimos el nodo a los cerrados para evitar revisarlo de nuevo
            return true;
        }
        return false;
    }

    void MarkPath(Node goalNode)
    {
        Node current = goalNode;
        pathNodes.Clear(); // Limpia la lista de nodos de la ruta
        while (current.parentRef != current)
        {
            pathNodes.Insert(0, current); // Insertar al inicio para mantener el orden correcto
            current = current.parentRef;
        }
        pathNodes.Insert(0, current); // Agregar el nodo de inicio al inicio de la lista
    }

    private void OnDrawGizmos()
    {
        if (nodeGrid == null) return;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(x, y, 0.0f);

                if (nodeGrid[y][x].isWalkable)
                {
                    Gizmos.color = Color.white; // ⚪ Nodos transitables
                }
                else
                {
                    Gizmos.color = Color.black; // ⬛ Obstáculos ahora son negros
                }
                Gizmos.DrawCube(position, Vector3.one * 0.5f);
            }
        }

        // 🟩 Nodo de inicio (Verde)
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(beginNodePos.x, beginNodePos.y, 0.0f), 0.5f);

        // 🔴 Nodo objetivo (Rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(goalNodePos.x, goalNodePos.y, 0.0f), 0.5f);

        // 🔵 Camino BFS encontrado (Azul)
        Gizmos.color = Color.blue;
        foreach (Node pathNode in pathNodes)
        {
            Gizmos.DrawSphere(new Vector3(pathNode.x, pathNode.y, 0.0f), 0.3f);
        }
    }
}
