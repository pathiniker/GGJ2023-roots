using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum AttackPhase
{
    Wait,
    ThrowStuff
}

public class EvilTree : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Transform _fightingSpot;

    [Header("Prefabs")]
    [SerializeField] GameObject _throwPrefab;

    bool _shouldFight = false;
    AttackPhase _attackPhase = AttackPhase.Wait;
    float _timeSinceLastAttack = 0f;
    bool _startedDeathSequence = false;
    public float Health = 100f;
    float _startingHealth = 100f;

    public bool IsAlive()
    {
        return Health > 0;
    }
    
    public void FightPlayer()
    {
        AudioManager.Instance.PlayBossMusic();

        GameController.Instance.MineMachine.CanMove = true;
        CameraController.Instance.Camera.DOFieldOfView(115f, 2f);
        StartCoroutine(StartFight());
        UiController.Instance.DoBossFightDisplay();
        StoryController.Instance.DisplayText($"I AM THE ROOTS OF ALL EVIL");
    }

    void EnterAttackPhase(AttackPhase phase)
    {
        _attackPhase = phase;
        StartCoroutine(DoAttackPhase());
    }

    void DoDeathSequence()
    {
        if (_startedDeathSequence)
            return;

        _startedDeathSequence = true;
        StartCoroutine(DeathSequence());
        
    }

    IEnumerator DeathSequence()
    {
        GameController.Instance.MineMachine.CanMove = false;
        transform.DOShakePosition(4f, 0.2f);
        StoryController.Instance.DisplayText($"This may be the end for me... But my seeds have been planted.");
        yield return new WaitForSeconds(4f);

        transform.DOScale(Vector3.zero, 5f);
        yield return new WaitForSeconds(3f);
        StoryController.Instance.DisplayText($"End.");
    }

    void ThrowItem()
    {
        StartCoroutine(DoThrowItem());
    }

    IEnumerator DoThrowItem()
    {
        float throwTime = 1f;

        GameObject obj = Instantiate(_throwPrefab, transform);
        obj.transform.SetParent(null);
        obj.transform.DOJump(GameController.Instance.MineMachine.transform.position, 4f, 1, throwTime);
        yield return new WaitForSeconds(throwTime);
        Destroy(obj);
        yield break;
    }

    IEnumerator DoAttackPhase()
    {
        float phaseTime;
        float attackDelay = 0.3f;

        switch (_attackPhase)
        {
            case AttackPhase.ThrowStuff:
                phaseTime = 8f;
                break;

            default:
                phaseTime = 6f;
                break;
        }

        float timePassed = 0f;
        float timeSinceLastAttack = 0f;

        while (timePassed < phaseTime)
        {
            if (!IsAlive())
                yield break;

            if (_attackPhase != AttackPhase.Wait)
            {
                if (_timeSinceLastAttack > attackDelay)
                {
                    DoCurrentAttack();
                }
            }

            _timeSinceLastAttack += Time.deltaTime;
            timePassed += Time.deltaTime;
            yield return null;
        }

        AttackPhase nextPhase = _attackPhase == AttackPhase.Wait ? AttackPhase.ThrowStuff : AttackPhase.Wait;
        EnterAttackPhase(nextPhase);

        yield break;
    }

    void DoCurrentAttack()
    {
        // Assume throw stuff for now
        _timeSinceLastAttack = 0f;

        if (!IsAlive())
            return;

        ThrowItem();
    }

    IEnumerator StartFight()
    {
        float moveTime = 3f;
        transform.DOMove(_fightingSpot.position, 3f); // Move to fighting position
        yield return new WaitForSeconds(moveTime);

        GameController.Instance.MineMachine.CanMove = true;
        _shouldFight = true;
        EnterAttackPhase(AttackPhase.ThrowStuff);
    }

    public void DealDamage(float amount)
    {
        Health -= amount;
        UiController.Instance.BossFight.SetHealthValueNormalized(Health / _startingHealth);

        if (Health <= 0)
        {
            DoDeathSequence();
            _startedDeathSequence = true;
        }
    }
}
