using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    public static AStarGrid Instance { get; private set; }

    [Header("Grid")]
    public Vector2Int gridSize = new Vector2Int(20, 20);
    public float cellSize = 1f;
    public LayerMask obstacleMask;
    public bool drawGizmos = true;

    private Node[,] grid;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSize.x, gridSize.y];
        Vector3 origin = transform.position;

        float width = gridSize.x * cellSize;
        float height = gridSize.y * cellSize;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                float worldX = origin.x - width / 2f + x * cellSize + cellSize / 2f;
                float worldZ = origin.z - height / 2f + y * cellSize + cellSize / 2f;
                Vector3 worldPos = new Vector3(worldX, origin.y, worldZ);

                bool walkable = !Physics.CheckBox(
                    worldPos,
                    Vector3.one * (cellSize * 0.45f),
                    Quaternion.identity,
                    obstacleMask
                );

                grid[x, y] = new Node(x, y, walkable, worldPos);
            }
        }
    }

    public Vector3 GetNextStep(Vector3 fromWorld, Vector3 toWorld)
    {
        List<Node> path = FindPath(fromWorld, toWorld);
        if (path == null || path.Count == 0)
            return fromWorld;

        // path[0] é o nó de partida, path[1] é o primeiro passo
        if (path.Count > 1)
            return path[1].worldPos;

        return path[0].worldPos;
    }

    private List<Node> FindPath(Vector3 startWorld, Vector3 targetWorld)
    {
        Node startNode = WorldToNode(startWorld);
        Node targetNode = WorldToNode(targetWorld);

        if (startNode == null || targetNode == null)
            return null;

        if (!targetNode.walkable)
            return null;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        foreach (Node n in grid)
        {
            n.gCost = int.MaxValue;
            n.hCost = 0;
            n.parent = null;
        }

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||
                    (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newGCost = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (newGCost < neighbour.gCost)
                {
                    neighbour.gCost = newGCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != null && currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        if (currentNode == startNode)
            path.Add(startNode);

        path.Reverse();
        return path;
    }

    private int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.x - b.x);
        int dstY = Mathf.Abs(a.y - b.y);

        int min = Mathf.Min(dstX, dstY);
        int max = Mathf.Max(dstX, dstY);

        return 14 * min + 10 * (max - min); // diagonais + ortogonal
    }

    private Node WorldToNode(Vector3 worldPos)
    {
        float width = gridSize.x * cellSize;
        float height = gridSize.y * cellSize;

        float percentX = (worldPos.x - (transform.position.x - width / 2f)) / width;
        float percentY = (worldPos.z - (transform.position.z - height / 2f)) / height;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.Clamp(Mathf.RoundToInt((gridSize.x - 1) * percentX), 0, gridSize.x - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt((gridSize.y - 1) * percentY), 0, gridSize.y - 1);

        return grid[x, y];
    }

    private IEnumerable<Node> GetNeighbours(Node node)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.x + x;
                int checkY = node.y + y;

                if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                {
                    yield return grid[checkX, checkY];
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || grid == null)
            return;

        foreach (Node n in grid)
        {
            Gizmos.color = n.walkable ? Color.white : Color.black;
            Gizmos.DrawCube(n.worldPos, Vector3.one * (cellSize * 0.9f));
        }
    }

    // Classe interna do nó do A*
    private class Node
    {
        public int x, y;
        public bool walkable;
        public Vector3 worldPos;

        public int gCost;
        public int hCost;
        public int FCost => gCost + hCost;
        public Node parent;

        public Node(int x, int y, bool walkable, Vector3 worldPos)
        {
            this.x = x;
            this.y = y;
            this.walkable = walkable;
            this.worldPos = worldPos;
        }
    }
}
