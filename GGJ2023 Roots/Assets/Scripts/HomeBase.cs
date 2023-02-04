using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBase : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HandlePlayerReturn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HandlePlayerExit();
        }
    }

    void HandlePlayerReturn()
    {
        GameController.Instance.MineMachine.SellInventory();
        GameController.Instance.MineMachine.ShowUpgradesDisplay(true);
    }

    void HandlePlayerExit()
    {
        GameController.Instance.MineMachine.ShowUpgradesDisplay(false);
    }
}
