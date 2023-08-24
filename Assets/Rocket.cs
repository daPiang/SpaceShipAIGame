using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float speed = 5f;
    public float raycastDistance = 2f;
    public float avoidForce = 10f;

    [Header("Raycast Settings")]
    [Tooltip("The angle to deviate for side raycasts.")]
    public float raycastAngle = 12f;
    [Tooltip("Color to display raycasts in debug mode.")]
    public Color raycastDebugColor = Color.red;

    private bool isPerformingTurn = false;
    private float turnAmountRequired = 0f;
    private float amountTurned = 0f;
    private int turnDirection = 1; // 1 for right, -1 for left

    private void Update()
    {
        MoveForward();
        if(isPerformingTurn)
        {
            HandleTurn();
        }
        else
        {
            CheckForObstacles();
        }
    }

    void MoveForward()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void CheckForObstacles()
    {
        Vector2 rightOffset = Quaternion.Euler(0, 0, -raycastAngle) * transform.up;
        Vector2 leftOffset = Quaternion.Euler(0, 0, raycastAngle) * transform.up;

        bool isObstacleOnRight = RaycastForObstacle(rightOffset, 1);
        bool isObstacleOnLeft = RaycastForObstacle(leftOffset, 1);
        bool isObstacleInFront = RaycastForObstacle(transform.up, 1.1f);
        bool isObstacleOnShipRight = RaycastForObstacle(transform.right, 1);
        bool isObstacleOnShipLeft = RaycastForObstacle(-transform.right, 1);

        if (isObstacleInFront)
        {
            if (isObstacleOnRight && isObstacleOnLeft)
            {
                if (isObstacleOnShipRight && isObstacleOnShipLeft)
                {
                    // If both left and right sides are blocked, perform a 180 turn
                    turnAmountRequired = 180f;
                }
                else
                {
                    // If only the front is blocked, perform a 90 turn
                    turnAmountRequired = 90f;
                }
                DecideTurnDirection(isObstacleOnShipRight, isObstacleOnShipLeft);
                isPerformingTurn = true;
            }
            else if (!isObstacleOnRight) 
            {
                AvoidObstacle(1);
            }
            else if (!isObstacleOnLeft) 
            {
                AvoidObstacle(-1);
            }
        }
        else
        {
            if (isObstacleOnRight) AvoidObstacle(1);
            if (isObstacleOnLeft) AvoidObstacle(-1);
        }
    }

    bool RaycastForObstacle(Vector2 direction, float distanceMultiplier)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance * distanceMultiplier);
        Debug.DrawRay(transform.position, direction * raycastDistance * distanceMultiplier, raycastDebugColor);
        return hit.collider != null && hit.collider.CompareTag("Asteroid");
    }

    void AvoidObstacle(float avoidDir)
    {
        transform.Rotate(0, 0, Time.deltaTime * avoidForce * avoidDir);
    }

    void HandleTurn()
    {
        float rotationAmount = Time.deltaTime * avoidForce * turnDirection;
        transform.Rotate(0, 0, rotationAmount);
        amountTurned += Mathf.Abs(rotationAmount);

        if(amountTurned >= turnAmountRequired)
        {
            isPerformingTurn = false;
            amountTurned = 0f;
        }
    }

    void DecideTurnDirection(bool obstacleRight, bool obstacleLeft)
    {
        if (obstacleRight && obstacleLeft)
        {
            // Random direction if obstacles on both sides
            turnDirection = Random.Range(0, 2) == 0 ? 1 : -1;
        }
        else if (obstacleRight)
        {
            turnDirection = -1; // Turn left
        }
        else if (obstacleLeft)
        {
            turnDirection = 1; // Turn right
        }
        else
        {
            // No obstacles on the side, so pick a random direction
            turnDirection = Random.Range(0, 2) == 0 ? 1 : -1;
        }
    }
}