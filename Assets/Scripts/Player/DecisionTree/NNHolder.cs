using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNHolder : MonoBehaviour
{
    private PredictiveAI.NeuralNetwork neuralNetwork;
    public int[] layers;

    /*
     * Input Layer:
     * distance from left lane
     * distance from right lane
     * x movement vector
     * kind of lane (0-straight, -1-left turn, 1-right turn)
     * distance in front of car
     * wheel angle / 180
    */

    [SerializeField]
    private int learnTimes = 1000;


    // Start is called before the first frame update
    void Start()
    {
        neuralNetwork = new PredictiveAI.NeuralNetwork(layers);

        int loops = 0;
        while(loops < learnTimes)
        {
            loops++;


        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
