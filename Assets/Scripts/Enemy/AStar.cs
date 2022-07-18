using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class AStar : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Where the AStar object is located.")]
    private Vector3 center;
    [SerializeField]
    [Tooltip("The maximum number of nodes width-wise to create nodes for.")]
    private int width;
    [SerializeField]
    [Tooltip("The maximum number of nodes height-wise to create nodes for.")]
    private int height;
    [SerializeField]
    [Tooltip("The Node prefab for visual aid.")]
    private GameObject nodePrefab;
    [SerializeField]
    [Tooltip("The platform tilemap.")]
    private Tilemap platformTilemap;
    [SerializeField]
    [Tooltip("The target position of the path.")]
    private Transform target;
    [SerializeField]
    [Tooltip("The starting position of the path.")]
    private Transform start;

    private LayerMask allPlatformsLayerMask;

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

    // private void Start() {
    //     float lowerX = center.x - width / 2f + 0.5f;
    //     float upperX = center.x + width / 2f - 0.5f;
    //     float lowerY = center.y - height / 2f + 0.5f;
    //     float upperY = center.y + height / 2f - 0.5f;

    //     for (float x = lowerX; x <= upperX; x+=1f) {
    //         for (float y = lowerY; y <= upperY; y+=1f) {
    //             Vector3Int coords = platformTilemap.WorldToCell(new Vector2(x, y));
    //             if (IsWalkable(coords)) {
    //                 GameObject node = GameObject.Instantiate(nodePrefab, new Vector2(x, y), Quaternion.identity);
    //             }
    //         }
    //     }

    //     InvokeRepeating("CalculatePath", 0f, 0.5f);
    // }

    // Returns whether the position is walkable.
    private bool IsWalkable(Vector3Int coords) {
        bool isEmpty = !platformTilemap.HasTile(coords);
        
        coords.y -= 1;
        bool hasPlatformUnder = platformTilemap.HasTile(coords);
        return (isEmpty && hasPlatformUnder);
    }

    // Returns whether the path is clear from p0 to p1 for jumping or dropping not including p0 and p1
    private bool IsClear(Vector3Int p0, Vector3Int p1, bool isJump) {

        // For jumping:
        // [X][X][X][G]
        // [X][ ][X][-]
        // [X][ ][ ][ ]
        // [X][ ][ ][ ]
        // [X][ ][ ][ ]
        // [S][ ][ ][ ]

        // For dropping
        // [S][X][ ][ ]
        // [-][X][ ][ ]
        // [ ][X][ ][ ]
        // [ ][X][ ][ ]
        // [ ][X][ ][ ]
        // [ ][X][X][G]

        if (isJump) {
            bool clearUp = (!platformTilemap.HasTile(new Vector3Int(p0.x, p0.y + 1, 0)) && 
                            !platformTilemap.HasTile(new Vector3Int(p0.x, p0.y + 2, 0)) && 
                            !platformTilemap.HasTile(new Vector3Int(p0.x, p0.y + 3, 0)) &&
                            !platformTilemap.HasTile(new Vector3Int(p0.x, p0.y + 4, 0)) &&
                            !platformTilemap.HasTile(new Vector3Int(p0.x, p0.y + 5, 0)));

            // Jumping right
            if (p1.x > p0.x) {
                bool clearRight = (!platformTilemap.HasTile(new Vector3Int(p0.x + 1, p0.y + 5, 0)) &&
                                   !platformTilemap.HasTile(new Vector3Int(p0.x + 2, p0.y + 5, 0)) &&
                                   !platformTilemap.HasTile(new Vector3Int(p0.x + 2, p0.y + 4, 0)));
                return (clearUp && clearRight);
            }

            // Jumping left
            else {
                bool clearLeft = (!platformTilemap.HasTile(new Vector3Int(p0.x - 1, p0.y + 5, 0)) &&
                                  !platformTilemap.HasTile(new Vector3Int(p0.x - 2, p0.y + 5, 0)) &&
                                  !platformTilemap.HasTile(new Vector3Int(p0.x - 2, p0.y + 4, 0)));
                return (clearUp && clearLeft);
            }
        } else {
            // Dropping right
            if (p1.x > p0.x) {
                bool clearDownRight = (!platformTilemap.HasTile(new Vector3Int(p0.x + 1, p0.y, 0))     && 
                                       !platformTilemap.HasTile(new Vector3Int(p0.x + 1, p0.y - 1, 0)) && 
                                       !platformTilemap.HasTile(new Vector3Int(p0.x + 1, p0.y - 2, 0)) && 
                                       !platformTilemap.HasTile(new Vector3Int(p0.x + 1, p0.y - 3, 0)) &&
                                       !platformTilemap.HasTile(new Vector3Int(p0.x + 1, p0.y - 4, 0)) &&
                                       !platformTilemap.HasTile(new Vector3Int(p0.x + 1, p0.y - 5, 0)) &&
                                       !platformTilemap.HasTile(new Vector3Int(p0.x + 2, p0.y - 5, 0)));
                return clearDownRight;
            }

            // Dropping left
            else {
                bool clearDownLeft = (!platformTilemap.HasTile(new Vector3Int(p0.x - 1, p0.y, 0))     && 
                                      !platformTilemap.HasTile(new Vector3Int(p0.x - 1, p0.y - 1, 0)) && 
                                      !platformTilemap.HasTile(new Vector3Int(p0.x - 1, p0.y - 2, 0)) && 
                                      !platformTilemap.HasTile(new Vector3Int(p0.x - 1, p0.y - 3, 0)) &&
                                      !platformTilemap.HasTile(new Vector3Int(p0.x - 1, p0.y - 4, 0)) &&
                                      !platformTilemap.HasTile(new Vector3Int(p0.x - 1, p0.y - 5, 0)) &&
                                      !platformTilemap.HasTile(new Vector3Int(p0.x - 2, p0.y - 5, 0)));
                return clearDownLeft;
            }
        }
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
        testPos = new Vector3Int(pos.x - 3, pos.y + 5, 0);
        if (IsWalkable(testPos) && IsClear(pos, testPos, true)) {
            Node newNode = new Node(testPos, n, 3f);
            neighbors.Add(newNode);
        }

        // Check jump right
        testPos = new Vector3Int(pos.x + 3, pos.y + 5, 0);
        if (IsWalkable(testPos) && IsClear(pos, testPos, true)) {
            Node newNode = new Node(testPos, n, 3f);
            neighbors.Add(newNode);
        }

        // Check drop left
        testPos = new Vector3Int(pos.x - 3, pos.y - 5, 0);
        if (IsWalkable(testPos) && IsClear(pos, testPos, false)) {
            Node newNode = new Node(testPos, n, 2f);
            neighbors.Add(newNode);
        }

        // Check drop right
        testPos = new Vector3Int(pos.x + 3, pos.y - 5, 0);
        if (IsWalkable(testPos) && IsClear(pos, testPos, false)) {
            Node newNode = new Node(testPos, n, 2f);
            neighbors.Add(newNode);
        }

        return neighbors;
    }


    // An optimization would be have the nodes already constructed, and then identify which nodes the enemy/player lie on. Then traverse premade neighbors.
    // Try to construct path only if enemy and player is at a walkable platform.
    // In this way the enemy only goes to the player's last known location.
    // Calculates the path. If it exists, returns a list of node positions to follow.
    public List<Vector2> CalculatePath() {
        Vector3Int startPos = platformTilemap.WorldToCell(new Vector2(start.position.x, start.position.y - 1f));
        Vector3Int goalPos = platformTilemap.WorldToCell(new Vector2(target.position.x, target.position.y - 1f));

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
            //Debug.Log("Not found. Expanded: " + counter);
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

    // Calculates the position of two ends of a platform for the enemy to patrol
    public List<Vector2> CalculatePatrolPath(int maxNodeDist) {
        List<Vector2> posList = new List<Vector2>();
        Vector3Int startPos = platformTilemap.WorldToCell(new Vector2(start.position.x, start.position.y - 1f));
        Vector3Int leftEnd = startPos;
        Vector3Int rightEnd = startPos;
        Vector3Int testPos;
        if (IsWalkable(startPos)) {
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
        return null;
    }

    // Returns the enemy's adjusted position.
    public Vector2 GetAdjustedPosition() {
        Vector3Int pos = platformTilemap.WorldToCell(new Vector2(start.position.x, start.position.y - 1f));
        return AdjustPos(pos);
    }

    // Returns whether the enemy is currently stuck.
    public bool IsStuck() {
        Vector3Int pos = platformTilemap.WorldToCell(new Vector2(start.position.x, start.position.y - 1f));
        if (IsWalkable(pos)) {
            return false;
        }
        Vector3Int leftPos = new Vector3Int(pos.x - 1, pos.y, 0);
        Vector3Int rightPos = new Vector3Int(pos.x + 1, pos.y, 0);
        if (IsWalkable(leftPos) || IsWalkable(rightPos)) {
            return true;
        }
        return false;
    }

    // Returns the direction the enemy should move when stuck.
    public Vector2 GetMoveDir() {
        Vector3Int pos = platformTilemap.WorldToCell(new Vector2(start.position.x, start.position.y - 1f));
        Vector3Int leftPos = new Vector3Int(pos.x - 1, pos.y, 0);
        Vector3Int rightPos = new Vector3Int(pos.x + 1, pos.y, 0);
        if (IsWalkable(rightPos)) {
            return new Vector2(1f, 0);
        } else if (IsWalkable(leftPos)) {
            return new Vector2(-1f, 0);
        } else {
            Debug.Log("No walkable platforms found. Enemy stuck.");
            return Vector2.zero;
        }
    }


}
