using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{

    private Dictionary<Vector2Int, Waypoint> wayPointDic = new Dictionary<Vector2Int, Waypoint>();
    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
    private Queue<Waypoint> queue = new Queue<Waypoint>();
    private bool isFinished = false;
    private Coroutine findBFS;
    private Coroutine findASTAR;
    private Coroutine findDijkstra;
    private Coroutine findPath;

    public enum FindingMethod {
        BFS,
        Dijkstra,
        ASTAR,
    }



    [Header("===== 起点与终点 =====")]
    public GameObject startPoint;
    public GameObject endPoint;
    [Header("===== 寻路方式 =====")]
    public FindingMethod findingMethod;


    // Start is called before the first frame update

    private void Start()
    {
        //起始点，终点，路径字典都不为空时开始寻路
        if (findingMethod == FindingMethod.BFS)
        {
            findBFS = StartCoroutine(BFSFinding());
        }
        else if (findingMethod == FindingMethod.ASTAR)
        {
            findASTAR = StartCoroutine(ASTARFinding());
        }
        else if (findingMethod == FindingMethod.Dijkstra) {
            findDijkstra = StartCoroutine(DijkstraFinding());
        }
        findPath = StartCoroutine(generateList());
    }

    private void Update()
    {
        if (startPoint == null || endPoint == null) {
            SetPoint();
        }
    }

    private void SetPoint() {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Waypoint")))
            {
                if (startPoint == null)
                {
                    startPoint = hit.transform.gameObject;
                    startPoint.GetComponent<MeshRenderer>().material.color = Color.blue;
                }
                else
                {
                    endPoint = hit.transform.gameObject;
                    endPoint.GetComponent<MeshRenderer>().material.color = Color.red;
                }
            }
        }
    }

    public bool notNull() {
        return endPoint != null;
    }

    public bool Finished() {
        return isFinished;
    }


    IEnumerator BFSFinding()
    {
        yield return new WaitUntil(notNull);
        wayPointDic = MapManager.instance.wayPointDic;
        //将起始点加入队列
        queue.Enqueue(startPoint.GetComponent<Waypoint>());
        //startPoint.GetComponent<Waypoint>().cost_so_far = 0;

        //当队列不为空时开始处理每一个节点
        while (queue.Count > 0)
        {
            //节点出队
            var current_waypoint = queue.Dequeue();
            //若当前节点就是最终的endpoint结束循环
            if (current_waypoint == endPoint.GetComponent<Waypoint>())
            {
                //endPoint.GetComponent<MeshRenderer>().material.color = Color.red;
                isFinished = true;
                StopCoroutine(findBFS);
                break;
            }
            else
            {
                //查找当前节点上下左右的四个节点
                ExploreAround(current_waypoint);
                //将当前节点设置为已访问（isExplored）
                current_waypoint.isExplored = true;
            }
            //sortQueue();
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator DijkstraFinding() {
        yield return new WaitUntil(notNull);
        wayPointDic = MapManager.instance.wayPointDic;
        //将起始点加入队列
        queue.Enqueue(startPoint.GetComponent<Waypoint>());
        //startPoint.GetComponent<Waypoint>().cost_so_far = 0;

        //当队列不为空时开始处理每一个节点
        while (queue.Count > 0)
        {
            //节点出队
            var current_waypoint = queue.Dequeue();
            //若当前节点就是最终的endpoint结束循环
            if (current_waypoint == endPoint.GetComponent<Waypoint>())
            {
                isFinished = true;
                StopCoroutine(findDijkstra);
                break;
            }
            else
            {
                //查找当前节点上下左右的四个节点
                ExploreAround(current_waypoint);
                //将当前节点设置为已访问（isExplored）
                current_waypoint.isExplored = true;
            }
            sortQueue();
            yield return new WaitForSeconds(0.1f);
        }
    }



    IEnumerator ASTARFinding()
    {
        yield return new WaitUntil(notNull);
        wayPointDic = MapManager.instance.wayPointDic;
        //将起始点加入队列
        queue.Enqueue(startPoint.GetComponent<Waypoint>());
        //startPoint.GetComponent<Waypoint>().cost_so_far = 0;

        //当队列不为空时开始处理每一个节点
        while (queue.Count > 0)
        {
            //节点出队
            var current_waypoint = queue.Dequeue();
            //若当前节点就是最终的endpoint结束循环
            if (current_waypoint == endPoint.GetComponent<Waypoint>())
            {
                isFinished = true;
                StopCoroutine(findASTAR);
                break;
            }
            else
            {
                //查找当前节点上下左右的四个节点
                ExploreAround(current_waypoint);
                //将当前节点设置为已访问（isExplored）
                current_waypoint.isExplored = true;
            }
            sortQueue();
            yield return new WaitForSeconds(0.1f);
        }
    }

    //计算当前点与终点的曼哈顿距离
    private int Distance(Waypoint _current)
    {
        Waypoint end = endPoint.GetComponent<Waypoint>();
        return Mathf.Abs(_current.vector2Int.x - end.vector2Int.x) + Mathf.Abs(_current.vector2Int.y - end.vector2Int.y);
    }



    //AStar算法，根据当前点到终点的曼哈顿距离进行排序
    void sortQueue()
    {
        List<Waypoint> waypoints = new List<Waypoint>();
        while (queue.Count > 0)
        {
            waypoints.Add(queue.Dequeue());
        }
        if (findingMethod == FindingMethod.Dijkstra)
        {
            waypoints.Sort((x, y) => x.cost_so_far - y.cost_so_far);
        }
        else if (findingMethod == FindingMethod.ASTAR) {
            waypoints.Sort((x, y) => x.distance - y.distance);
        }

        foreach (var way in waypoints)
        {
            queue.Enqueue(way);
        }
    }

    void ExploreAround(Waypoint _current) {
        foreach (var direction in directions)
        {
            var exploreAround = _current.GetComponent<Waypoint>().getPosition() + direction;

            try
            {
                Waypoint neighbour = wayPointDic[exploreAround];


                if (findingMethod == FindingMethod.Dijkstra || findingMethod == FindingMethod.ASTAR) {
                    int next_cost = _current.cost_so_far + neighbour.cost;
                    if (next_cost <= neighbour.cost_so_far || neighbour.cost_so_far == 0)
                    {
                        neighbour.cost_so_far = next_cost;
                    }
                    neighbour.distance = Distance(neighbour) + neighbour.cost_so_far;
                }

                if (!neighbour.isExplored && !queue.Contains(neighbour) && !neighbour.isBrick)
                {
                    if (neighbour != endPoint.GetComponent<Waypoint>()) {
                        if (findingMethod == FindingMethod.Dijkstra || findingMethod == FindingMethod.ASTAR) {
                            neighbour.gameObject.GetComponent<MeshRenderer>().material.color = new Color(0,neighbour.cost/255f,neighbour.cost/255f);
                        }
                        else
                        {
                            neighbour.gameObject.GetComponent<MeshRenderer>().material.color = Color.cyan;
                        }
                       
                    }
                    neighbour.from = _current;
                    queue.Enqueue(neighbour);
                }
            }
            catch
            {
            }
        }
    }

    IEnumerator generateList() {
        yield return new WaitUntil(Finished);
        Waypoint prePoint = endPoint.GetComponent<Waypoint>().from;
        while (prePoint != startPoint.GetComponent<Waypoint>()) {
            prePoint.GetComponent<MeshRenderer>().material.color = Color.yellow;
            prePoint = prePoint.from;
            yield return new WaitForSeconds(0.1f);
        }
        StopCoroutine(findPath);
    }

}
