using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelStation : MonoBehaviour
{
    bool _hasRefueled = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            HandlePlayerReturn();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            HandlePlayerExit();
    }

    void HandlePlayerReturn()
    {
        GameController.Instance.MineMachine.Refuel(showText: true);
    }

    void HandlePlayerExit()
    {
        GameController.Instance.MineMachine.StopRefuel();

        if (!_hasRefueled)
        {
            StartCoroutine(ShowStoryMessage());
        }

        _hasRefueled = true;
    }

    IEnumerator ShowStoryMessage()
    {
        yield return new WaitForSeconds(1f);
        StoryController.Instance.PlayNextStoryStep();
    }
}
