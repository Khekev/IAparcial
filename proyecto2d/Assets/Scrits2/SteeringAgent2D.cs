
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class SteeringAgent2D : MonoBehaviour
{
    public enum Behaviour
    {
        SeekArrival,
        Flee,
        Wander,
        Pursuit,
        Evade
    }

    [Header("Comportamiento")]
    public Behaviour behaviour;
    public Transform target;
    public bool startsActive = false;

    [Header("Movimiento")]
    public float maxSpeed = 3f;
    public float acceleration = 12f;
    public float arrivalRadius = 1.5f;
    public float stopRadius = 0.08f;
    public float fleeRadius = 5f;
    public float predictionTime = 0.8f;

    [Header("Wander")]
    public float wanderRadius = 1.2f;
    public float wanderDistance = 1.5f;
    public float wanderJitter = 2f;

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleLayers;
    public float feelerLength = 1.2f;
    public float feelerWidth = 0.25f;
    public float avoidanceStrength = 2f;

    [Header("Limites del escenario")]
    public Vector2 arenaMin = new Vector2(-7.5f, -5f);
    public Vector2 arenaMax = new Vector2(7.5f, 5f);
    public float boundaryMargin = 1f;

    private Rigidbody2D rb;
    private Vector2 velocity;
    private Vector2 wanderDirection;
    private bool isActive;

    public bool IsActive => isActive;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;

        velocity = Vector2.zero;
        wanderDirection = Random.insideUnitCircle.normalized;
        isActive = startsActive;
    }

    public void SetActive(bool active)
    {
        isActive = active;

        if (!active)
        {
            velocity = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (!isActive)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 position = rb.position;

        Vector2 desiredVelocity =
            CalculateDesiredVelocity(position);

        Vector2 avoidance =
            CalculateObstacleAvoidance(position);

        Vector2 boundaryForce =
            CalculateBoundaryAvoidance(position);

        if (avoidance.sqrMagnitude > 0.01f)
        {
            // Si hay un obstáculo delante, priorizar esquivarlo
            desiredVelocity = avoidance.normalized * maxSpeed;
        }
        else if (boundaryForce.sqrMagnitude > 0.01f)
        {
            // Si está cerca del borde, regresar al escenario
            desiredVelocity = boundaryForce.normalized * maxSpeed;
        }

        desiredVelocity =
            Vector2.ClampMagnitude(desiredVelocity, maxSpeed);


        Vector2 steering = desiredVelocity - velocity;

        steering = Vector2.ClampMagnitude(
            steering,
            acceleration * Time.fixedDeltaTime
        );

        velocity += steering;

        // Evitar que conserve velocidad cuando debe detenerse
        if (desiredVelocity.sqrMagnitude < 0.01f)
        {
            velocity = Vector2.zero;
        }

        rb.linearVelocity = velocity;

        if (velocity.sqrMagnitude > 0.01f)
        {
            transform.up = velocity.normalized;
        }
    }

    Vector2 CalculateDesiredVelocity(Vector2 position)
    {
        switch (behaviour)
        {
            case Behaviour.SeekArrival:
                {
                    if (target == null)
                        return Vector2.zero;

                    Vector2 direction =
                        (Vector2)target.position - position;

                    float distance = direction.magnitude;

                    if (distance <= stopRadius)
                        return Vector2.zero;

                    float speed = maxSpeed;

                    if (distance < arrivalRadius)
                    {
                        speed *= distance / arrivalRadius;
                    }

                    return direction.normalized * speed;
                }

            case Behaviour.Flee:
                {
                    if (Camera.main == null)
                        return Vector2.zero;

                    Vector2 mousePosition =
                        Camera.main.ScreenToWorldPoint(
                            Input.mousePosition
                        );

                    Vector2 direction =
                        position - mousePosition;

                    if (direction.magnitude > fleeRadius)
                        return Vector2.zero;

                    if (direction.sqrMagnitude < 0.001f)
                        direction = Random.insideUnitCircle;

                    return direction.normalized * maxSpeed;
                }

            case Behaviour.Wander:
                {
                    wanderDirection +=
                        Random.insideUnitCircle *
                        wanderJitter *
                        Time.fixedDeltaTime;

                    if (wanderDirection.sqrMagnitude < 0.001f)
                        wanderDirection = Vector2.up;

                    wanderDirection.Normalize();

                    Vector2 forward =
                        velocity.sqrMagnitude > 0.05f
                        ? velocity.normalized
                        : (Vector2)transform.up;

                    Vector2 circleCenter =
                        forward * wanderDistance;

                    Vector2 wanderPoint =
                        circleCenter +
                        wanderDirection * wanderRadius;

                    return wanderPoint.normalized * maxSpeed;
                }

            case Behaviour.Pursuit:
                {
                    if (target == null)
                        return Vector2.zero;

                    SteeringAgent2D targetAgent =
                        target.GetComponent<SteeringAgent2D>();

                    Vector2 targetVelocity =
                        targetAgent != null
                        ? targetAgent.velocity
                        : Vector2.zero;

                    float distance =
                        Vector2.Distance(position, target.position);

                    float prediction =
                        Mathf.Min(
                            distance / Mathf.Max(maxSpeed, 0.1f),
                            predictionTime
                        );

                    Vector2 futurePosition =
                        (Vector2)target.position +
                        targetVelocity * prediction;

                    return (futurePosition - position).normalized
                        * maxSpeed;
                }



            case Behaviour.Evade:
                {
                    if (target == null)
                        return Vector2.zero;

                    SteeringAgent2D pursuer =
                        target.GetComponent<SteeringAgent2D>();

                    Vector2 pursuerVelocity =
                        pursuer != null ? pursuer.velocity : Vector2.zero;

                    float distance =
                        Vector2.Distance(position, target.position);

                    if (distance > fleeRadius)
                    {
                        return velocity.sqrMagnitude > 0.1f
                            ? velocity.normalized * maxSpeed
                            : Vector2.zero;
                    }

                    Vector2 futurePosition =
                        (Vector2)target.position +
                        pursuerVelocity * 0.3f;

                    Vector2 escape = position - futurePosition;

                    if (escape.sqrMagnitude < 0.01f)
                        escape = Vector2.up;

                    return escape.normalized * maxSpeed;
                }
        }

        return Vector2.zero;
    }

    Vector2 CalculateObstacleAvoidance(Vector2 position)
    {
        Vector2 forward =
            velocity.sqrMagnitude > 0.05f
            ? velocity.normalized
            : (Vector2)transform.up;

        if (forward.sqrMagnitude < 0.01f)
            forward = Vector2.up;

        Vector2 left = Rotate(forward, -30f);
        Vector2 right = Rotate(forward, 30f);

        RaycastHit2D centerHit =
            Physics2D.CircleCast(
                position,
                feelerWidth,
                forward,
                feelerLength,
                obstacleLayers
            );

        RaycastHit2D leftHit =
            Physics2D.CircleCast(
                position,
                feelerWidth,
                left,
                feelerLength * 0.85f,
                obstacleLayers
            );

        RaycastHit2D rightHit =
            Physics2D.CircleCast(
                position,
                feelerWidth,
                right,
                feelerLength * 0.85f,
                obstacleLayers
            );

        Vector2 avoidance = Vector2.zero;

        if (centerHit.collider != null)
        {
            Vector2 normal = centerHit.normal;

            if (normal.sqrMagnitude < 0.01f)
                normal = Rotate(forward, 90f);

            avoidance += normal *
                (1f + (feelerLength - centerHit.distance)
                / feelerLength);
        }

        if (leftHit.collider != null)
            avoidance += Rotate(forward, 45f);

        if (rightHit.collider != null)
            avoidance += Rotate(forward, -45f);

        return avoidance.sqrMagnitude > 0.001f
            ? avoidance.normalized
            : Vector2.zero;
    }

    Vector2 CalculateBoundaryAvoidance(Vector2 position)
    {
        Vector2 force = Vector2.zero;

        if (position.x < arenaMin.x + boundaryMargin)
            force.x += 1f;

        if (position.x > arenaMax.x - boundaryMargin)
            force.x -= 1f;

        if (position.y < arenaMin.y + boundaryMargin)
            force.y += 1f;

        if (position.y > arenaMax.y - boundaryMargin)
            force.y -= 1f;

        return force.sqrMagnitude > 0.001f
            ? force.normalized
            : Vector2.zero;
    }

    static Vector2 Rotate(Vector2 direction, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;

        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        return new Vector2(
            direction.x * cos - direction.y * sin,
            direction.x * sin + direction.y * cos
        );
    }
}