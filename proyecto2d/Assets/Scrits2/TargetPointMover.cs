
using UnityEngine;

public class TargetPointMover : MonoBehaviour
{
    public Camera mainCamera;

    public Vector2 minBounds = new Vector2(-7.5f, -5f);
    public Vector2 maxBounds = new Vector2(7.5f, 5f);

    void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera == null)
            return;

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition =
                mainCamera.ScreenToWorldPoint(
                    Input.mousePosition
                );

            float x = Mathf.Clamp(
                mousePosition.x,
                minBounds.x,
                maxBounds.x
            );

            float y = Mathf.Clamp(
                mousePosition.y,
                minBounds.y,
                maxBounds.y
            );

            transform.position = new Vector3(x, y, 0f);
        }
    }
}