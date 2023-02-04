using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DepthLevel
{
    GroundLevel = 0,
    TierOne = 1,
    TierTwo = 2,
    TierThree = 3,
    TierFour = 4,
    TierFive = 5
}

public class GridCell : MonoBehaviour
{
    [SerializeField] CellData _cellData;

    public float Health { get; private set; }

    public CellData Data { get { return _cellData; } }

    private void OnEnable()
    {
        Health = _cellData.StartingHealth;
    }

    public void DealDamage(float amount)
    {
        amount = Mathf.Abs(amount);
        Health -= amount;

        if (Health <= 0)
            FinishCell();
    }

    public void FinishCell()
    {
        // Collect items from cell
        GameController.Instance.MineMachine.AddItemToInventory(Data.MinedItemId, 1);

        Destroy(gameObject);
    }
}
