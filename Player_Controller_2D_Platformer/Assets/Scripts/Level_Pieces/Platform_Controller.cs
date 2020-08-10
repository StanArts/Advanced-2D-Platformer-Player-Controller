using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Controller : Raycast_Controller
{
    public LayerMask riderMask;
    //public Vector3 move;
    public float speed;
    int fromWaypointIndex;
    float percentBetweenWaypoints; // between 0 and 1

    public bool isCycling;

    public float waitTime;
    float nextMoveTime;
    [Range(0,20)]
    public float easeAmount;

    public Vector3[] localWaypoints;
    [HideInInspector]
    public Vector3[] globalWaypoints;

    List<RiderMovement> riderMovement;
    Dictionary<Transform, Player_Controller> riderDictionary = new Dictionary<Transform, Player_Controller>();

    public override void Start()
    {
        base.Start();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    void Update()
    {
        UpdateRaycastOrigins();

        //Vector3 velocity = move * Time.deltaTime;
        Vector3 velocity = CalculatePlatformMovement();

        CalculateRiderMovement(velocity);

        MoveRiders(true);
        transform.Translate(velocity);
        MoveRiders(false);
    }

    float Ease (float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1-x, a));
    }

    Vector3 CalculatePlatformMovement()
    {
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoins = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoins;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easePercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easePercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (!isCycling)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }

            nextMoveTime = Time.time + waitTime;
        }

        return newPos - transform.position;
    }
    
    void MoveRiders(bool beforeMovePlatform)
    {
        foreach (RiderMovement rider in riderMovement)
        {
            if (!riderDictionary.ContainsKey(rider.transform))
            {
                riderDictionary.Add(rider.transform, rider.transform.GetComponent<Player_Controller>());
            }

            if (rider.movedBeforePlatform == beforeMovePlatform)
            {
                riderDictionary[rider.transform].Move(rider.velocity, rider.standingOnPlatform);
            }
        }
    }

    void CalculateRiderMovement (Vector3 velocity)
    {
        HashSet<Transform> movedRiders = new HashSet<Transform>();
        riderMovement = new List<RiderMovement>();

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
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, riderMask);

                if (hit && hit.distance != 0)
                {
                    if (!movedRiders.Contains(hit.transform))
                    {
                        movedRiders.Add(hit.transform);
                    
                        float pushX = (directionY == 1) ? velocity.x:0;
                        float pushY = velocity.y - (hit.distance - skinWidth * directionY);

                        //hit.transform.Translate(new Vector3(pushX, pushY));

                        riderMovement.Add(new RiderMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
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
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, riderMask);

                if (hit)
                {
                    if (!movedRiders.Contains(hit.transform))
                    {
                        movedRiders.Add(hit.transform);

                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = -skinWidth;

                        //float pushX = (directionY == 1) ? velocity.x : 0;
                        //float pushY = velocity.x - (hit.distance - skinWidth * directionX);

                        //hit.transform.Translate(new Vector3(pushX, pushY));

                        riderMovement.Add(new RiderMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }

        // Sticks objects to the platform when they are using it:
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = skinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, riderMask);

                if (hit)
                {
                    if (!movedRiders.Contains(hit.transform))
                    {
                        movedRiders.Add(hit.transform);

                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        //hit.transform.Translate(new Vector3(pushX, pushY));

                        riderMovement.Add(new RiderMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
    }

    struct RiderMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool movedBeforePlatform;

        public RiderMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _movedBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            movedBeforePlatform = _movedBeforePlatform;
        }
    }

    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying)?globalWaypoints[i] : localWaypoints[i] + transform.position;
                // Draws the vertical trajectory:
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                // Draws the horizontal trajectory:
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
