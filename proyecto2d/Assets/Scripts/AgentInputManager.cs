using UnityEngine;

public class AgentInputManager : MonoBehaviour
{
    [Header("Agentes")]
    public GameObject agente1;
    public GameObject agente2;
    public GameObject agente3;
    public GameObject agente4;
    public GameObject agente5;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleAgent(agente1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToggleAgent(agente2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ToggleAgent(agente3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ToggleAgent(agente4);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ToggleAgent(agente5);
        }
    }

    void ToggleAgent(GameObject agente)
    {
        if (agente == null)
            return;

        agente.SetActive(
            !agente.activeSelf
        );
    }
}