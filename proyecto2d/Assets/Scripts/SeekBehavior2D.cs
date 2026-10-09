
using UnityEngine;

public class SeekBehavior2D : MonoBehaviour

{
    [Header("Configuración del Agente")]
    public Transform target; // Posición del objetivo
    public float maxSpeed = 5f;
    public float maxForce = 0.1f;
    public float mass = 1f;

    private Vector3 velocity;

    void Update()
    {
        // Si no hay objetivo, el agente no hace nada
        if (target == null) return;

        // Calcular la dirección hacia el objetivo
        Vector3 desiredVelocity =
            (target.position - transform.position).normalized * maxSpeed;

        // Calcular la fuerza de dirección (Steering)
        Vector3 steering = desiredVelocity - velocity;

        // Limitar la fuerza máxima
        steering = Vector3.ClampMagnitude(steering, maxForce);

        // Aplicar la masa
        steering /= mass;

        // Actualizar la velocidad
        velocity = Vector3.ClampMagnitude(
            velocity + steering,
            maxSpeed
        );

        // Mover al agente
        transform.position += velocity * Time.deltaTime;
    }
}
