using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public SerialControl SerialControl;
    public float steeringSpeed = 5f; // Steering speed (how fast the wheel turns)
    public float maxSteeringAngle = 30f; // Maximum steering angle (in degrees)
    public float moveSpeed = 5f; // Constant forward speed

    private Rigidbody2D rb;
    private float steeringInput = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleSteeringInput();
    }

    void FixedUpdate()
    {
        MoveCar();
    }

    void HandleSteeringInput()
    {
        // Get player input for steering angle (simulated by mouse or controller input)
        steeringInput = Mathf.Clamp(SerialControl.rotation, -maxSteeringAngle, maxSteeringAngle);

        // Smoothly rotate the car based on the steering input
        transform.Rotate(0, 0, steeringInput * steeringSpeed *  Time.deltaTime);
    }

    void MoveCar()
    {
        // Move the car forward at a constant speed
        rb.velocity = transform.up * moveSpeed; // Move in the direction the car is facing
    }
}

