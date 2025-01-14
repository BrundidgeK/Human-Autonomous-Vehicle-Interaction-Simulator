   using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictMovement : MonoBehaviour
{
    public enum movement { straight, lane_right, lane_left, turn_left, turn_right }
    public movement prev, current, future;

    public static float distance_BTW_lanes = 2;

    private SerialControl rotationRead;
    private float prevSteer;
    public LaneDetector player_detect;
    private Detector[] prevValues;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void changeMovement(movement m)
    {
        prev = current;
        current = future;
        future = m;
    }
    public void changeMovement(int m)
    {
        prev = current;
        current = future;
        future = (movement)m;
    }
}
