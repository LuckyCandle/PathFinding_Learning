using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject waypoint_object;
    public Dictionary<Vector2Int, Waypoint> wayPointDic = new Dictionary<Vector2Int, Waypoint>();
    public int row; //ÐÐ
    public int col; //ÁÐ

    public static MapManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {
            if (instance != this) {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        MapGenerate();
    }

    private void MapGenerate() {
        for (int i = 0; i < col; i++)
        {
            for (int j = 0; j < row; j++)
            {
                Vector2Int vector2Int = new Vector2Int(i, j);
                GameObject wayObject = Instantiate(waypoint_object, new Vector3(i, 0, j), Quaternion.identity, transform);
                wayObject.name = "waypoint" + vector2Int + "";
                Waypoint waypoint = wayObject.GetComponent<Waypoint>();
                int num = Random.Range(0, 255);
                wayObject.GetComponent<MeshRenderer>().material.color = new Color(num / 255f, num / 255f, num / 255f);
                waypoint.cost = num;
                if (num % 5 == 0)
                {
                    waypoint.isBrick = true;
                }
                waypoint.vector2Int = vector2Int;
                wayPointDic.Add(vector2Int, waypoint);
            }
        }
    }

}
