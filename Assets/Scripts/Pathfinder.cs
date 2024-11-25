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
        DFS, BFS, DIJKSTRA, ASTAR
    }

    public SearchType searchType = SearchType.DFS;

    protected override void Awake()
    {
        base.Awake();
        // coursework states arg0 should be "nodes" not "nodes.Length
        GetComponent<GridRenderer>().Initialise(Map.MapWidth, Map.MapHeight, vGridUnitSize.x, vGridUnitSize.y);
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
            case SearchType.DFS:
                path = DepthFirstSearch(startNode, endNode);
                break;
            case SearchType.BFS:
                path = BreadthFirstSearch(startNode, endNode);
                break;
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

    public List<Node> DepthFirstSearch(Node startNode, Node endNode)
    {
        startNode.onOpenList = true;

        Stack<Node> nodesStack = new Stack<Node>();
        nodesStack.Push(startNode);

        int visitOrder = 0; // DEBUG CODE: Used to assign order a node has been seen for to node debugging purposes. Would not bbe used in production code

        while (nodesStack.Count > 0)
        {
            Node currentNode = nodesStack.Pop();

            Debug.Log(currentNode.x + ", " + currentNode.y);
            currentNode.visitOrder = visitOrder++;   // DEBUG CODE: Used to assign order a node has been seen for to node debugging purposes. Would not bbe used in production code

            if (currentNode == endNode)
            {
                return GetFoundPath(endNode);
            }

            Node[] neighbours = currentNode.neighbours;
            int neighboursCount = neighbours.Length;
            for (int neighbourIndex = 0; neighbourIndex < neighboursCount; ++neighbourIndex)
            {
                Node currentNeighbour = neighbours[neighbourIndex];
                if (!currentNeighbour.onOpenList)
                {
                    currentNeighbour.onOpenList = true;
                    currentNeighbour.parent = currentNode;

                    nodesStack.Push(currentNeighbour);
                }
            }
        }

        // No path has been found
        return GetFoundPath(null);
    }

    public List<Node> RecurvsiveDepthFirstSearch(Node startNode, Node endNode, int visitOrder = 0)
    {
        throw new NotImplementedException();
    }
    private List<Node> BreadthFirstSearch(Node startNode, Node endNode)
    {
        // almost direct copy of DFS except using Queue<T> rather than Stack<T> to ensure FIFO (First In First Out)

        startNode.onOpenList = true;

        Queue<Node> nodeQueue = new Queue<Node>();
        nodeQueue.Enqueue(startNode);

        int visitOrder = 0; // DEBUG CODE: Used to assign order a node has been seen for to node debugging purposes. Would not bbe used in production code

        while (nodeQueue.Count > 0)
        {
            Node currentNode = nodeQueue.Dequeue();

            Debug.Log(currentNode.x + ", " + currentNode.y);
            currentNode.visitOrder = visitOrder++;   // DEBUG CODE: Used to assign order a node has been seen for to node debugging purposes. Would not bbe used in production code

            if (currentNode == endNode)
            {
                return GetFoundPath(endNode);
            }

            Node[] neighbours = currentNode.neighbours;
            int neighboursCount = neighbours.Length;
            for (int neighbourIndex = 0; neighbourIndex < neighboursCount; ++neighbourIndex)
            {
                Node currentNeighbour = neighbours[neighbourIndex];
                if (!currentNeighbour.onOpenList)
                {
                    currentNeighbour.onOpenList = true;
                    currentNeighbour.parent = currentNode;

                    nodeQueue.Enqueue(currentNeighbour);
                }
            }
        }

        // No path has been found
        return GetFoundPath(null);
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
        throw new NotImplementedException();
    }

    private int ChebyshevDistanceHeuristic(int currentX, int currentY, int targetX, int targetY)
    {
        throw new NotImplementedException();
    }

    private int EuclideanDistanceHeuristic(int currentX, int currentY, int targetX, int targetY)
    {
        throw new NotImplementedException();
    }

    private int ManhattanDistanceHeuristic(int currentX, int currentY, int targetX, int targetY)
    {
        throw new NotImplementedException();
    }
}