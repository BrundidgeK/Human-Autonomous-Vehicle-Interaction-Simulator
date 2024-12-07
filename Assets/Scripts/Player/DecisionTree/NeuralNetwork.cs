using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace PredictiveAI
{
    public class NeuralNetwork
    {
        Layer[] layers;

        public NeuralNetwork(params int[] layerSizes) {
            layers = new Layer[layerSizes.Length-1];
            for(int i = 0; i < layers.Length; i++)
            {
                layers[i] = new Layer(layerSizes[i], layerSizes[i+1]);
            }
        }

        double[] CalculateOutputs(double[] inputs)
        {
            foreach(Layer layer in layers)
            {
                inputs = layer.CalculateOutputs(inputs);
            }

            return inputs;
        }

        int Classify(double[] inputs)
        {
            double[] outputs = CalculateOutputs(inputs);

            int index = -1;
            double maxVal = -1;
            for(int i = 0; i < outputs.Length;i++)
            {
                if (outputs[i] > maxVal)
                {
                    maxVal = outputs[i];
                    index = i;
                }
            }

            return index;
        }

        public void Learn(DataPoint[] trainingData, double learnRate)
        {
            const double h = .0000001;
            double originalCost = Cost(trainingData);

            foreach(Layer layer in layers)
            {
                for(int i = 0; i < layer.numNodesIn; i++)
                {
                    for(int o = 0; o < layer.numNodesOut; o++)
                    {
                        layer.weights[i, o] += h;
                        double deltaCost = Cost(trainingData) - originalCost;
                        layer.weights[i, o] -= h;
                        layer.costGradientW[i, o] = deltaCost / h;
                    }
                }
                layer.applyGradients(learnRate);
            }
        }

        void UpdateGradients(DataPoint data)
        {
            Layer outputLayer = layers[layers.Length-1];
            double[] nodes = outputLayer.CalculateOutputNodeValues(data.expectedOutputs);
        }

        double Cost(DataPoint data)
        {
            double[] outputs = CalculateOutputs(data.Data());
            Layer outputLayer = layers[layers.Length - 1];
            double cost = 0;

            for(int i = 0; i < outputs.Length;i++)
            {
                cost += Layer.NodeCost(outputs[i], data.expectedOutputs[i]);
            }

            return cost; 
        }

        double Cost(DataPoint[] data)
        {
            double totalCost = 0;

            foreach(DataPoint dataPt in data)
            {
                totalCost += Cost(dataPt);
            }

            return totalCost / data.Length;
        }
    }
}