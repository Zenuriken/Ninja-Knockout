using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class AStar : MonoBehaviour
{
    [Tooltip("Where the AStar object is located.")]
    public Vector3 center;
    [Tooltip("The maximum number of nodes width-wise to create nodes for.")]
    public int width;
    [Tooltip("The maximum number of nodes height-wise to create nodes for.")]
    public int height;
    [Tooltip("The Node prefab for visual aid.")]
    public GameObject nodePrefab;
    [Tooltip("The platform tilemap.")]
    public Tilemap platformTilemap;
    [Tooltip("The target position of the path.")]
    public Transform target;
    [Tooltip("The starting position of the path.")]
    public Transform start;

    private LayerMask allPlatformsLayerMask;
    private Vector3Int offset = new Vector3Int(0, -1, 0);


    public class Node {
        private Vector3Int pos;
        private Node parent;
        private float cost;

        // Constructor for the Node class
        public Node(Vector3Int position, Node parent, float actionCost) {
            this.pos = position;
            this.parent = parent;
            this.cost += actionCost;
            if (parent != null) {
                this.cost += parent.GetCost();
            }
        }

        // Returns the cell position of the Node.
        public Vector3Int GetPos() {
            return this.pos;
        }

        // Returns the parent of the Node.
        public Node GetParent() {
            return this.parent;
        }

        // Returns the cumulative cost of getting to this node from ancestor nodes.
        public float GetCost() {
            return this.cost;
        }
    }

    // Draws the bounds of where pathfinding will take place.
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, new Vector2(width, height));
    }

    private void Awake() {
        allPlatformsLayerMask = LayerMask.GetMask("Platform", "OneWayPlatform");
    }

    private void Start() {
        float lowerX = center.x - width / 2f + 0.5f;
        float upperX = center.x + width / 2f - 0.5f;
        float lowerY = center.y - height / 2f + 0.5f;
        float upperY = center.y + height / 2f - 0.5f;

        for (float x = lowerX; x <= upperX; x+=1f) {
            for (float y = lowerY; y <= upperY; y+=1f) {
                Vector3Int coords = platformTilemap.WorldToCell(new Vector2(x, y));
                if (IsWalkable(coords)) {
                    GameObject node = GameObject.Instantiate(nodePrefab, new Vector2(x, y), Quaternion.identity);
                }
            }
        }

        InvokeRepeating("CalculatePath", 0f, 0.5f);
    }

    // Returns whether the position is walkable.
    private bool IsWalkable(Vector3Int coords) {
        bool isEmpty = !platformTilemap.HasTile(coords);
        
        coords.y -= 1;
        bool hasPlatformUnder = platformTilemap.HasTile(coords);
        return (isEmpty && hasPlatformUnder);
    }

    // Returns a list of walkable neighbor nodes to explore.
    public List<Node> GetNeighbors(Node n) {
        List<Node> neighbors = new List<Node>();
        Vector3Int pos = n.GetPos();
        Vector3Int testPos = Vector3Int.zero;

        // Check left
        testPos = new Vector3Int(pos.x - 1, pos.y, 0);
        if (IsWalkable(testPos)) {
            Node newNode = new Node(testPos, n, 1f);
            neighbors.Add(newNode);
        }

        // Check right
        testPos = new Vector3Int(pos.x + 1, pos.y, 0);
        if (IsWalkable(testPos)) {
            Node newNode = new Node(testPos, n, 1f);
            neighbors.Add(newNode);
        }

        // Check jump left
        testPos = new Vector3Int(pos.x - 2, pos.y + 4, 0);
        if (IsWalkable(testPos)) {
            Node newNode = new Node(testPos, n, 1f);
            neighbors.Add(newNode);
        }

        // Check jump right
        testPos = new Vector3Int(pos.x + 2, pos.y + 4, 0);
        if (IsWalkable(testPos)) {
            Node newNode = new Node(testPos, n, 1f);
            neighbors.Add(newNode);
        }

        // Check drop

        return neighbors;
    }


    // An optimization would be have the nodes already constructed, and then identify which nodes the enemy/player lie on. Then traverse premade neighbors.

    // Calculates the path. If it exists, returns a list of node positions to follow.
    private List<Vector3Int> CalculatePath() {

        // Try to construct path only if enemy and player is at a walkable platform.
        // In this way the enemy only goes to the player's last known location.
        Vector3Int startPos = platformTilemap.WorldToCell(new Vector2(start.position.x, start.position.y - 1f));
        Vector3Int goalPos = platformTilemap.WorldToCell(new Vector2(target.position.x, target.position.y - 1f));

        if (IsWalkable(startPos) && IsWalkable(goalPos)) {
            Vector3Int startCellPos = platformTilemap.WorldToCell(goalPos);
            Vector3Int goalCellPos = platformTilemap.WorldToCell(goalPos);
            List<Vector3Int> reached = new List<Vector3Int>();
            PriorityQueue<Node> frontier = new PriorityQueue<Node>();
            Node startingNode = new Node(startPos, null, 0f); // Node: (position, parent Node, cost)
            frontier.Enqueue(0f, startingNode);
            int counter = 0;
            while (frontier.Count() > 0 && counter < 1000) {
                Node currNode = frontier.Dequeue();
                Debug.Log("Frontier nodes: " + frontier.Count());
                // If we found the player.
                if (platformTilemap.WorldToCell(currNode.GetPos()) == goalCellPos) {
                    List<Vector3Int> path = new List<Vector3Int>();
                    while (currNode.GetParent() != null) {
                        path.Add(currNode.GetPos());
                        currNode = currNode.GetParent();
                    }
                    DrawPath(path);
                    return path;
                }
                
                // If we didn't find the player -> Search neighboring Nodes.
                if (!reached.Contains(currNode.GetPos())) {
                    reached.Add(currNode.GetPos());
                    List<Node> neighbors = GetNeighbors(currNode);
                    foreach (Node node in neighbors) {
                        float totalNodeCost = node.GetCost() + CalcHeuristic(node.GetPos(), goalPos);
                        frontier.Enqueue(totalNodeCost, node);
                    }
                }
                counter += 1;
            }
            Debug.Log("Not found. Expanded: " + counter);
        }
        return null;
    }

    // Calculates Heuristic Value for a given node using Euclidean Distance
    private float CalcHeuristic(Vector3Int pos, Vector3Int goalPos) {
        return Mathf.Sqrt(Mathf.Pow(pos.x - goalPos.x, 2) + Mathf.Pow(pos.y - goalPos.y, 2));
    }

    // Draw the Path created by CalculatePath()
    private void DrawPath(List<Vector3Int> path) {
        for (int i = path.Count - 1; i > 0; i--) {
            Vector2 p1 = platformTilemap.CellToWorld(path[i]);
            p1.x += 0.5f;
            p1.y += 0.5f;
            Vector2 p2 = platformTilemap.CellToWorld(path[i - 1]);
            p2.x += 0.5f;
            p2.y += 0.5f;
            Debug.DrawLine(p1, p2, Color.green, 0.5f, false);
        }
    }
}
