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

    bool _isShattering = false;

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
        {
            if (!_isShattering)
                StartCoroutine(DoShatter());

            _isShattering = true;
        }
        else
        {
            AudioManager.Instance.PlayBreakDirtSfx();
            Destroy(gameObject);
        }
    }

    public IEnumerator DoShatter(System.Action onCompleteCb = null)
    {
        float duration = 0.35f;
        int count = Random.Range(2, 6);

        AudioManager.Instance.PlayBreakCellSfx();

        List<RockFragment> prefabs = GameController.Instance.GridGenerator.GetRockFragmentPrefabs(count);
        List<RockFragment> spawned = new List<RockFragment>();

        float durationPer = duration / count;
        foreach (RockFragment f in prefabs)
        {
            float jump = Random.Range(0.1f, 1f);
            float scale = Random.Range(0.3f, 1.3f);
            scale *= 3f;
            float fragDuration = duration * jump * 2f;
            Vector3 punch = new Vector3(scale, scale, scale);

            RockFragment fragment = Instantiate(f, transform);
            spawned.Add(fragment);
            fragment.transform.localScale = Vector3.zero;
            Vector3 pos = fragment.transform.localPosition;
            pos.z -= 0.8f;
            fragment.transform.localPosition = new Vector3(Mathf.Lerp(pos.x - 0.5f, pos.x + 0.5f, Random.Range(0f, 1f)), pos.y, pos.z);
            fragment.SyncTo(Data.MinedItemId);
            fragment.transform.SetParent(GameController.Instance.MineMachine.transform);

            fragment.transform.DOPunchScale(punch, fragDuration);
            fragment.transform.DOLocalJump(Vector3.zero, jump, 1, fragDuration);

            yield return new WaitForSeconds(durationPer / 2f);
        }

        yield return new WaitForSeconds(duration / count);
        GameController.Instance.MineMachine.AddItemToInventory(Data.MinedItemId, count);
        AudioManager.Instance.PlayCollectOreSfx();

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
