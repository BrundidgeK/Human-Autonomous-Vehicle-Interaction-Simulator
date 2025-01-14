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
            setData(data);
        }
        public DataPoint() { }

        public double[] Data()
        {
            return data;
        }

        public void setData(double[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != data[i])
                    data[i] = 0;
            }

            this.data = data;
        }

        public override string ToString()
        {
            string l = "";

            foreach (var item in data)
            {
                l += item.ToString()+", ";
            }

            return l;
        }
    }
}