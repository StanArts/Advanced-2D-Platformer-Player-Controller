using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Raycast_Controller : MonoBehaviour
{
    public LayerMask collisionMask;

    public const float skinWidth = .015f;
    // Defines how many rays are being fired horizontally and vertically:
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    // Used to calculate the spacing between each horizontal and vertical ray:
    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D collider;
    public RaycastOrigins raycastOrigins;

    public virtual void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    // Updates the defined coordinates
    public void UpdateRaycastOrigins()
    {
        // Bounds of the collider:
        Bounds bounds = collider.bounds;
        // Shrinks the bounds on all sides by the width of the skin of the Player's body:
        bounds.Expand(skinWidth * (-2));

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * (-2));

        /*
         * Checks if both the horizontal and the vertical 
         * ray counters are greater than or equal to 2
         * since when we require them there has to be one 
         * in each corner:
        */
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        /*
         * After we make sure we have at least two rays fired
         * in the horizontal and vertical directions,
         * we can calculate the spacing between each ray.
         * We assume that the horizontal ray counter equals 2,
         * then we would be dividing the bounds.size.y by 1,
         * meaning that the spacing in between the two rays
         * would be the entire length of the bounds.y:
        */
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        // Respectively, we repeat the calculations and for the verticalRaySpacing:
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    // Defining Box Collider's corners coordinates
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
