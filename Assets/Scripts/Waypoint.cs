using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public bool isExplored = false;
    public bool isBrick = false;
    public Waypoint from;
    public int cost;
    public int cost_so_far;
    public int distance;
    public Vector2Int vector2Int;
    // Start is called before the first frame update
    void Start()
    {
    }

    public Vector2Int getPosition() {
        return new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        if (isBrick)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = new Color(139/255f,117/255f,0);
        }
    }
}
