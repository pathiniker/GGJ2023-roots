using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MineMachine : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _baseMoveSpeed = 200f;
    [SerializeField] float _baseBoosterForce = 200f;
    [SerializeField] float _hitDelay = 0.2f;
    [SerializeField] float _baseMineStrength = 1f;
    [SerializeField] float _baseFuelCapacity = 100f;

    [Header("Fuel Settings")]
    [SerializeField] float _baseFuelBurnRate = 0.05f;
    [SerializeField] float _refuelRate = 0.2f;

    [Header("Machine Parts")]
    [SerializeField] GameObject _drillAnchor;

    [Header("Components")]
    [SerializeField] Collider _collider;

    bool _isRefueling = false;
    float _timeSinceLastHit = 0f;
    Vector2 _bounds;
    MineDirection _currentDirection;

    [Header("Run Time Miner Stats")]
    public float MoveSpeedMultiplier = 1f;
    public float BoosterForceMultiplier = 1f;
    public float MineStrengthMultiplier = 1f;
    public float CurrentFuelCapacity;

    public float RemainingFuel;

    private void Start()
    {
        _bounds = _collider.bounds.extents;
        CurrentFuelCapacity = _baseFuelCapacity;
        RemainingFuel = CurrentFuelCapacity;
        UiController.Instance.SetFuelDisplay(1f);
    }

    public float GetMoveSpeed()
    {
        return _baseMoveSpeed * MoveSpeedMultiplier;
    }

    public float GetBoosterForce()
    {
        return _baseBoosterForce * BoosterForceMultiplier;
    }

    public float GetMineStrength()
    {
        return _baseMineStrength * MineStrengthMultiplier;
    }

    public float GetNormalizedFuelLevel()
    {
        return RemainingFuel / CurrentFuelCapacity;
    }

    public float GetBurnFuelRate()
    {
        return _baseFuelBurnRate;
    }

    GridCell GetContactCell(MineDirection direction)
    {
        if (direction == MineDirection.NONE)
            return null;

        float checkDistance = 0f;
        float contactBufferDistance = 0.35f;
        Vector3 vectorDirection = Vector3.zero;

        switch (direction)
        {
            case MineDirection.Right:
                checkDistance = _bounds.x;
                vectorDirection = Vector3.right;
                break;

            case MineDirection.Left:
                checkDistance = _bounds.x;
                vectorDirection = Vector3.left;
                break;

            case MineDirection.Up:
                checkDistance = _bounds.y;
                vectorDirection = Vector3.up;
                contactBufferDistance = 0.5f;
                break;

            case MineDirection.Down:
                checkDistance = _bounds.y;
                vectorDirection = Vector3.down;
                contactBufferDistance = 0.5f;
                break;
        }

        RaycastHit hit;
        float hitDistance = checkDistance + contactBufferDistance;
        if (Physics.Raycast(transform.position, vectorDirection, out hit, hitDistance))
        {
            Debug.DrawRay(transform.position, vectorDirection * hitDistance, Color.yellow);
            GridCell cell = hit.collider.GetComponent<GridCell>();

            if (cell != null)
            {
                //Debug.Log($"Hit cell: {cell.name}");
                return cell;
            }
        }

        return null;
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, _bounds.y + 0.5f);
    }

    public void Refuel(bool doInstant = false)
    {
        if (_isRefueling)
            return;

        //RemainingFuel = CurrentFuelCapacity;
        //UiController.Instance.SetFuelDisplay(1f);

        if (doInstant)
        {
            RemainingFuel = CurrentFuelCapacity;
            UiController.Instance.SetFuelDisplay(1f);
        } else
        {
            StartCoroutine(DoRefuel());
        }
    }

    public void StopRefuel()
    {
        _isRefueling = false;
    }

    IEnumerator DoRefuel()
    {
        _isRefueling = true;
        float rateMultiplier = 1f;

        while (RemainingFuel <= CurrentFuelCapacity)
        {
            if (!_isRefueling)
                break;

            RemainingFuel += _refuelRate * rateMultiplier;
            UiController.Instance.SetFuelDisplay(GetNormalizedFuelLevel());
            rateMultiplier *= 1.05f;
            yield return new WaitForSeconds(0.01f);
            yield return null;
        }

        yield break;
    }

    public void BurnFuel(float amount)
    {
        RemainingFuel -= amount;
        UiController.Instance.SetFuelDisplay(GetNormalizedFuelLevel());
    }

    public void TryMine()
    {
        if (_timeSinceLastHit < _hitDelay)
            return;

        // TODO: Check block based on current mine direction
        GridCell cell = GetContactCell(_currentDirection);

        if (cell == null)
            return;

        // MINE!
        //Debug.LogWarning($"MINE CELL: {cell.name}");
        _timeSinceLastHit = 0f;
        float dmg = GetMineStrength();
        cell.DealDamage(dmg);
        //Destroy(cell.gameObject);
    }

    public void SetDirection(MineDirection direction)
    {
        _currentDirection = direction;

        // TODO: Animate drill direction
        float toZ = direction switch
        {
            MineDirection.Left => 90f,
            MineDirection.Right => -90f,
            MineDirection.Down => 0f,
            MineDirection.Up => 180f,
            _ => 90f
        };

        float rotateTime = 0.25f;
        Vector3 toRotate = new Vector3(0, -90f, toZ);
        _drillAnchor.transform.DOLocalRotate(toRotate, rotateTime, RotateMode.Fast);
    }

    private void Update()
    {
        _timeSinceLastHit += Time.deltaTime;
    }
}
