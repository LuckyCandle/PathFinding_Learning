using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("===== ��ͼ��Ϣ =====")]
    public GameObject tilePrefab; //��Ƭ��ͼ��prefab
    public Vector2 mapSize; //�����ͼ�����������Ƭ��
    public Transform mapHolder;
    [Range(0, 1)]
    public float outlinePercent;

    [Header("===== �ϰ�����Ϣ =====")]
    public GameObject obsPrefab; //�ϰ����prefab
    [Range(0,1)]
    public float obsPercent; //�ϰ���ٷֱ�

    public List<Vector2Int> TileCoordList = new List<Vector2Int>();

    public Color foreground, background;
    public float minHeight, maxHeight;

    private Queue<Vector2Int> TileCoordQueue;
    private Vector2Int mapCenter; //�κ������ͼ�����ĵ�
    bool[,] mapObstacles;              //��άbool���飬�ж�ĳ����(x,y)���Ƿ����ϰ���

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
        mapCenter = new Vector2Int((int)(mapSize.x / 2), (int)(mapSize.y / 2));  //���õ�ͼ���ĵ�
        mapObstacles = new bool[(int)mapSize.x, (int)mapSize.y];                      //��ʼ����ά��������

        //ϴ���㷨ShuffleCoords���Ѿ��õ��������
        TileCoordQueue = new Queue<Vector2Int>(Utility.ShuffleCoords(TileCoordList));

        //Ĭ�ϵ�ǰ�ϰ�������Ϊ0
        int currentObsCount = 0;

        for (int i = 0; i < obsCount; i++)
        {
            //�򵥵�Random��������������ų�ÿһ��������ֵ����кţ���������ǳ����ɿ���
            //Vector2Int randomCoord = TileCoordList[UnityEngine.Random.Range(0,TileCoordList.Count)]; 

            //����ϴ���㷨�õ�ĳ�����԰ڷ�Obstacle�ĵ�
            Vector2Int randomCoord = TileCoordQueue.Dequeue();
            TileCoordQueue.Enqueue(randomCoord);


            //��ˮ����㷨��ʼ�������������������ϰ���(���ƻ����㷨)
            mapObstacles[randomCoord.x, randomCoord.y] = true;     
            currentObsCount++;

            if (randomCoord != mapCenter && MapIsFullyAccessible(mapObstacles, currentObsCount))
            {
                //����߶�
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

    //��ˮ����㷨
    private bool MapIsFullyAccessible(bool[,] _mapObstacles, int _currentObsCount)
    {
        bool[,] mapFlags = new bool[_mapObstacles.GetLength(0), _mapObstacles.GetLength(1)]; //�����mapFlag�����������㷨�е�visited����
        Queue<Vector2Int> queue = new Queue<Vector2Int>(); //���е��������ɸѡ��洢�����������
        queue.Enqueue(mapCenter);
        mapFlags[mapCenter.x, mapCenter.y] = true; //���ĵ���Ϊ�Ѽ��

        int accessibleCount = 1; //��ʼ����ǰʵ�ʿ��ߵ���Ƭ���������Ŀ��ߣ��������Ϊ1��
        while (queue.Count > 0) {
            Vector2Int currentTile = queue.Dequeue();
            foreach (var item in directions) //�������������˳����б�������
            {
                var explore = currentTile + item;
                int x = explore.x;
                int y = explore.y;
                //�жϱ߽�
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
