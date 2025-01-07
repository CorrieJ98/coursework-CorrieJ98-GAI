/*
 Content adapted from Hamids Pathfinding Game AI lesson
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathfinderBase : MonoBehaviour
{
    //<summary>x moveCost, y moveCost, diagonal moveCost</summary>
    [SerializeField] protected Vector3Int vMovementCost = new Vector3Int(10,10,14);

    #region obsolete legacy code
    //[SerializeField] protected int movementCostX = 10;
    //[SerializeField] protected int movementCostY = 10;
    //[SerializeField] protected int movementCostDiag = 14;

    // protected const int kNodesWidth = 100;
    // protected const int kNodesHeight = 100;
    #endregion

    protected int movementDiagMinusXY;

    protected byte[] initialMapData;    // call GetMapData() from Map.cs to populate this array
    protected Node[] nodes;

    // Map.cs map width & height are 100. DO NOT CHANGE
    protected Vector2Int vNodeArea = new Vector2Int(100,100);


    protected virtual void Awake()
    {
        initialMapData = GameData.Instance.Map.GetMapData();

        movementDiagMinusXY = vMovementCost.z - (vMovementCost.x + vMovementCost.y);
        CreateNodes();
        CreateNodeConnections();
    }

    private void CreateNodes()
    {
        nodes = new Node[vNodeArea.x * vNodeArea.y];

        for (int y = 0; y < vNodeArea.y; ++y)
        {
            for (int x = 0; x < vNodeArea.x; ++x)
            {
                Node node = new Node();
                node.x = x;
                node.y = y;
                nodes[x + (vNodeArea.x * y)] = node;
            }
        }
    }

    private void CreateNodeConnections()
    {
        for (int nodeY = 0; nodeY < vNodeArea.y; ++nodeY)
        {
            for (int nodeX = 0; nodeX < vNodeArea.x; ++nodeX)
            {
                int nodeIndex = nodeX + (vNodeArea.x * nodeY);
                Node node = nodes[nodeIndex];

                if (initialMapData[nodeIndex] > 0)
                {
                    node.neighbours = new Node[0];
                    node.neighbourCosts = new int[0];
                    continue;
                }

                int connectedNodesCount = 0;
                for (int neighbourY = nodeY - 1; neighbourY <= nodeY + 1; ++neighbourY)
                {
                    if (neighbourY < 0 || neighbourY >= vNodeArea.y)
                    {
                        continue;
                    }

                    for (int neighbourX = nodeX - 1; neighbourX <= nodeX + 1; ++neighbourX)
                    {
                        if (neighbourX < 0 ||
                            neighbourX >= vNodeArea.x ||
                            (neighbourX == nodeX && neighbourY == nodeY) ||
                            initialMapData[neighbourX + (neighbourY * vNodeArea.x)] > 0)
                        {
                            continue;
                        }

                        ++connectedNodesCount;
                    }
                }

                node.neighbours = new Node[connectedNodesCount];
                node.neighbourCosts = new int[connectedNodesCount];

                int connectedNodesIndex = 0;
                for (int neighbourY = nodeY - 1; neighbourY <= nodeY + 1; ++neighbourY)
                {
                    if (neighbourY < 0 || neighbourY >= vNodeArea.y)
                    {
                        continue;
                    }

                    for (int neighbourX = nodeX - 1; neighbourX <= nodeX + 1; ++neighbourX)
                    {
                        if (neighbourX < 0 ||
                            neighbourX >= vNodeArea.x ||
                            (neighbourX == nodeX && neighbourY == nodeY) ||
                            initialMapData[neighbourX + (neighbourY * vNodeArea.x)] > 0)
                        {
                            continue;
                        }

                        node.neighbours[connectedNodesIndex] = nodes[neighbourX + (neighbourY * vNodeArea.x)];
                        node.neighbourCosts[connectedNodesIndex] = CalculateInitialCost(nodeX, nodeY, neighbourX, neighbourY);
                        ++connectedNodesIndex;
                    }
                }
            }
        }
    }

    protected int CalculateInitialCost(int firstNodeX, int firstNodeY, int secondNodeX, int secondNodeY)
    {
        int xCost = Mathf.Abs(secondNodeX - firstNodeX);
        int yCost = Mathf.Abs(secondNodeY - firstNodeY);
        if ((xCost + yCost) < 2)
        {
            if (xCost > 0)
            {
                return vMovementCost.x;
            }
            return vMovementCost.y;
        }
        return vMovementCost.z;
    }

    protected List<Node> GetFoundPath(Node endNode)
    {
        List<Node> foundPath = new List<Node>();
        if (endNode != null)
        {
            foundPath.Add(endNode);

            while (endNode.parent != null)
            {
                foundPath.Add(endNode.parent);
                endNode = endNode.parent;
            }

            // Reverse the path so the start node is at index 0
            foundPath.Reverse();
        }
        return foundPath;
    }
}
