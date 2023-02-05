using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_UpgradesDisplay : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] List<UI_UpgradeOption> _optionButtons = new List<UI_UpgradeOption>();

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    public void ReRollOptions()
    {
        //Debug.lo
        for (int i = 0; i < _optionButtons.Count; i++)
        {
            System.Action cb = null;
            System.Action completePurchaseCb = null;
            string label = "";
            int cost = 0;

            switch (i)
            {
                case 0: // Drill
                    label = "Drill+";
                    cost = 25;
                    completePurchaseCb += () => GameController.Instance.MineMachine.UpgradeDrill();
                    break;

                case 1: // Storage
                    label = "Storage+";
                    cost = 15;
                    completePurchaseCb += () => GameController.Instance.MineMachine.UpgradeStorage();
                    break;

                case 2: // Fuel
                    label = "Fuel+";
                    cost = 20;
                    completePurchaseCb += () => GameController.Instance.MineMachine.UpgradeFuel();
                    break;

                default:
                    break;
            }

            cb += () => GameController.Instance.MineMachine.SpendCurrency(cost, completePurchaseCb);

            UI_UpgradeOption option = _optionButtons[i];
            option.SyncTo(label, cost, cb);
        }
    }
}
