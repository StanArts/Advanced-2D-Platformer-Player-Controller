using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Controller : Raycast_Controller
{
    public LayerMask objectsMask;
    public Vector3 move;

    public override void Start()
    {
        base.Start();
    }

    void Update()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = move * Time.deltaTime;

        MoveObjects(velocity);
        transform.Translate(velocity);
    }
    
    void MoveObjects (Vector3 velocity)
    {
        HashSet<Transform> movedObjects = new HashSet<Transform>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        // Moves the platform vertically:
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, objectsMask);

                if (hit)
                {
                    if (!movedObjects.Contains(hit.transform))
                    {
                        movedObjects.Add(hit.transform);
                    }

                    float pushX = (directionY == 1) ? velocity.x:0;
                    float pushY = velocity.y - (hit.distance - skinWidth * directionY);

                    hit.transform.Translate(new Vector3(pushX, pushY));
                }
            }
        }

        // Moves the platform horizontally:
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, objectsMask);

                if (hit)
                {
                    if (!movedObjects.Contains(hit.transform))
                    {
                        movedObjects.Add(hit.transform);
                    }

                    float pushX = (directionY == 1) ? velocity.x : 0;
                    float pushY = velocity.x - (hit.distance - skinWidth * directionX);

                    hit.transform.Translate(new Vector3(pushX, pushY));
                }
            }
        }
    }
}
