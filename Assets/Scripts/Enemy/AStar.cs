using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class AStar : MonoBehaviour
{
    [SerializeField][Tooltip("The Node prefab for visual aid.")]
    private GameObject nodePrefab;
    [SerializeField][Tooltip("Whether adjacent nodes will be instantiated.")]
    private bool nodeModeEnabled;
    
    private Transform nodeParent;
    private Tilemap platformTilemap;
    private LayerMask platformLayerMask;
    private Vector3 spawnPos;
    
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

    private void Awake() {
        platformLayerMask = LayerMask.GetMask("Platform");
        platformTilemap = GameObject.Find("Tilemap_Platform").GetComponent<Tilemap>();
        if (nodeModeEnabled) nodeParent = GameObject.Find("NodeParent").transform;
    }

    private void Update() {
        if (nodeModeEnabled) VisualizeNeighbors();
    }

    // Returns whether the position is walkable.
    private bool IsWalkable(Vector3Int coords) {
        bool isEmpty = !platformTilemap.HasTile(coords);
        
        coords.y -= 1;
        bool hasPlatformUnder = platformTilemap.HasTile(coords);
        return (isEmpty && hasPlatformUnder);
    }
    
    // Retrieves a list of neighbors to a given node position.
    private List<Node> GetNeighbors(Node n) {
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

        // Check right jump/drop positions
        int xOffset = 3;
        int yOffset = -6;
        for (int y = yOffset; y < 7; y++) {
            testPos = new Vector3Int(pos.x + xOffset, pos.y + y, 0);
            if (IsWalkable(testPos) && IsClear(pos, testPos)) {
                Node newNode;
                // Jump
                if (y >= 0 && !platformTilemap.HasTile(new Vector3Int(testPos.x - 1, testPos.y - 1, 0))) {
                    newNode = new Node(testPos, n, 3f);
                // Drop
                } else if (!platformTilemap.HasTile(new Vector3Int(pos.x + 1, pos.y - 1, 0))) {
                    newNode = new Node(testPos, n, 2f);
                } else {
                    continue;
                }
                neighbors.Add(newNode);
            }
        }
        xOffset = 4;
        yOffset = -5;
        for (int y = yOffset; y < 6; y++) {
            testPos = new Vector3Int(pos.x + xOffset, pos.y + y, 0);
            if (IsWalkable(testPos) && IsClear(pos, testPos)) {
                Node newNode;
                // Jump
                if (y >= 0 && !platformTilemap.HasTile(new Vector3Int(testPos.x - 1, testPos.y - 1, 0))) {
                    newNode = new Node(testPos, n, 3f);
                // Drop
                } else if (!platformTilemap.HasTile(new Vector3Int(pos.x + 1, pos.y - 1, 0))) {
                    newNode = new Node(testPos, n, 2f);
                } else {
                    continue;
                }
                neighbors.Add(newNode);
            }
        }
        xOffset = 5;
        yOffset = -4;
        for (int y = yOffset; y < 5; y++) {
            testPos = new Vector3Int(pos.x + xOffset, pos.y + y, 0);
            if (IsWalkable(testPos) && IsClear(pos, testPos)) {
                Node newNode;
                // Jump
                if (y >= 0 && !platformTilemap.HasTile(new Vector3Int(testPos.x - 1, testPos.y - 1, 0))) {
                    newNode = new Node(testPos, n, 3f);
                // Drop
                } else if (!platformTilemap.HasTile(new Vector3Int(pos.x + 1, pos.y - 1, 0))) {
                    newNode = new Node(testPos, n, 2f);
                } else {
                    continue;
                }
                neighbors.Add(newNode);
            }
        }

        // Check left jump/drop positions
        xOffset = -3;
        yOffset = -6;
        for (int y = yOffset; y < 7; y++) {
            testPos = new Vector3Int(pos.x + xOffset, pos.y + y, 0);
            if (IsWalkable(testPos) && IsClear(pos, testPos)) {
                Node newNode;
                // Jump
                if (y >= 0 && !platformTilemap.HasTile(new Vector3Int(testPos.x + 1, testPos.y - 1, 0))) {
                    newNode = new Node(testPos, n, 3f);
                // Drop
                } else if (!platformTilemap.HasTile(new Vector3Int(pos.x - 1, pos.y - 1, 0))) {
                    newNode = new Node(testPos, n, 2f);
                } else {
                    continue;
                }
                neighbors.Add(newNode);
            }
        }
        xOffset = -4;
        yOffset = -5;
        for (int y = yOffset; y < 6; y++) {
            testPos = new Vector3Int(pos.x + xOffset, pos.y + y, 0);
            if (IsWalkable(testPos) && IsClear(pos, testPos)) {
                Node newNode;
                // Jump
                if (y >= 0 && !platformTilemap.HasTile(new Vector3Int(testPos.x + 1, testPos.y - 1, 0))) {
                    newNode = new Node(testPos, n, 3f);
                // Drop
                } else if (!platformTilemap.HasTile(new Vector3Int(pos.x - 1, pos.y - 1, 0))) {
                    newNode = new Node(testPos, n, 2f);
                } else {
                    continue;
                }
                neighbors.Add(newNode);
            }
        }
        xOffset = -5;
        yOffset = -4;
        for (int y = yOffset; y < 5; y++) {
            testPos = new Vector3Int(pos.x + xOffset, pos.y + y, 0);
            if (IsWalkable(testPos) && IsClear(pos, testPos)) {
                Node newNode;
                // Jump
                if (y >= 0 && !platformTilemap.HasTile(new Vector3Int(testPos.x + 1, testPos.y - 1, 0))) {
                    newNode = new Node(testPos, n, 3f);
                // Drop
                } else if (!platformTilemap.HasTile(new Vector3Int(pos.x - 1, pos.y - 1, 0))) {
                    newNode = new Node(testPos, n, 2f);
                } else {
                    continue;
                }
                neighbors.Add(newNode);
            }
        }
        return neighbors;
    }

    // Returns whether there's a clear jump/drop path from pos to testPos.
    private bool IsClear(Vector3Int pos, Vector3Int testPos) {
        Vector2 abovePos = AdjustPos(new Vector3Int(pos.x, pos.y + 2, 0));
        Vector2 aboveTestPos = AdjustPos(new Vector3Int(testPos.x, testPos.y + 2, 0));
        Vector2 dir = (aboveTestPos - abovePos).normalized;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(abovePos, dir, CalcHeuristic(pos, testPos), platformLayerMask);
        return raycastHit2D.collider == null;
    }

    // An optimization would be have the nodes already constructed, and then identify which nodes the enemy/player lie on. Then traverse premade neighbors.
    // Try to construct path only if enemy and player is at a walkable platform.
    // In this way the enemy only goes to the player's last known location.
    // Calculates the path. If it exists, returns a list of node positions to follow.
    public List<Vector2> CalculatePath(Vector3 targetPos) {
        Vector3Int startPos = platformTilemap.WorldToCell(new Vector2(this.transform.position.x, this.transform.position.y - 1f));
        Vector3Int goalPos = platformTilemap.WorldToCell(new Vector2(targetPos.x, (targetPos.y - 1f)));

        if (IsWalkable(startPos) && !IsWalkable(goalPos)) {
            Vector3Int nearestNode = FindNearestWalkableNode(goalPos);
            if (nearestNode != Vector3.zero) goalPos = nearestNode;
        }

        if (IsWalkable(startPos) && IsWalkable(goalPos)) {
            List<Vector3Int> reached = new List<Vector3Int>();
            PriorityQueue<Node> frontier = new PriorityQueue<Node>();
            // Node: (position, parent Node, cost)
            Node startingNode = new Node(startPos, null, 0f);
            frontier.Enqueue(0f, startingNode);
            // Number of nodes we will traverse before we quit searching.
            int counter = 1000;

            while (frontier.Count() > 0 && counter > 0) {
                Node currNode = frontier.Dequeue();
                // If we found the player.
                if (platformTilemap.WorldToCell(currNode.GetPos()) == goalPos) {
                    List<Vector2> path = new List<Vector2>();
                    while (currNode.GetParent() != null) {
                        Vector2 adjustedPos = AdjustPos(currNode.GetPos());
                        path.Add(adjustedPos);
                        currNode = currNode.GetParent();
                    }
                    path.Reverse();
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
                counter -= 1;
            }
            //Debug.Log("Not found. Searched: " + counter + " nodes.");
        }
        return null;
    }

    // Calculates Heuristic Value for a given node using Euclidean Distance
    private float CalcHeuristic(Vector3Int pos, Vector3Int goalPos) {
        return Mathf.Sqrt(Mathf.Pow(pos.x - goalPos.x, 2) + Mathf.Pow(pos.y - goalPos.y, 2));
    }

    // Draw the Path created by CalculatePath()
    private void DrawPath(List<Vector2> path) {
        for (int i = path.Count - 1; i > 0; i--) {
            //Color color = (i % 2 == 0) ? Color.green : Color.red;
            Debug.DrawLine(path[i], path[i - 1], Color.green, 0.5f, false);
        }
    }

    // Converts the cell coords to the world position of the cell's center.
    private Vector2 AdjustPos(Vector3Int cellCoords) {
        Vector2 pos = (Vector2)platformTilemap.CellToWorld(cellCoords);
        pos.x += 0.5f;
        pos.y += 0.5f;
        return pos;
    }

    // Returns the two end positions of the patrol path.
    public List<Vector2> CalculatePatrolPath(int maxNodeDist) {
        List<Vector2> posList = new List<Vector2>();
        Vector3Int startPos = platformTilemap.WorldToCell(new Vector2(this.transform.position.x, this.transform.position.y - 1f));
        Vector3Int leftEnd = startPos;
        Vector3Int rightEnd = startPos;
        Vector3Int testPos;

        if (!IsWalkable(startPos)) return null;

        // Checking left
        int currDist = 0;
        testPos = startPos;
        while (IsWalkable(testPos) && currDist <= maxNodeDist) {
            leftEnd = testPos;
            testPos = new Vector3Int(testPos.x - 1, testPos.y, 0);
            currDist++;
        }
        posList.Add(AdjustPos(leftEnd));
        
        // Checking right
        currDist = 0;
        testPos = startPos;
        while (IsWalkable(testPos) && currDist <= maxNodeDist) {
            rightEnd = testPos;
            testPos = new Vector3Int(testPos.x + 1, testPos.y, 0);
            currDist++;
        }
        posList.Add(AdjustPos(rightEnd));

        return posList;
    }

    // Returns the enemy's adjusted position: The cell coordinate on top of the platform they are standing on.
    public Vector2 GetAdjustedPosition() {
        Vector3Int pos = platformTilemap.WorldToCell(new Vector2(this.transform.position.x, this.transform.position.y - 1f));
        return AdjustPos(pos);
    }

    // Returns 0 if the enemy is not stuck, -1 if they should move left, 1 if they should move right.
    public int Unstuck() {
        Vector3Int pos = platformTilemap.WorldToCell(new Vector2(this.transform.position.x, this.transform.position.y - 1f));
        if (IsWalkable(pos)) return 0;
        Vector3Int leftPos = new Vector3Int(pos.x - 1, pos.y, 0);
        Vector3Int rightPos = new Vector3Int(pos.x + 1, pos.y, 0);
        if (IsWalkable(leftPos)) return -1;
        if (IsWalkable(rightPos)) return 1;
        return 0;
    }

    // Returns whether the enemy has returned to its spawn position.
    public bool HasReturned(Vector3 spawnPos) {
        Vector3Int pos = platformTilemap.WorldToCell(new Vector2(this.transform.position.x, this.transform.position.y - 1f));
        Vector3Int spawn = platformTilemap.WorldToCell(new Vector2(spawnPos.x, spawnPos.y - 1f));
        return pos == spawn;
    }

    // Returns a vector pointing in the direction of the given angle (in degrees).
    public Vector2 GetVectorFromAngle(float angle) {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    private Vector3Int FindNearestWalkableNode(Vector3Int goalPos) {
        Vector2 goalPosWorld = AdjustPos(goalPos);
        Vector3Int testPos;
        int N = 20;
        int K = 1;
        while (true) {
            int k = K-1;
            for (int n = 0; n <= k; n++) {
                int x = -k + n; 
                int y = -n;
                // Vector2 testPosWorld = new Vector2(goalPosWorld.x + x, goalPosWorld.y + y);
                // GameObject.Instantiate(nodePrefab, testPosWorld, Quaternion.identity, nodeParent);
                testPos = new Vector3Int(goalPos.x + x, goalPos.y + y, 0);
                if (IsWalkable(testPos)) return testPos;
            }
            for (int n = 1; n <= k; n++) {
                int x = n; 
                int y = -k + n; 
                // Vector2 testPosWorld = new Vector2(goalPosWorld.x + x, goalPosWorld.y + y);
                // GameObject.Instantiate(nodePrefab, testPosWorld, Quaternion.identity, nodeParent);
                testPos = new Vector3Int(goalPos.x + x, goalPos.y + y, 0);
                if (IsWalkable(testPos)) return testPos;
            }
            for (int n = 1; n <= k; n++) {
                int x = k - n; 
                int y = n;
                // Vector2 testPosWorld = new Vector2(goalPosWorld.x + x, goalPosWorld.y + y);
                // GameObject.Instantiate(nodePrefab, testPosWorld, Quaternion.identity, nodeParent);
                testPos = new Vector3Int(goalPos.x + x, goalPos.y + y, 0);
                if (IsWalkable(testPos)) return testPos;
            }
            for (int n = 1; n <= k - 1; n++) {
                int x = -n; 
                int y = k - n;
                // Vector2 testPosWorld = new Vector2(goalPosWorld.x + x, goalPosWorld.y + y);
                // GameObject.Instantiate(nodePrefab, testPosWorld, Quaternion.identity, nodeParent);
                testPos = new Vector3Int(goalPos.x + x, goalPos.y + y, 0);
                if (IsWalkable(testPos)) return testPos;
            }
            K++;
            if(K > N/2) break;
        }
        return Vector3Int.zero;
    }

    // Instantiate node objects for neighbor nodes of current position. NodeMode must be enabled.
    public void VisualizeNeighbors() {
        foreach (Transform child in nodeParent) {
            GameObject.Destroy(child.gameObject);
        }
        Vector3Int startPos = platformTilemap.WorldToCell(new Vector2(this.transform.position.x, this.transform.position.y - 1f));
        GameObject startNodeObj = GameObject.Instantiate(nodePrefab, new Vector2(startPos.x + 0.5f, startPos.y + 0.5f), Quaternion.identity, nodeParent);
        startNodeObj.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 1f, 1f);
        Node startingNode = new Node(startPos, null, 0f);
        List<Node> neighbors = GetNeighbors(startingNode);

        foreach (Node node in neighbors) {
            Vector3Int p = node.GetPos();
            GameObject nodeObj = GameObject.Instantiate(nodePrefab, new Vector2(p.x + 0.5f, p.y + 0.5f), Quaternion.identity, nodeParent);
        }
    }
}
