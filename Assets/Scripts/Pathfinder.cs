using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : PathfinderBase
{
    private Vector2Int vStartNodePos = Vector2Int.zero;
    private Vector2Int vTargetNodePos = Vector2Int.zero;
    private Vector2 vGridUnitSize = new Vector2(1080.0f, 1080.0f);  // ARBITRARY NUMBER???

    public enum SearchType
    {
        DIJKSTRA, ASTAR
    }

    public SearchType searchType = SearchType.ASTAR;

    protected override void Awake()
    {
        base.Awake();
        
        /*
         set start and end points to be ally and enemy
         agents house points respectively. currently these are set to
         v2.zero
         */

        ExecuteAlgorithm();
    }
    public void ExecuteAlgorithm()
    {
        // Clear any temporary data the nodes have from any previous execution of an algorithm
        foreach (var node in nodes)
        {
            node.Reset();
        }

        // Set start and end nodes
        Node startNode = nodes[vStartNodePos.x + (vNodeArea.x * vStartNodePos.y)];
        Node endNode = nodes[vTargetNodePos.x + (vNodeArea.x * vTargetNodePos.y)];

        List<Node> path = null;

        switch (searchType)
        {
            case SearchType.DIJKSTRA:
                path = Dijkstra(startNode, endNode);
                break;
            case SearchType.ASTAR:
                path = AStar(startNode, endNode);
                break;
        }

        // obsolete from pathfinding coursework
        // GetComponent<GridRenderer>().RenderPath(path);
    }

    public List<Node> Dijkstra(Node startNode, Node endNode)
    {
        //int min = 0x7FFFFFFF;	// initialise to i32  max to indicate unknown distance

        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        openList.Add(startNode);
        startNode.onOpenList = true;


        int visitOrder = 0; // DEBUG CODE: Used to assign order a node has been seen for to node debugging purposes. Would not bbe used in production code

        while (openList.Count > 0)
        {
            openList.Sort();
            Node currentNode = openList[0];
            openList.RemoveAt(0);

            closedList.Add(currentNode);
            currentNode.onClosedList = true;

            Debug.Log(currentNode.x + ", " + currentNode.y);
            currentNode.visitOrder = visitOrder++;   // DEBUG CODE: Used to assign order a node has been seen for to node debugging purposes. Would not bbe used in production code

            if (currentNode == endNode)
            {
                return GetFoundPath(endNode);
            }

            Node[] neighbours = currentNode.neighbours;

            for (int i = 0; i < neighbours.Length; ++i)
            {
                Node currentNeighbour = neighbours[i];
                if (currentNeighbour.onClosedList)
                {
                    continue;
                }
                /*
                if (currentNeighbour != null)
                {
                    if (currentNeighbour.x == currentNode.x)
                    {
                        // neighbour is immediately vertical
                        currentNeighbour.g += movementYCost;
                    }

                    if (currentNeighbour.y == currentNode.y)
                    {
                        // neighbour is immediately horizontal
                        currentNeighbour.g += movementXCost;
                    }
                    else
                    {
                        // neighbour must be diagonal
                        currentNeighbour.g += movementDiagonalCost;
                    }
                };*/
                int g = currentNode.g + currentNode.neighbourCosts[i];
                if (g < currentNeighbour.g || currentNeighbour.onOpenList == false)
                {
                    currentNeighbour.g = g;
                    currentNeighbour.f = g;
                    currentNeighbour.parent = currentNode;
                }


                if (!currentNeighbour.onOpenList)
                {
                    currentNeighbour.onOpenList = true;
                    openList.Add(currentNeighbour);
                }
            }
        }
        // No path has been found
        return GetFoundPath(null);
    }

    public List<Node> AStar(Node startNode, Node endNode)
    {
        if (startNode == endNode)
        {
            startNode.onClosedList = true;
            return new List<Node>() { startNode };
        }

        int visitOrder = 0; // DEBUG CODE: Used to assign order a node has been seen for to node derbugging purposes. Would not bbe used in production code

        // A binary heap is a more efficient data structure to sort nodes based on costs for A*
        // However you could use the sorted list that is commented out instead
        BinaryHeap<Node> openList = new BinaryHeap<Node>(nodes.Length);

        //List<Node> openList = new List<Node>(nodes.Length);

        openList.Add(startNode);
        Node currentNode;

        while (openList.Count > 0)
        {
            // For the binary heap implementation uncomment this and comment/remove the 3 lines after.
            currentNode = openList.Remove();

            //openList.Sort();
            //currentNode = openList[0];
            //openList.RemoveAt(0);

            // Mark as being on closed list as by removing from the open list it means the shortest route has been found to
            // this node. By using a boolean variable to signify this we can avoid using another list which can save memory
            // NOTE: technically currentNode.onOpenList should be set to false here for complete correctness has it has
            // been removed from the open list, but we don't have to do this as checks for the closed list happen first in
            // the algorithm when considering neighbours
            currentNode.onClosedList = true;

            currentNode.visitOrder = visitOrder++; // DEBUG CODE: Used to assign order a node has been seen for to node derbugging purposes. Would not bbe used in production code

            // If this node is the end node then the path has been found and we just need to return it
            if (currentNode == endNode)
            {
                return GetFoundPath(endNode);
            }

            // Need to determine which neighbours can be added
            Node[] neighbours = currentNode.neighbours;
            int neighboursCount = neighbours.Length;
            for (int connectedNodesIndex = 0; connectedNodesIndex < neighboursCount; ++connectedNodesIndex)
            {
                Node currentNeighbour = neighbours[connectedNodesIndex];

                // If this node is on the closed list then skip as we have already processed it
                if (currentNeighbour.onClosedList)
                {
                    continue;
                }

                // Calculate costs
                int gCost = currentNode.g + currentNode.neighbourCosts[connectedNodesIndex];

                // Uncomment/Comment the heuristics to see the differences
                //int hCost = ChebyshevDistanceHeuristic(connectedNode.x, connectedNode.y, endNode.x, endNode.y);
                int hCost = EuclideanDistanceHeuristic(currentNeighbour.x, currentNeighbour.y, endNode.x, endNode.y);
                //int hCost = ManhattanDistanceHeuristic(connectedNode.x, connectedNode.y, endNode.x, endNode.y);

                int fCost = gCost + hCost;

                // If this connected node has a lower overall cost or its the first time we have seen it (since its
                // not on the open list), then set the values of it.
                if (fCost <= currentNeighbour.f || !currentNeighbour.onOpenList)
                {
                    currentNeighbour.g = gCost;
                    currentNeighbour.h = hCost;
                    currentNeighbour.f = fCost;
                    currentNeighbour.parent = currentNode;
                }

                // Finally if this node is not on the open list add it and mark it as added so we can check for this
                // in the future. Note this could have been combined with the above code but to keep the example
                // simple it has been left like this for readibility.
                if (!currentNeighbour.onOpenList)
                {
                    currentNeighbour.onOpenList = true;
                    openList.Add(currentNeighbour);
                }
            }
        }

        return GetFoundPath(null);
    }

    private int EuclideanDistanceHeuristic(int currentX, int currentY, int targetX, int targetY)
    {
        // messy; return a straight-line integer cost from currentPos to targetPos
        return (int)new Vector2 ((currentX - targetX) * vMovementCost.x, (currentY - targetY) * vMovementCost.y).magnitude;

    }
}