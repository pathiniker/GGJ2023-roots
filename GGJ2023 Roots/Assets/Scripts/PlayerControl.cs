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

    [Header("Miner")]
    [SerializeField] MineMachine _mineMachine;

    [Header("Components")]
    [SerializeField] Rigidbody _rb;

    private void Start()
    {
        _rb.useGravity = true;
    }

    bool KeyIsPressed(KeyCode key)
    {
        return Input.GetKey(key);
    }

    // Mine automatically when making contact with mineable ground

    private void Update()
    {
        Vector3 velocity = Vector3.zero;
        bool isGrounded = _mineMachine.IsGrounded();
        velocity.y = _rb.velocity.y;
        MineDirection direction = MineDirection.NONE;
        UiController.Instance.SetElevationText(transform.position.y);
        float moveSpeed = _mineMachine.GetMoveSpeed();

        if (KeyIsPressed(UP_KEY))
        {
            // Fire booster
            float forceStrength = _mineMachine.GetBoosterForce() * Time.deltaTime;
            Vector3 boostForce = new Vector3(0, Vector3.up.y * forceStrength, 0);
            velocity.y = boostForce.y;
        } else if (KeyIsPressed(DOWN_KEY))
        {
            direction = MineDirection.Down;
        }

        if (KeyIsPressed(LEFT_KEY))
        {
            velocity += Vector3.left * moveSpeed * Time.deltaTime;
            direction = MineDirection.Left;
        } else if (KeyIsPressed(RIGHT_KEY))
        {
            velocity += Vector3.right * moveSpeed * Time.deltaTime;
            direction = MineDirection.Right;
        }

        if (direction != MineDirection.NONE)
        {
            _mineMachine.SetDirection(direction);

            if (isGrounded)
                _mineMachine.TryMine();
        }

        _rb.velocity = velocity;
    }
}
