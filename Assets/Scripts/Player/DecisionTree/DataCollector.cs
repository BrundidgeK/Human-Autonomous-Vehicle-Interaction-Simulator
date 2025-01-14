using PredictiveAI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataCollector : MonoBehaviour
{
    public PredictMovement.movement current;

    public static float distance_BTW_lanes = 2;

    private SerialControl rotationRead;
    public LaneDetector player_detect;
    private double prevLeft, prevRight, prevFront;
    public Transform car;

    // Path to store training data
    private string filePath = "C:/Users/kgbru/OneDrive/Documents/game stuff/AV-UNI-SIMI/Assets/Scripts/Player/DecisionTree";

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
        rotationRead = FindObjectOfType<SerialControl>();

        filePath += "/TrainingData.csv";

        // Create the file and add the header if it doesn't exist
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "LeftChange,RightChange,LaneType,FrontChange,WheelAngle,ExpectedOutput\n");
        }
    }

    bool recording = false;
    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.Space) && !recording)
        {
            recording = true;
            StartCoroutine(record());
        }
        else if(Input.GetKeyUp(KeyCode.Space)) 
        {
            recording = false;
        }
    }

    IEnumerator record()
    {
        yield return new WaitForSeconds(.125f);
        writeToFile();
        if (recording)
            StartCoroutine(record());
    }

    private void writeToFile()
    {
        string expectedOut = expectOut();
        if (expectedOut == "0,0,0,0,0")
        {
            Debug.LogError("Pick an expected output");
            return;
        }

        DataPoint d = getCurrentValues();

        // Prepare the line for CSV
        string line = d.ToString() + expectedOut;
        line = line.Replace("NaN", "0");

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

    public DataPoint getCurrentValues()
    {
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
        try
        {
            switch (player_detect.detectors[0].laneDetected.lane)
            {
                case LaneObject.lane_type.left_turn:
                    laneType = -1;
                    break;
                case LaneObject.lane_type.right_turn:
                    laneType = 1;
                    break;
            }
        }
        catch
        {
            laneType = 0;
        }

        Vector2 front_hit = player_detect.detectors[0].hitPoint;
        double distanceFront = Mathf.Sqrt((front_hit.x - car.position.x) * (front_hit.x - car.position.x) + (front_hit.y - car.position.y) * (front_hit.y - car.position.y));
        double changeFront = Mathf.Clamp((float)(distanceFront - prevFront), -1, 1);
        if (double.IsNaN(changeFront))
            changeFront = 0;
        prevFront = distanceFront;

        double rotate = rotationRead.rotation;

        DataPoint data = new DataPoint();
        data.setData(new double[]
        {
            leftChange, rightChange, laneType, changeFront, rotate
        });
        return data;
    }
}
