using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Player_Controller))]
public class Player : MonoBehaviour
{
    public float jumpHeight;
    public float timeToJumpApex;
    float accelarationTimeAirborne = .2f;
    float accelarationTimeGrounded = .1f;
    float moveSpeed = 6;

    float jumpVelocity;
    float gravity;
    Vector3 velocity;
    float velocityXSmoothing;

    Player_Controller controller;

    void Start()
    {
        controller = GetComponent<Player_Controller>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
    }

    void Update()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown (KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelarationTimeGrounded : accelarationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}