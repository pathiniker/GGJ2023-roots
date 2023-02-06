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
        int drillLevel = GameController.Instance.MineMachine.DrillLevel;
        bool offerDrillUpgrade = drillLevel < MineMachine.MAX_DRILL_LEVEL;

        int showOptions = 3; // Make dynamic

        for (int i = 0; i < _optionButtons.Count; i++)
        {
            UI_UpgradeOption option = _optionButtons[i];

            System.Action cb = null;
            System.Action completePurchaseCb = null;
            string label = "";
            int cost = 0;

            bool showOption = i < showOptions;

            if (i == 0 && !offerDrillUpgrade)
                showOption = false;

            option.gameObject.SetActive(showOption);

            if (!showOption)
                continue;

            switch (i)
            {
                case 0: // Drill
                    label = "Upgrade Drill";
                    cost = 100 * drillLevel;
                    completePurchaseCb += () => GameController.Instance.MineMachine.UpgradeDrill();
                    break;

                case 1: // Storage
                    label = "Increase Storage";
                    cost = 15;
                    completePurchaseCb += () => GameController.Instance.MineMachine.UpgradeStorage();
                    break;

                case 2: // Fuel
                    label = "Upgrade Fuel";
                    cost = 20;
                    completePurchaseCb += () => GameController.Instance.MineMachine.UpgradeFuel();
                    break;

                default:
                    break;
            }

            cb += () => GameController.Instance.MineMachine.SpendCurrency(cost, completePurchaseCb);

            option.SyncTo(label, cost, cb);
        }
    }
}
