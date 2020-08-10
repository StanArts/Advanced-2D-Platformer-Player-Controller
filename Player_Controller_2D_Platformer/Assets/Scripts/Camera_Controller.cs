using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public Player_Controller target;
    public float verticalOffset;

    public float lookAheadDistanceX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;

    // Defines area around the followed target:
    public Vector2 focusedAreaSize;

    FocusedArea focusedArea;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    bool lookAheadStopped;

    void Start()
    {
        focusedArea = new FocusedArea(target.collider.bounds, focusedAreaSize);
    }

    void LateUpdate()
    {
        focusedArea.Update(target.collider.bounds);

        Vector2 focusedPosition = focusedArea.center + Vector2.up * verticalOffset;

        if (focusedArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusedArea.velocity.x);

            if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusedArea.velocity.x) && target.playerInput.x != 0)
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDistanceX;
            }
            else
            {
                if (!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDistanceX - currentLookAheadX)/4f;
                }
            }
        }

        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);
        //targetLookAheadX = lookAheadDirX * lookAheadDistanceX;

        focusedPosition.y = Mathf.SmoothDamp(transform.position.y, focusedPosition.y, ref smoothVelocityY, verticalSmoothTime);
        focusedPosition += Vector2.right * currentLookAheadX;

        transform.position = (Vector3)focusedPosition + Vector3.forward * (-10);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusedArea.center, focusedAreaSize);
    }

    struct FocusedArea
    {
        public Vector2 center;
        public Vector2 velocity;

        float left, right;
        float top, bottom;

        public FocusedArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }

            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }

            top += shiftY;
            bottom += shiftY;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
}
