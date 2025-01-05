using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataCollector : MonoBehaviour
{
    public PredictMovement.movement current;

    public static float distance_BTW_lanes = 3;

    private SerialControl rotationRead;
    public LaneDetector player_detect;
    private double prevLeft, prevRight, prevFront;
    public Transform car;

    // Path to store training data
    private string filePath;

    /*
     * Input Layer:
     * distance from left lane
     * distance from right lane
     * kind of lane (0-straight, -1-left turn, 1-right turn)
     * change distance in front of car
     * wheel angle / 180
    */

    void Start()
    {
        // Initialize the file path to save data
        filePath = Application.dataPath + "/TrainingData.csv";

        // Create the file and add the header if it doesn't exist
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "LeftChange,RightChange,LaneType,FrontChange,WheelAngle,ExpectedOutput\n");
        }
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
            writeToFile();
    }

    private void writeToFile()
    {
        string expectedOut = expectOut();
        if (expectedOut == "0,0,0,0,0")
        {
            Debug.LogError("Pick an expected output");
            return;
        }

        // Calculate inputs
        Vector2 left_hit = player_detect.detectors[2].hitPoint;
        double distanceFromLeft = Mathf.Cos(Mathf.Atan2(left_hit.y - car.position.y, left_hit.x - car.position.x));
        Vector2 right_hit = player_detect.detectors[1].hitPoint;
        double distanceFromRight = Mathf.Cos(Mathf.Atan2(right_hit.y - car.position.y, right_hit.x - car.position.x));
        double leftChange = Mathf.Clamp((float)(distanceFromLeft - prevLeft), -1, 1);
        double rightChange = Mathf.Clamp((float)(distanceFromRight - prevRight), -1, 1);
        prevLeft = distanceFromLeft;
        prevRight = distanceFromRight;

        int laneType = 0;
        switch (player_detect.detectors[0].laneDetected.lane)
        {
            case LaneObject.lane_type.left_turn:
                laneType = -1;
                break;
            case LaneObject.lane_type.right_turn:
                laneType = 1;
                break;
        }

        Vector2 front_hit = player_detect.detectors[0].hitPoint;
        double distanceFront = Mathf.Sqrt((front_hit.x - car.position.x) * (front_hit.x - car.position.x) + (front_hit.y - car.position.y) * (front_hit.y - car.position.y));
        double changeFront = Mathf.Clamp((float)(distanceFront - prevFront), -1, 1);
        prevFront = distanceFront;

        double rotate = Mathf.Clamp(rotationRead.rotation / 180.0f, -1, 1);

        // Prepare the line for CSV
        string line = leftChange + "," + rightChange + "," + laneType + "," + changeFront + "," + rotate + "," + expectedOut;

        // Append the data to the file
        File.AppendAllText(filePath, line + "\n");
    }

    private string expectOut()
    {
        switch (current)
        {
            case PredictMovement.movement.straight:
                return "1,0,0,-1,-1";
            case PredictMovement.movement.lane_right:
                return "0,1,-1,0,-1";
            case PredictMovement.movement.lane_left:
                return "0,-1,1,0,0";
            case PredictMovement.movement.turn_right:
                return "0,0,0,1,-1";
            case PredictMovement.movement.turn_left:
                return "0,0,0,-1,1";
        }

        return "0,0,0,0,0";
    }
}
