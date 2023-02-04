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

    void HandlePlayerReturn()
    {
        GameController.Instance.MineMachine.SellInventory();
    }
}
