using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class PathFinding : MonoBehaviour
{
    //public Transform seeker, target;
    private Grid grid;
    private PathManager requestManager;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        requestManager =GetComponent<PathManager>();
    }


    //private void Update()
    //{
    //    if (Input.GetButtonDown("Jump"))
    //    {
    //        FindPath(seeker.position, target.position);
    //    }
        
    //}


    public void StartFindPath(Vector3 startPos,Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator   FindPath(Vector3 startPos,Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        //sw.Start();

        Vector3[] wayPoints = new Vector3[0];
        bool pathSuccess = false;

        
        Node startNode = grid.NodeFromWorldPoint(startPos);//起点
        Node targetNode = grid.NodeFromWorldPoint(targetPos);//终点


        //起点与终点是否为障碍
        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closeSet = new HashSet<Node>();

            openSet.Add(startNode);

            //
            while (openSet.Count > 0)
            {

                Node currentNode = openSet.RemoveFirst();
                closeSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    //sw.Stop();
                    //print("时间:" + sw.ElapsedMilliseconds + "ms");
                    pathSuccess = true;
                    break;
                }


                foreach (Node neighbour in grid.GetNeighboursNodes(currentNode))
                {
                    if (!neighbour.walkable || closeSet.Contains(neighbour)) continue;

                    int tempCostToNei = currentNode.gCosh + CalculateDistance(currentNode, neighbour);
                    if (tempCostToNei < neighbour.gCosh || !openSet.Contains(neighbour))
                    {
                        neighbour.parent = currentNode;
                        neighbour.gCosh = tempCostToNei;
                        neighbour.hCost = CalculateDistance(neighbour, targetNode);
                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }

                }

            }
        }
       
        
        yield return null;

        if (pathSuccess)
        {
            wayPoints = GetPath(startNode, targetNode);
        }
        requestManager.FinishProcessingPath(wayPoints, pathSuccess);
    }


    //获取路径
    private Vector3[] GetPath(Node startNode,Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;     
    }

    //简化路径
    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;
        for(int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    //计算两点之间的距离
    private int CalculateDistance(Node fromNode, Node toNode)
    {
        int xDistance = Mathf.Abs(fromNode.gridX-toNode.gridX);
        int yDistance = Mathf.Abs(fromNode.gridY-toNode.gridY);

        //if (xDistance > yDistance)
        //    return 14 * yDistance + 10 * (xDistance - yDistance);
        //return 14 * xDistance + 10*(yDistance - xDistance);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return 14 * Mathf.Min(xDistance, yDistance) + 10 * remaining;
    }
}
