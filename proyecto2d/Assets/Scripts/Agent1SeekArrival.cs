using UnityEngine;

public class Agent1SeekArrival : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;

    [Header("Movimiento")]
    public float maxSpeed = 5f;
    public float maxForce = 0.1f;
    public float mass = 1f;

    [Header("Arrival")]
    public float arrivalRadius = 3f;
    public float stopRadius = 0.1f;

    [Header("Obstacle Avoidance")]
    public float detectionDistance = 2f;
    public float obstacleRadius = 0.5f;
    public float avoidanceForce = 3f;
    public LayerMask obstacleLayer;

    private Vector2 velocity;

    void Update()
    {
        if (target == null)
            return;

        Vector2 steering = Vector2.zero;

        // SEEK + ARRIVAL
        steering += CalculateArrival();

        // OBSTACLE AVOIDANCE
        steering += CalculateObstacleAvoidance();

        // Limitar fuerza
        steering = Vector2.ClampMagnitude(steering, maxForce);

        // Aplicar masa
        steering /= mass;

        // Actualizar velocidad
        velocity += steering;

        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

        // Movimiento 2D
        transform.position +=
            (Vector3)(velocity * Time.deltaTime);

        // Rotación
        if (velocity.magnitude > 0.01f)
        {
            float angle =
                Mathf.Atan2(velocity.y, velocity.x) *
                Mathf.Rad2Deg;

            transform.rotation =
                Quaternion.Euler(0, 0, angle);
        }
    }

    Vector2 CalculateArrival()
    {
        Vector2 direction =
            (Vector2)target.position -
            (Vector2)transform.position;

        float distance = direction.magnitude;

        if (distance <= stopRadius)
        {
            velocity = Vector2.zero;
            return Vector2.zero;
        }

        direction.Normalize();

        float speed = maxSpeed;

        if (distance < arrivalRadius)
        {
            speed =
                maxSpeed *
                (distance / arrivalRadius);
        }

        Vector2 desiredVelocity =
            direction * speed;

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