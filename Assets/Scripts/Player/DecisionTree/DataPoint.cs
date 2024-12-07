using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PredictiveAI
{
    public class DataPoint
    {
        public double[] data;
        public double[] expectedOutputs; // EX: 0, 0, 1, 0, 0 (lane left)
        

        public DataPoint(double[] data)
        {
            this.data = data;
        }
        public DataPoint() { }

        public double[] Data()
        {
            return data;
        }

        public void setData(double[] data)
        {
            this.data = data;
        }

        public override string ToString()
        {
            return data.ToString();
        }
    }
}