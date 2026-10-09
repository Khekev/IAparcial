using UnityEngine;

public class Agent3Wander : MonoBehaviour
{
    [Header("Movimiento")]
    public float maxSpeed = 3f;
    public float maxForce = 0.1f;
    public float mass = 1f;

    [Header("Wander")]
    public float wanderRadius = 2f;
    public float wanderDistance = 3f;
    public float wanderJitter = 1f;

    [Header("Limites de pantalla")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    private Vector2 velocity;
    private Vector2 wanderTarget;

    void Start()
    {
        wanderTarget = Random.insideUnitCircle * wanderRadius;
    }

    void Update()
    {
        Vector2 steering = CalculateWander();

        steering = Vector2.ClampMagnitude(steering, maxForce);
        steering /= mass;

        velocity += steering;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

        transform.position +=
            (Vector3)(velocity * Time.deltaTime);

        // Mantener dentro de la pantalla
        Vector3 position = transform.position;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        transform.position = position;
    }

    Vector2 CalculateWander()
    {
        Vector2 randomOffset =
            Random.insideUnitCircle *
            wanderJitter *
            Time.deltaTime;

        wanderTarget += randomOffset;

        wanderTarget =
            Vector2.ClampMagnitude(
                wanderTarget,
                wanderRadius
            );

        Vector2 forward;

        if (velocity.magnitude > 0.01f)
            forward = velocity.normalized;
        else
            forward = transform.right;

        Vector2 targetPosition =
            (Vector2)transform.position +
            forward * wanderDistance +
            wanderTarget;

        Vector2 desiredVelocity =
            (targetPosition -
             (Vector2)transform.position)
            .normalized * maxSpeed;

        return desiredVelocity - velocity;
    }
}