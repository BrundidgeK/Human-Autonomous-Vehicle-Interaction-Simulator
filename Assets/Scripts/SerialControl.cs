using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class SerialControl : MonoBehaviour
{
    [SerializeField]
    private int baudRate = 9600;
    [SerializeField]
    private int millisWait = 500;
    SerialPort serialPort;

    public float rotation;
    Thread serialThread;
    bool keepReading = true;

    void Start()
    {
        serialPort = new SerialPort("COM4", baudRate);
        if (!serialPort.IsOpen)
        {
            serialPort.Open();
            serialPort.ReadTimeout = 100; // Set a short timeout to avoid blocking
        }

        serialThread = new Thread(ReadFromPort);
        serialThread.Start();

        Invoke("updateRotation", millisWait / 1000);
    }

    void Update()
    {

    }

    private void ReadFromPort()
    {
        while (keepReading)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    string data = serialPort.ReadLine(); // Read a line of data from the serial port
                    rotation = toDouble(data);
                }
            }
            catch (System.TimeoutException)
            {
                // Handle timeout exceptions
            }
        }
    }

    void OnApplicationQuit()
    {
        keepReading = false;
        if (serialThread != null && serialThread.IsAlive)
        {
            serialThread.Join(); // Wait for thread to finish
        }
        if (serialPort.IsOpen)
        {
            serialPort.Close(); // Close serial port on exit
        }
    }

    float toDouble(string s)
    {
        float num = float.NaN;

        try
        {
            num = -float.Parse(s);
        }
        catch
        {
            return rotation;
        }

        return num;
    }
}
