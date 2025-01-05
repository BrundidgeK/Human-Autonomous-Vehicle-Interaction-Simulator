using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AVMovement : MonoBehaviour
{
    private float moveSpeed;
    private Rigidbody2D rb;

    [SerializeField]
    private Vector2 moveTarget = new Vector2(0, 1);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = FindObjectOfType<CarMovement>().moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCar();
    }

    void MoveCar()
    {
        // Set velocity based on the current movement target
        rb.velocity = moveTarget * moveSpeed;
    }

    public void ChangeDirection(Vector2 move)
    {
        moveTarget = move;
    }
}
