using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PredictiveAI
{
    public class Layer
    {
        public int numNodesIn, numNodesOut;
        public double[,] costGradientW;
        public double[] costGradientB;
        public double[,] weights;
        public double[] biases;

        double[] weightedInputs;
        double[] activations;


        public Layer(int numNodesIn, int numNodesOut)
        {
            this.numNodesIn = numNodesIn;
            this.numNodesOut = numNodesOut;

            costGradientW = new double[numNodesIn, numNodesOut];
            weights = new double[numNodesIn, numNodesOut];
            costGradientB = new double[numNodesOut];

            weights = new double[numNodesIn, numNodesOut];
            biases = new double[numNodesOut];
            weightedInputs = new double[numNodesOut];
            activations = new double[numNodesOut];
            randomWeights();
        }

        private void randomWeights()
        {
            for(int i = 0; i < numNodesIn; i++)
            {
                for(int o = 0; o < numNodesOut; o++)
                {
                    double rand = Random.Range(-3, 1);
                    weights[i, o] = rand / Mathf.Sqrt(numNodesIn);
                }
            }
        }

        public void applyGradients(double learnRate)
        {
            for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
            {
                biases[nodeOut] -= costGradientB[nodeOut] * learnRate;
                for (int nodeIn = 0; nodeIn < numNodesIn; nodeIn++)
                {
                    weights[nodeIn, nodeOut] -= costGradientW[nodeIn, nodeOut] * learnRate;
                }
            }
        }

        public double[] CalculateOutputs(double[] inputs)
        {
            double[] weightedInputs = new double[numNodesOut];

            for(int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
            {
                double weightedInput = biases[nodeOut];
                for(int nodeIn = 0; nodeIn < numNodesIn; nodeIn++)
                {
                    weightedInput += inputs[nodeIn] * weights[nodeIn, nodeOut];
                }
                activations[nodeOut] = ActivationFunction(weightedInput);
            }

            activations = weightedInputs;
            return weightedInputs;
        }

        public double[] CalculateOutputNodeValues(double[] expectedOutputs)
        {
            double[] nodeValues = new double[expectedOutputs.Length];

            for(int i = 0; i < nodeValues.Length; i++)
            {
                double costDer = NodeCostDerivative(activations[i], expectedOutputs[i]);
                double actDer = ActivationFunctionDerivative(weightedInputs[i]);
                nodeValues[i] = actDer * costDer;
            }

            return nodeValues;
        }

        double ActivationFunction(double weightedInput)
        {
            return 1.0 / (1 + Mathf.Exp(-(float)weightedInput));
        }

        double ActivationFunctionDerivative(double weightedInput)
        {
            double act = ActivationFunction(weightedInput);
            return act * (1 - act); 
        }

        public static double NodeCost(double outputActivation, double expectedOutput)
        {
            double error = outputActivation - expectedOutput;
            return error * error;
        }

        public static double NodeCostDerivative(double output, double expected)
        {
            return 2 * (output - expected);
        }
    }
}