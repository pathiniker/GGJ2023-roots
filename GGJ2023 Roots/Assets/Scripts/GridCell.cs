using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

        if (!string.IsNullOrEmpty(Data.MinedItemId))
            StartCoroutine(DoShatter());
        else
            Destroy(gameObject);
    }

    public IEnumerator DoShatter(System.Action onCompleteCb = null)
    {
        float duration = 0.35f;
        int count = 5;

        List<RockFragment> prefabs = GameController.Instance.GridGenerator.GetRockFragmentPrefabs(count);
        List<RockFragment> spawned = new List<RockFragment>();

        float durationPer = duration / count;
        foreach (RockFragment f in prefabs)
        {
            float jump = Random.Range(0.5f, 1.5f);
            float scale = Random.Range(0.25f, 1.2f);
            scale *= 2f;
            float fragDuration = duration * jump * 2f;
            Vector3 punch = new Vector3(scale, scale, scale);

            RockFragment fragment = Instantiate(f, transform);
            spawned.Add(fragment);
            fragment.transform.localScale = Vector3.zero;
            fragment.SyncTo(Data.MinedItemId);
            fragment.transform.SetParent(GameController.Instance.MineMachine.transform);

            fragment.transform.DOPunchScale(punch, fragDuration);
            //Vector3 jumpToPosition = GameController.Instance.MineMachine.transform.position;
            //jumpToPosition.z -= 0.6f;
            //fragment.transform.DOJump(jumpToPosition, jump, 1, duration * jump * 2f);
            fragment.transform.DOLocalJump(Vector3.zero, jump, 1, fragDuration);

            yield return new WaitForSeconds(durationPer / 2f);
        }

        yield return new WaitForSeconds(duration / count);
        GameController.Instance.MineMachine.AddItemToInventory(Data.MinedItemId, count);

        for (int i = 0; i < spawned.Count; i++)
        {
            Destroy(spawned[i].gameObject);
        }

        spawned.Clear();

        onCompleteCb?.Invoke();
        Destroy(gameObject);
        yield break;
    }
}
