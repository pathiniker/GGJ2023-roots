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

    bool _isBoosting = false;
    bool _isGrounded = false;
    bool _upPressed = false;
    bool _downPressed = false;
    bool _leftPressed = false;
    bool _rightPressed = false;
    DepthLevel _currentDepthLevel = DepthLevel.GroundLevel;

    public MineMachine MineMachine { get { return _mineMachine; } }

    private void Start()
    {
        _rb.useGravity = true;
    }

    bool KeyIsPressed(KeyCode key)
    {
        return Input.GetKey(key);
    }

    public DepthData GetCurrentDepthData()
    {
        // TODO
        // Get current player Y
        // Return Depth Level of greatest value that is less than current player Y

        int y = Mathf.FloorToInt(transform.position.y);
        DepthData data = GameController.Instance.GridGenerator.GetDepthDataForY(y);

        return data;
    }

    // Mine automatically when making contact with mineable ground

    private void Update()
    {
        _upPressed = KeyIsPressed(UP_KEY);
        _downPressed = KeyIsPressed(DOWN_KEY);
        _rightPressed = KeyIsPressed(RIGHT_KEY);
        _leftPressed = KeyIsPressed(LEFT_KEY);
    }

    private void FixedUpdate()
    {
        DepthData currentLevel = GetCurrentDepthData();
        UiController.Instance.SetDepthLevelName(currentLevel.LevelName);
        if (currentLevel.Level != _currentDepthLevel)
        {
            int previousIdx = (int)_currentDepthLevel;
            int newIdx = (int)currentLevel.Level;

            if (newIdx > previousIdx)
            {
                // Deeper, add new audio
                AudioManager.Instance.FadeAudioForDepth(true, currentLevel.Level);
            }
            else
            {
                // Higher, remove previous audio
                AudioManager.Instance.FadeAudioForDepth(false, _currentDepthLevel);
            }
        }

        _currentDepthLevel = currentLevel.Level;

        Vector3 velocity = Vector3.zero;
        bool isGrounded = _mineMachine.IsGrounded();
        velocity.y = _rb.velocity.y;
        MineDirection direction = MineDirection.NONE;
        float moveSpeed = _mineMachine.GetMoveSpeed();

        if (isGrounded && !_isGrounded)
        {
            // Just landed. Shake screen / deal damage based on velocity
            //Debug.Log($"Landing velocity: {_rb.velocity}");

            float yVelocity = _rb.velocity.y;

            if (yVelocity < -9.8f)
            {
                float shake = yVelocity / -20f;
                shake = Mathf.Min(shake, 1f);
                CameraController.Instance.DoShake(shake);
                //Debug.Log("Shake: " + shake);
            }
        }

        float fuelToBurn = 0f;
        float fuelBurnRate = _mineMachine.GetBurnFuelRate();
        bool isBoosting = false;

        float remainingFuel = _mineMachine.RemainingFuel;

        if (remainingFuel <= 0f)
        {
            //Debug.Log("Out of fuel!");
            return;
        }

        if (_upPressed)
        {
            // Fire booster
            float forceStrength = _mineMachine.GetBoosterForce() * Time.deltaTime;
            Vector3 boostForce = new Vector3(0, Vector3.up.y * forceStrength, 0);
            velocity.y = boostForce.y;
            isBoosting = true;
            fuelToBurn = fuelBurnRate * 1.5f;
        }
        else if (_downPressed)
        {
            direction = MineDirection.Down;
            fuelToBurn = fuelBurnRate;
        }

        if (_leftPressed)
        {
            velocity += Vector3.left * moveSpeed * Time.deltaTime;
            direction = MineDirection.Left;

            if (!isBoosting)
                fuelToBurn = fuelBurnRate;
        }
        else if (_rightPressed)
        {
            velocity += Vector3.right * moveSpeed * Time.deltaTime;
            direction = MineDirection.Right;

            if (!isBoosting)
                fuelToBurn = fuelBurnRate;
        }

        if (isBoosting || direction != MineDirection.NONE)
            _mineMachine.BurnFuel(fuelToBurn);

        if (isBoosting && !_isBoosting)
        {
            _mineMachine.ToggleBoosters(true);
        }
        else if (!isBoosting && _isBoosting)
        {
            _mineMachine.ToggleBoosters(false);
        }

        _isBoosting = isBoosting;

        if (direction != MineDirection.NONE)
        {
            _mineMachine.SetDirection(direction);

            switch (direction)
            {
                case MineDirection.Left:
                case MineDirection.Right:
                    _mineMachine.DoWheelRotation(direction, isGrounded);
                    break;
            }

            if (isGrounded)
                _mineMachine.TryMine();
        }

        _rb.velocity = velocity;

        float elevation = transform.position.y;
        UiController.Instance.SetElevationText(elevation);
        if (elevation > 0f)
        {
            _mineMachine.Refuel();
        }
        else
        {
            _mineMachine.StopRefuel();
        }

        _isGrounded = isGrounded;
    }
}
