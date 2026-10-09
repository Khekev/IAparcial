using UnityEngine;

public class Agnt2Flee : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;

    [Header("Movimiento")]
    public float maxSpeed = 5f;
    public float maxForce = 0.1f;
    public float mass = 1f;

    [Header("Flee")]
    public float fleeDistance = 8f;

    [Header("Limites de pantalla")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    private Vector2 velocity;

    void Update()
    {
        if (target == null)
            return;

        Vector2 steering = CalculateFlee();

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

    Vector2 CalculateFlee()
    {
        Vector2 direction =
            (Vector2)transform.position -
            (Vector2)target.position;

        float distance = direction.magnitude;

        if (distance > fleeDistance)
        {
            return Vector2.zero;
        }

        direction.Normalize();

        Vector2 desiredVelocity =
            direction * maxSpeed;

        return desiredVelocity - velocity;
    }
}
