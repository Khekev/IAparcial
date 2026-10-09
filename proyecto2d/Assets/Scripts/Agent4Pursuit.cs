using UnityEngine;

public class Agent4Pursuit : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;

    [Header("Movimiento")]
    public float maxSpeed = 5f;
    public float maxForce = 0.1f;
    public float mass = 1f;

    [Header("Pursuit")]
    public float predictionTime = 1.5f;

    [Header("Limites de pantalla")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    private Vector2 velocity;
    private Vector2 targetVelocity;
    private Vector2 lastTargetPosition;

    void Start()
    {
        if (target != null)
        {
            lastTargetPosition = target.position;
        }
    }

    void Update()
    {
        if (target == null)
            return;

        // Calcular velocidad del objetivo
        targetVelocity =
            ((Vector2)target.position - lastTargetPosition) /
            Mathf.Max(Time.deltaTime, 0.0001f);

        lastTargetPosition = target.position;

        Vector2 steering = CalculatePursuit();

        steering = Vector2.ClampMagnitude(
            steering,
            maxForce
        );

        steering /= mass;

        velocity += steering;

        velocity = Vector2.ClampMagnitude(
            velocity,
            maxSpeed
        );

        transform.position +=
            (Vector3)(velocity * Time.deltaTime);

        // Mantener dentro de la pantalla
        Vector3 position = transform.position;

        position.x = Mathf.Clamp(
            position.x,
            minX,
            maxX
        );

        position.y = Mathf.Clamp(
            position.y,
            minY,
            maxY
        );

        transform.position = position;
    }

    Vector2 CalculatePursuit()
    {
        Vector2 direction =
            (Vector2)target.position -
            (Vector2)transform.position;

        float distance = direction.magnitude;

        float speed = velocity.magnitude;

        float prediction;

        if (speed <= 0.01f)
        {
            prediction = predictionTime;
        }
        else
        {
            prediction = distance / speed;

            prediction = Mathf.Min(
                prediction,
                predictionTime
            );
        }

        // Predecir dónde estará el objetivo
        Vector2 futurePosition =
            (Vector2)target.position +
            targetVelocity * prediction;

        Vector2 desiredVelocity =
            (futurePosition -
             (Vector2)transform.position)
            .normalized * maxSpeed;

        return desiredVelocity - velocity;
    }
}