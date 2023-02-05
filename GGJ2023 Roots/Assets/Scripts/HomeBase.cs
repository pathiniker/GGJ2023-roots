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
        bool canBuy = GameController.Instance.MineMachine.Currency > 0;
        GameController.Instance.MineMachine.ShowUpgradesDisplay(canBuy);
    }

    void HandlePlayerExit()
    {
        GameController.Instance.MineMachine.ShowUpgradesDisplay(false);
    }
}
