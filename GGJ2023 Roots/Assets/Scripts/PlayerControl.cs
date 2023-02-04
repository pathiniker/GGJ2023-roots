using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MineDirection
{
    NONE,
    Left,
    Right,
    Down,
    Up
}

public class PlayerControl : MonoBehaviour
{
    const KeyCode UP_KEY = KeyCode.W;
    const KeyCode DOWN_KEY = KeyCode.S;
    const KeyCode LEFT_KEY = KeyCode.A;
    const KeyCode RIGHT_KEY = KeyCode.D;

    [Header("Settings")]
    [SerializeField] float _moveSpeed = 1f;
    [SerializeField] float _boosterForce = 1f;

    [Header("Components")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] Collider _collider;

    float _distanceToGround;
    float _sideDistance;
    MineDirection _currentDirection = MineDirection.Right;

    private void Start()
    {
        _distanceToGround = _collider.bounds.extents.y;
        _sideDistance = _collider.bounds.extents.x;
        _rb.useGravity = true;
    }

    bool KeyIsPressed(KeyCode key)
    {
        return Input.GetKey(key);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, _distanceToGround + 0.1f);
    }

    GridCell GetContactCell(MineDirection direction)
    {
        if (direction == MineDirection.NONE)
            return null;

        float checkDistance = 0f;
        float contactBufferDistance = 0.1f;
        Vector3 vectorDirection = Vector3.zero;

        switch (direction)
        {
            case MineDirection.Right:
                checkDistance = _sideDistance;
                vectorDirection = Vector3.right;
                break;

            case MineDirection.Left:
                checkDistance = _sideDistance;
                vectorDirection = Vector3.left;
                break;

            case MineDirection.Up:
                checkDistance = _distanceToGround;
                vectorDirection = Vector3.up;
                break;

            case MineDirection.Down:
                checkDistance = _distanceToGround;
                vectorDirection = Vector3.down;
                break;

            default:
                vectorDirection = Vector3.zero;
                break;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, vectorDirection, out hit, checkDistance + contactBufferDistance))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            GridCell cell = hit.collider.GetComponent<GridCell>();

            if (cell != null)
            {
                //Debug.Log($"Hit cell: {cell.name}");
                return cell;
            }
        }

        return null;
    }

    void TryMine()
    {
        // TODO: Check block based on current mine direction
        GridCell cell = GetContactCell(_currentDirection);

        if (cell == null)
            return;

        // MINE!
        Debug.LogWarning($"MINE CELL: {cell.name}");
        Destroy(cell.gameObject);
    }

    // Mine automatically when making contact with mineable ground

    private void Update()
    {
        Vector3 velocity = Vector3.zero;
        velocity.y = _rb.velocity.y;
        MineDirection direction = MineDirection.NONE;

        if (KeyIsPressed(UP_KEY))
        {
            // Fire booster
            Vector3 boostForce = new Vector3(0, Vector3.up.y * _boosterForce, 0);
            boostForce *= Time.deltaTime;
            velocity.y = boostForce.y;
        }

        if (KeyIsPressed(DOWN_KEY))
        {
            // Drill down, if is on ground
            // Else, do nothing
            direction = MineDirection.Down;
        }

        if (KeyIsPressed(LEFT_KEY))
        {
            // Move left
            // If in contact with unminded ground, do mine action
            velocity += Vector3.left * _moveSpeed * Time.deltaTime;
            direction = MineDirection.Left;
        }

        if (KeyIsPressed(RIGHT_KEY))
        {
            // Move right
            // If in contact with unminded ground, do mine action
            velocity += Vector3.right * _moveSpeed * Time.deltaTime;
            direction = MineDirection.Right;
        }

        if (direction != MineDirection.NONE)
        {
            _currentDirection = direction;
            TryMine();
        }

        _rb.velocity = velocity;
    }
}
