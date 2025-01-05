using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarAvoidance : MonoBehaviour
{
    private Transform userCar;
    private PredictMovement prediction;

    private AVMovement movement;
    private float moveChange = .75f;

    private static float carRadius = 1f;
    private static float safeDistance = carRadius * 4;
    private float reactDistance = safeDistance;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<AVMovement>();

        userCar = GameObject.FindGameObjectWithTag("Player").transform;
        prediction = FindObjectOfType<PredictMovement>();
    }

    // Update is called once per frame
    void Update()
    {

        // Evaluate movement only when the player car is within reaction distance
        if (Vector2.Distance(transform.position, userCar.position) < reactDistance)
            movement.ChangeDirection(evaluate());
        else
            movement.ChangeDirection(new Vector2(0, 1));
    }

    Vector2 prevDiff = Vector2.zero, prevPos = Vector2.zero;
    private Vector2 evaluate()
    {
        Vector2 diff = (transform.position - userCar.position).normalized;
        Vector2 moveVector = prevDiff - diff,
                move1Vector = (Vector2)transform.position - prevPos;
        float angle = Mathf.Atan2(diff.x, diff.y); // Angle between AI and player car
        float angleBtwLanes = Mathf.Atan2(Vector2.Distance(transform.position, userCar.position), PredictMovement.distance_BTW_lanes);

        // Predict player movement
        int playerLane = prediction.future != 0 ? (int)prediction.future : (int)prediction.current;

        prevDiff = diff;
        prevPos = transform.position;

        if (Mathf.Abs(angle) < angleBtwLanes) // If the player is in the center lane
            return new Vector2(0, 1); // Continue straight

        // Check if AI is behind the player
        if (Mathf.Abs(angle - Mathf.PI) < angleBtwLanes)
        {
            return new Vector2(0, moveChange); // Adjust to avoid collision
        }
        else // If not behind, determine position relative to the player
        {
            if (diff.x > 0) // Player is on the right side
            {
                if (playerLane == 1) // Player changing to the right lane
                    return diff.y > 0 ? new Vector2(0, 1.0f / moveChange) : new Vector2(0, moveChange); // Adjust for top/bottom
            }
            else // Player is on the left side
            {
                if (playerLane == 2) // Player changing to the left lane
                    return diff.y > 0 ? new Vector2(0, 1.0f / moveChange) : new Vector2(0, moveChange); // Adjust for top/bottom
            }
        }

        // Default to moving forward
        return new Vector2(0, 1);
    }
}

