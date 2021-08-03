using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        var PathFinding = FindObjectOfType<PathFinding>();
        //����Э��
        //StartCoroutine(FindWayPoint(PathFinding.GetPath()));
    }

    IEnumerator FindWayPoint(List<Waypoint> pathwayPoints) {
        foreach (var waypoint in pathwayPoints)
        {
            transform.position = waypoint.transform.position + new Vector3(0, 1.0f, 0);
            //ÿ0.5��ִ��һ��
            yield return new WaitForSeconds(0.5f);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
