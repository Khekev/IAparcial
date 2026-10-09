using UnityEngine;

public class Agent5Evade : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;

    [Header("Movimiento")]
    public float maxSpeed = 5f;
    public float maxForce = 0.1f;
    public float mass = 1f;

    [Header("Evade")]
    public float predictionTime = 1.5f;
    public float evadeDistance = 10f;

    [Header("Obstacle Avoidance")]
    public float detectionDistance = 2f;
    public float obstacleRadius = 0.5f;
    public float avoidanceForce = 3f;
    public LayerMask obstacleLayer;

    private Vector2 velocity;
    private Vector2 targetVelocity;
    private Vector2 lastTargetPosition;

    void Start()
    {
        if (target != null)
        {
            lastTargetPosition =
                target.position;
        }
    }

    void Update()
    {
        if (target == null)
            return;

        // Velocidad del perseguidor
        targetVelocity =
            ((Vector2)target.position -
             lastTargetPosition) /
            Mathf.Max(Time.deltaTime, 0.0001f);

        lastTargetPosition =
            target.position;

        Vector2 steering = Vector2.zero;

        // EVADE
        steering += CalculateEvade();

        // OBSTACLE AVOIDANCE
        steering += CalculateObstacleAvoidance();

        steering =
            Vector2.ClampMagnitude(
                steering,
                maxForce
            );

        steering /= mass;

        velocity += steering;

        velocity =
            Vector2.ClampMagnitude(
                velocity,
                maxSpeed
            );

        transform.position +=
            (Vector3)(velocity * Time.deltaTime);

        if (velocity.magnitude > 0.01f)
        {
            float angle =
                Mathf.Atan2(
                    velocity.y,
                    velocity.x
                ) * Mathf.Rad2Deg;

            transform.rotation =
                Quaternion.Euler(0, 0, angle);
        }
    }

    Vector2 CalculateEvade()
    {
        Vector2 direction =
            (Vector2)target.position -
            (Vector2)transform.position;

        float distance =
            direction.magnitude;

        if (distance > evadeDistance)
        {
            return Vector2.zero;
        }

        float speed =
            velocity.magnitude;

        float prediction;

        if (speed <= 0.01f)
        {
            prediction = predictionTime;
        }
        else if (speed <=
                 distance / predictionTime)
        {
            prediction = predictionTime;
        }
        else
        {
            prediction =
                distance / speed;
        }

        Vector2 futurePosition =
            (Vector2)target.position +
            targetVelocity *
            prediction;

        // Dirección contraria
        Vector2 fleeDirection =
            (Vector2)transform.position -
            futurePosition;

        if (fleeDirection.magnitude <= 0.01f)
            return Vector2.zero;

        Vector2 desiredVelocity =
            fleeDirection.normalized *
            maxSpeed;

        return desiredVelocity - velocity;
    }

    Vector2 CalculateObstacleAvoidance()
    {
        if (velocity.magnitude <= 0.01f)
            return Vector2.zero;

        Vector2 direction =
            velocity.normalized;

        RaycastHit2D hit =
            Physics2D.CircleCast(
                transform.position,
                obstacleRadius,
                direction,
                detectionDistance,
                obstacleLayer
            );

        if (hit.collider != null)
        {
            return hit.normal * avoidanceForce;
        }

        return Vector2.zero;
    }
}