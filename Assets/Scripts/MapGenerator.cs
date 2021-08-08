using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("===== 地图信息 =====")]
    public GameObject tilePrefab; //瓦片地图的prefab
    public Vector2 mapSize; //保存地图横向和纵向瓦片数
    public Transform mapHolder;
    [Range(0, 1)]
    public float outlinePercent;

    [Header("===== 障碍物信息 =====")]
    public GameObject obsPrefab; //障碍物的prefab
    [Range(0,1)]
    public float obsPercent; //障碍物百分比

    public List<Vector2Int> TileCoordList = new List<Vector2Int>();

    public Color foreground, background;
    public float minHeight, maxHeight;

    private Queue<Vector2Int> TileCoordQueue;
    private Vector2Int mapCenter; //任何随机地图的中心点
    bool[,] mapObstacles;              //二维bool数组，判断某个点(x,y)上是否有障碍物

    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        MapTilesGenerate();
        ObstacleGenerate();
    }

    private void MapTilesGenerate() {
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                Vector3 newPos = new Vector3(-mapSize.x / 2 + 0.5f + i, 0, -mapSize.y / 2 + 0.5f + j);
                GameObject mapTile = Instantiate(tilePrefab, newPos, Quaternion.Euler(90, 0, 0), mapHolder);
                mapTile.transform.localScale *= (1 - outlinePercent);

                TileCoordList.Add(new Vector2Int(i, j));
            }
        }
    }

    private void ObstacleGenerate() {
        int obsCount = (int)(mapSize.x * mapSize.y * obsPercent);
        mapCenter = new Vector2Int((int)(mapSize.x / 2), (int)(mapSize.y / 2));  //设置地图中心点
        mapObstacles = new bool[(int)mapSize.x, (int)mapSize.y];                      //初始化二维布尔数组

        //洗牌算法ShuffleCoords，已经得到乱序队列
        TileCoordQueue = new Queue<Vector2Int>(Utility.ShuffleCoords(TileCoordList));

        //默认当前障碍物数量为0
        int currentObsCount = 0;

        for (int i = 0; i < obsCount; i++)
        {
            //简单的Random随机方法并不会排除每一轮随机出现的序列号，导致随机非常不可控制
            //Vector2Int randomCoord = TileCoordList[UnityEngine.Random.Range(0,TileCoordList.Count)]; 

            //根据洗牌算法得到某个可以摆放Obstacle的点
            Vector2Int randomCoord = TileCoordQueue.Dequeue();
            TileCoordQueue.Enqueue(randomCoord);


            //洪水填充算法开始：满足条件才能生成障碍物(类似回溯算法)
            mapObstacles[randomCoord.x, randomCoord.y] = true;     
            currentObsCount++;

            if (randomCoord != mapCenter && MapIsFullyAccessible(mapObstacles, currentObsCount))
            {
                //随机高度
                float obsHeight = Mathf.Lerp(minHeight, maxHeight, UnityEngine.Random.Range(0f, 1f));

                Vector3 newPos = new Vector3(-mapSize.x / 2 + 0.5f + randomCoord.x, obsHeight / 2, -mapSize.y / 2 + 0.5f + randomCoord.y);
                GameObject obstacleTile = Instantiate(obsPrefab, newPos, Quaternion.identity, mapHolder);
                obstacleTile.transform.localScale = new Vector3(1 - outlinePercent, obsHeight, 1 - outlinePercent);

                float colorPercent = randomCoord.y / mapSize.y;
                obstacleTile.GetComponent<MeshRenderer>().material.color = Color.Lerp(foreground, background, colorPercent);
            }
            else {
                mapObstacles[randomCoord.x, randomCoord.y] = false;
                currentObsCount--;
            }

        }
    }

    //洪水填充算法
    private bool MapIsFullyAccessible(bool[,] _mapObstacles, int _currentObsCount)
    {
        bool[,] mapFlags = new bool[_mapObstacles.GetLength(0), _mapObstacles.GetLength(1)]; //这里的mapFlag类似于搜索算法中的visited数组
        Queue<Vector2Int> queue = new Queue<Vector2Int>(); //所有的坐标会在筛选后存储到这个队列中
        queue.Enqueue(mapCenter);
        mapFlags[mapCenter.x, mapCenter.y] = true; //中心点标记为已检测

        int accessibleCount = 1; //初始化当前实际可走的瓦片数量（中心可走，因此现在为1）
        while (queue.Count > 0) {
            Vector2Int currentTile = queue.Dequeue();
            foreach (var item in directions) //按照上右下左的顺序进行遍历搜索
            {
                var explore = currentTile + item;
                int x = explore.x;
                int y = explore.y;
                //判断边界
                if (x >= 0 && x < _mapObstacles.GetLength(0) && y >= 0 && y < mapObstacles.GetLength(1)) {
                    if (!mapFlags[x, y] && !_mapObstacles[x, y]) {
                        mapFlags[x, y] = true;
                        accessibleCount++;
                        queue.Enqueue(explore);
                    }
                }
            }
        }
        return accessibleCount == (int)(mapSize.x*mapSize.y - _currentObsCount);
    }
}
