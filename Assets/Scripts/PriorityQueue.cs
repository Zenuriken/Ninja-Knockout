using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<Node>
{
    #region Instance Variables
    private List<(float, Node)> myList;
    private int count;
    #endregion

    #region Instantiation
    public PriorityQueue() {
        this.myList = new List<(float, Node)>();
        this.count = 0;
    }
    #endregion

    #region Public Methods
    // Adds an element to the list with the given priority.
    public void Enqueue(float p, Node n) {
        this.myList.Add((p, n));
        this.count += 1;
    }

    // Removes the element with the highest priority and returns it.
    public Node Dequeue() {
        if (this.count == 0) {
            return default(Node);
        }
        (float, Node) minPair = this.myList[0];
        foreach ((float, Node) pair in myList) {
            if (pair.Item1 < minPair.Item1) {
                minPair = pair;
            }
        }
        this.myList.Remove(minPair);
        this.count -= 1;
        return minPair.Item2;
    }

    // Returns the number of elements in the PriorityQueue
    public int Count() {
        return this.count;
    }

    // Clears the priority queue
    public void Clear() {
        this.myList.Clear();
        this.count = 0;
    }
    #endregion


}
