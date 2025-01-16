using PredictiveAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class NNHolder : MonoBehaviour
{
    private PredictiveAI.NeuralNetwork neuralNetwork;
    private DataCollector collect;
    public static int[] layers = new int[]
    {
        6, 10, 7, 5
    };

    [SerializeField]
    private bool learning;

    /*
     * Input Layer:
     * distance from left lane
     * distance from right lane
     * kind of lane (0-straight, -1-left turn, 1-right turn)
     * distance in front of car
     * wheel angle / 180
    */

    [SerializeField]
    private int epochs = 1000, curE = 0;
    [SerializeField]
    private double learnRate = .25;

    string filePath = "C:/Users/kgbru/OneDrive/Documents/game stuff/AV-UNI-SIMI/Assets/Scripts/Player/DecisionTree/";

    private PredictMovement predictMovement;

    // Start is called before the first frame update
    void Start()
    {
        predictMovement = FindObjectOfType<PredictMovement>();
        collect = FindObjectOfType<DataCollector>();

        neuralNetwork = new PredictiveAI.NeuralNetwork(layers);
        curE = learning ? 0 : epochs;
        if (!learning)
            setWeights();

        currentMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    long currentMillis;

    // Update is called once per frame
    void Update()
    {
        if (curE < epochs)
        {
            List<DataPoint> datas = CSVToData();
            neuralNetwork.Learn(datas, datas.Count/5, learnRate);

            string a = "Epochs: " + curE + " / " + epochs + "\n";
            string b = "Current Cost: " + neuralNetwork.cost + "\n";
            string c = "Time: " + (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - currentMillis) / 1000.0);
            Debug.Log(a + b + c);


            curE++;
            if(curE == epochs)
            {
                string path = filePath+"WeightsBiases.txt"; 
                var sb = new StringBuilder();

                // Iterate through each layer
                foreach (Layer l in neuralNetwork.getLayers())
                {
                    for (int nodeOut = 0; nodeOut < l.numNodesOut; nodeOut++)
                    {
                        sb.Append(l.biases[nodeOut]); // Add bias for the current node

                        // Add weights for the current node
                        for (int nodeIn = 0; nodeIn < l.numNodesIn; nodeIn++)
                        {
                            sb.Append(", ").Append(l.weights[nodeIn, nodeOut]);
                        }

                        sb.AppendLine(); // New line after processing each output node
                    }

                    sb.AppendLine(); // Add an empty line between layers for readability
                }

                // Write all data to the file at once
                File.WriteAllText(path, sb.ToString());

                Debug.Log("Learning Complete!");
            }
        }
        else
        {
            int i = neuralNetwork.Classify(collect.getCurrentValues().data);
            try
            {
                predictMovement.changeMovement(i);
            }
            catch (Exception e)
            {
                Debug.Log(e + ", " + i);
            }
        }
    }

    private void setWeights()
    {
        string path = filePath + "WeightsBiases.txt";
        var lines = File.ReadAllLines(path);

        int layerIndex = 0; // To keep track of the current layer
        int nodeOutIndex = 0; // To track the output node in the current layer
        Layer currentLayer = null;

        foreach (var line in lines)
        {
            // Skip empty lines (used to separate layers)
            if (string.IsNullOrWhiteSpace(line))
            {
                layerIndex++;
                nodeOutIndex = 0; // Reset nodeOut index for the next layer
                currentLayer = null; // Move to the next layer
                continue;
            }

            // Get the current layer if not already set
            if (currentLayer == null && layerIndex < neuralNetwork.getLayers().Length)
            {
                currentLayer = neuralNetwork.getLayers()[layerIndex];
            }

            if (currentLayer == null) continue; // Safety check if no valid layer is found

            // Split the line into values (bias followed by weights)
            var values = line.Split(',').Select(v => float.Parse(v.Trim())).ToArray();

            // Set bias for the current output node
            currentLayer.biases[nodeOutIndex] = values[0];

            // Set weights for the current output node
            for (int nodeIn = 0; nodeIn < currentLayer.numNodesIn; nodeIn++)
            {
                currentLayer.weights[nodeIn, nodeOutIndex] = values[nodeIn + 1];
            }

            nodeOutIndex++; // Move to the next output node
        }

        Debug.Log("Weights and biases successfully loaded from file.");
    }


    private List<DataPoint> CSVToData()
    {
        var dataPoints = new List<DataPoint>();

        // Read all lines from the CSV file
        var lines = File.ReadAllLines(filePath + "TrainingData.csv");

        // Assume the first line is the header
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            var values = line.Split(',');


            if (values.Length < 11)
            {
                Debug.LogError($"Line {i + 1} is invalid: {line}");
                continue;
            }

            try
            {
                // Parse each field and create a DataPoint object
                var dataPoint = new DataPoint
                {
                    data = new double[]
                    {
                    double.Parse(values[0].Trim(), CultureInfo.InvariantCulture),
                    double.Parse(values[1].Trim(), CultureInfo.InvariantCulture),
                    double.Parse(values[2].Trim(), CultureInfo.InvariantCulture),
                    double.Parse(values[3].Trim(), CultureInfo.InvariantCulture),
                    double.Parse(values[4].Trim(), CultureInfo.InvariantCulture),
                    double.Parse(values[5].Trim(), CultureInfo.InvariantCulture)
                    },
                    expectedOutputs = new double[]
                    {
                    double.Parse(values[6].Trim(), CultureInfo.InvariantCulture),
                    double.Parse(values[7].Trim(), CultureInfo.InvariantCulture),
                    double.Parse(values[8].Trim(), CultureInfo.InvariantCulture),
                    double.Parse(values[9].Trim(), CultureInfo.InvariantCulture),
                    double.Parse(values[10].Trim(), CultureInfo.InvariantCulture)
                    }
                };

                dataPoints.Add(dataPoint);
            }
            catch (FormatException ex)
            {
                Debug.LogError($"Line {i + 1} has invalid number format: {line}. Error: {ex.Message}");
            }
            catch (OverflowException ex)
            {
                Debug.LogError($"Line {i + 1} has a value out of range: {line}. Error: {ex.Message}");
            }
        }
    

        if (dataPoints.Count == 0)
            Debug.LogError("Training Data could not be extracted");

        return dataPoints;
    }
}
