using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Player_Controller))]
public class Player : MonoBehaviour
{
    float moveSpeed = 6;
    float jumpVelocity = 10;
    float gravity = -20;
    Vector3 velocity;

    Player_Controller controller;

    void Start()
    {
        controller = GetComponent<Player_Controller>();
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

        velocity.x = input.x * moveSpeed;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}