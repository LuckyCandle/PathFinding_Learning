using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMap : MonoBehaviour
{
    public int numbers;
    public GameObject obstacle;
    // Start is called before the first frame update
    void Start()
    {
        numbers = Random.Range(0, 10);
        for (int i = 0; i < numbers; i++) {
            int x = Random.Range(-10, 10);
            int z = Random.Range(-10, 10);
            Instantiate(obstacle,new Vector3(x,0.5f,z),Quaternion.identity,gameObject.transform);
        }
    }
}
