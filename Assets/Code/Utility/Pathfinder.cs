using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Node
{
    public int x;
    public int y;
    public bool pathable;
    public Vector3 worldPoint;

    public int gCost;
    public int hCost;
    public Node parent;

    public Node (int _x, int _y, bool _pathable, Vector3 _worldPoint)
    {
        x = _x;
        y = _y;
        pathable = _pathable;
        worldPoint = _worldPoint;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}

public class PathRequest
{
    public Transform requester;
    public InputController requesterController;
    public Vector3 startPosition;
    public Vector3 targetPosition;

    public PathRequest(Transform _requester, InputController _requesterController, Vector3 _startPosition, Vector3 _targetPosition)
    {
        requester = _requester;
        requesterController = _requesterController;
        startPosition = _startPosition;
        targetPosition = _targetPosition;
    }
}

public class Pathfinder : MonoBehaviour
{
    // Start is called before the first frame update

    int gridWorldSize;
    public Node[,] nodeGrid;
    float nodeDiameter = 0.3333f;
    float nodeRadius = 0.3333f/2;

    List<Node> generatedPath;

    public List<PathRequest> PathfindQueue;
    float lastTimePathChecked;

    public int count;

    void Awake()
    {
        gridWorldSize = 30;
        generate_grid(gridWorldSize * 3);
        PathfindQueue = new List<PathRequest>();
    }

    // Update is called once per frame
    void Update()
    {
        executeFindPathQueue();
    }

    public void generate_grid(int size)
    {
        nodeGrid = new Node[size, size];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize / 2 - Vector3.forward * gridWorldSize / 2;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = true;

                if(Physics.CheckSphere(worldPoint, nodeRadius - 0.01f, LayerMask.GetMask("WallLayer")))
                {
                    walkable = false;
                }

                nodeGrid[x, y] = new Node(x, y, walkable, worldPoint);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize / 2) / gridWorldSize;
        float percentY = (worldPosition.z + gridWorldSize / 2) / gridWorldSize;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridWorldSize * 3 - 1) * percentX);
        int y = Mathf.RoundToInt((gridWorldSize * 3 - 1) * percentY);
        return nodeGrid[x, y];
    }


    public void get_random_patrol_near_anchor(Vector3 anchorPos, int range)
    {

    }

    public void executeFindPathQueue()
    {
        if (Time.fixedTime - lastTimePathChecked > 0.05 && PathfindQueue.Count > 0)
        {
            var request = PathfindQueue.First();

            var path = Get_path(request.startPosition, request.targetPosition);

            generatedPath = path; //debug

            List<Vector3> pathVectors = new List<Vector3>();

            //Debug.Log(path.ToString());

            if(generatedPath != null)
            {
                foreach (Node n in path)
                {

                    Vector3 vector = new Vector3(n.worldPoint.x, 0, n.worldPoint.z);
                    pathVectors.Add(vector);

                }
            }

            request.requesterController.currPath = pathVectors;
            request.requesterController.requestingPath = false;
            PathfindQueue.Remove(request);
            lastTimePathChecked = Time.time;

            count = PathfindQueue.Count;
        }
    }

    List<Node> Get_path(Vector3 currentPos, Vector3 targetPos)
    {
        Node startNode = NodeFromWorldPoint(currentPos);
        Node targetNode = NodeFromWorldPoint(targetPos);

        if(targetNode.pathable == true)
        {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (Node neighbour in GetNeighbours(currentNode))
                {
                    if (!neighbour.pathable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }
        return null;
    }

    List<Node> RetracePath(Node startNode, Node endnode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endnode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        return  path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        else
        {
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.x + x;
                int checkY = node.y + y;

                if (checkX >= 0 && checkX < gridWorldSize * 3 && checkY >= 0 && checkY < gridWorldSize * 3)
                {
                    neighbours.Add(nodeGrid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    void OnDrawGizmos()
    {
        if(nodeGrid != null)
        {
            foreach (Node n in nodeGrid)
            {
                Gizmos.color = (n.pathable) ? Color.clear : Color.red;

                if (generatedPath != null)
                {
                    if (generatedPath.Contains(n))
                    {
                        if (n == generatedPath[0])
                        {
                            Gizmos.color = Color.magenta;
                        }
                        else
                            Gizmos.color = Color.yellow;
                    }
                }

                //if (startNode == n)
                //{
                //    Gizmos.color = Color.green;
                //}
                //if (endNode == n)
                //{
                //    Gizmos.color = Color.red;
                //}

                //if (playerNode == n)
                //{
                //    Gizmos.color = Color.cyan;
                //}

                Gizmos.DrawCube(n.worldPoint, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}
