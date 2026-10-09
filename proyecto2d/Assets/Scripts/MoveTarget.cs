using UnityEngine;

public class MoveTarget2D : MonoBehaviour
{
    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(
            Input.mousePosition
        );

        mouse.z = 0;

        transform.position = mouse;
    }
}