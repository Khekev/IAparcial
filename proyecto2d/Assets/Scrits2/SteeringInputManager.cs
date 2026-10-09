
using UnityEngine;

public class SteeringInputManager : MonoBehaviour
{
    [Header("Agentes en orden del 1 al 5")]
    public SteeringAgent2D[] agents = new SteeringAgent2D[5];

    private readonly KeyCode[] numberKeys =
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5
    };

    private readonly KeyCode[] keypadKeys =
    {
        KeyCode.Keypad1,
        KeyCode.Keypad2,
        KeyCode.Keypad3,
        KeyCode.Keypad4,
        KeyCode.Keypad5
    };

    void Update()
    {
        for (int i = 0; i < 5; i++)
        {
            if (Input.GetKeyDown(numberKeys[i]) ||
                Input.GetKeyDown(keypadKeys[i]))
            {
                if (agents[i] != null)
                {
                    agents[i].SetActive(
                        !agents[i].IsActive
                    );
                }
            }
        }
    }
}