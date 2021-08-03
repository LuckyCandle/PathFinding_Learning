using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    void Update()
    {
        transform.position = new Vector3(MapManager.instance.col / 2, (MapManager.instance.col + MapManager.instance.row)/2, MapManager.instance.row / 2);
    }
}
