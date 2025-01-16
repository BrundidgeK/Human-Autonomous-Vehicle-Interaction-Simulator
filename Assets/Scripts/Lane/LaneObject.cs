using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneObject : MonoBehaviour
{

    public enum lane_type { straight, right_turn, left_turn}
    public lane_type lane;
    [HideInInspector]
    public int id;

    void Awake()
    {
        id = Random.Range(0, int.MaxValue);

        LineRenderer line = GetComponent<LineRenderer>();
        EdgeCollider2D edge = GetComponent<EdgeCollider2D>();
        line.positionCount = transform.childCount;
        edge.points = new Vector2[transform.childCount];

        List<Vector2> points = new List<Vector2>(); 
        for(int i = 0; i < line.positionCount; i++)
        {
            line.SetPosition(i, transform.GetChild(i).localPosition+transform.position);
            points.Add(transform.GetChild(i).localPosition);
        }

        edge.SetPoints(points);
    }
}