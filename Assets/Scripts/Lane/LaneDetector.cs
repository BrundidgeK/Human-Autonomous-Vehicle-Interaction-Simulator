using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneDetector : MonoBehaviour
{
    public LayerMask layer;

    public Detector[] detectors;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < detectors.Length; i++)
            detectors[i] = Detect(detectors[i]);
    }

    Detector Detect(Detector d)
    {
        float angle = (d.angle - transform.rotation.eulerAngles.z) * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));

        Ray2D ray = new Ray2D(transform.position, dir); // Cast in the negative Y direction

        // Perform the raycast. This time, the hit result is returned directly
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, d.distance, layer);

        if (hit.collider != null) // Check if the raycast hit something
        {
            d.laneDetected = hit.collider.gameObject.GetComponent<LaneObject>();
            d.hitPoint = hit.point;
        }
        else
        {
            d.laneDetected = null;
            d.hitPoint = new Vector2(float.NaN, float.NaN);
        }

        // Draw a debug ray to visualize it in the scene view
        Debug.DrawRay(transform.position, dir*d.distance, Color.blue);
        return d;
    }
}

[Serializable]
public struct Detector
{
    public float distance;
    public float angle;

    public LaneObject laneDetected;
    public Vector2 hitPoint;
}
