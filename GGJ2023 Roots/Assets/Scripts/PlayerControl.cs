using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    bool _isGrounded = true;

    bool KeyIsPressed(KeyCode key)
    {
        return Input.GetKey(key);
    }

    bool IsGrounded()
    {
        // TODO: Detect if miner object is landed on a surface

        return true;
    }

    // Mine automatically when making contact with mineable ground

    private void Update()
    {
        Vector3 velocity = Vector3.zero;
        velocity.y = _rb.velocity.y;

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
        }

        if (KeyIsPressed(LEFT_KEY))
        {
            // Move left
            // If in contact with unminded ground, do mine action
            velocity += Vector3.left * _moveSpeed * Time.deltaTime;
        }

        if (KeyIsPressed(RIGHT_KEY))
        {
            // Move right
            // If in contact with unminded ground, do mine action
            velocity += Vector3.right * _moveSpeed * Time.deltaTime;
        }

        _rb.velocity = velocity;
    }
}
