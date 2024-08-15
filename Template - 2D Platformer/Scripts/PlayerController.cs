using ScriptableObjectArchitecture;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerControls _input;
    Vector2 _moveVector = Vector2.zero;

    Rigidbody2D _rb;
    bool _isJumping = false;
    bool _isRunning = false;
    bool _isCrouching = false;

    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _jumpForce = 10f;
    [SerializeField] float _runSpeed = 8f;
    [SerializeField] AudioClip _jumpSound;
    [SerializeField] AudioClip _deathSound;
    [SerializeField] GameObject _fireBall;
    [SerializeField] Vector2Reference _playerPosition;

    [Header("Listen to these events...")]
    [SerializeField] GameEvent _onFinishedTimer;
    [SerializeField] GameEvent _onPlayerFell;
    [SerializeField] GameEvent _onReachedFinishLine;

    [Header("Call these events...")]
    [SerializeField] AudioClipGameEvent _onPlaySound;
    [SerializeField] GameEvent _onPlayerDied;
    [SerializeField] GameEvent _onPauseGameToggle;

    void Awake()
    {
        _input = new PlayerControls();
    }

    void EnableInput()
    {
        _input.Enable();
    }

    void DisableInput()
    {
        _input.Disable();
    }

    void OnEnable()
    {
        _input.Enable();
        _input.Player.Move.performed += OnMovePerformed;
        _input.Player.Move.canceled += OnMoveCancelled;
        _input.Player.Fire.performed += Fire;
        _input.Player.Jump.performed += Jump;
        _input.Player.Sprint.performed += OnSprintPerformed;
        _input.Player.Sprint.canceled += OnSprintCancelled;
        _input.Player.Pause.performed += OnPausePerformed;
        _input.Player.Crouch.performed += OnCrouchPerformed;
        _input.Player.Crouch.canceled += OnCrouchPerformed;

        _onFinishedTimer.AddListener(StartDeathSequence);
        _onPlayerFell.AddListener(StartDeathSequence);
        _onReachedFinishLine.AddListener(DisableInput);
    }

    void OnDisable()
    {
        _input.Disable();
        _input.Player.Move.performed -= OnMovePerformed;
        _input.Player.Move.canceled -= OnMoveCancelled;
        _input.Player.Fire.performed -= Fire;
        _input.Player.Jump.performed -= Jump;
        _input.Player.Sprint.performed -= OnSprintPerformed;
        _input.Player.Sprint.canceled -= OnSprintCancelled;
        _input.Player.Pause.performed -= OnPausePerformed;
        _input.Player.Crouch.performed -= OnCrouchPerformed;
        _input.Player.Crouch.canceled -= OnCrouchPerformed;

        _onFinishedTimer.RemoveListener(StartDeathSequence);
        _onPlayerFell.RemoveListener(StartDeathSequence);
        _onReachedFinishLine.RemoveListener(DisableInput);
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        _onPauseGameToggle.Raise();
    }

    private void OnMovePerformed(InputAction.CallbackContext value)
    {
        _moveVector = value.ReadValue<Vector2>();
    }

    private void OnMoveCancelled(InputAction.CallbackContext value)
    {
        _moveVector = Vector2.zero;
    }

    private void Fire(InputAction.CallbackContext value)
    {
        Instantiate(_fireBall, transform.position, transform.rotation);
        _playerPosition.Value = new Vector2(transform.position.x, transform.position.y);
    }

    void Jump(InputAction.CallbackContext value)
    {
        if (!_isJumping)
        {
            _rb.AddForce(new Vector2(0f, _jumpForce), ForceMode2D.Impulse);
            _isJumping = true;
            _onPlaySound.Raise(_jumpSound);
        }
    }

    void OnCrouchPerformed(InputAction.CallbackContext value)
    {
        _isCrouching = true;
    }

    void OnCrouchCancelled(InputAction.CallbackContext value)
    {
        _isCrouching = false;
    }

    void OnSprintPerformed(InputAction.CallbackContext value)
    {
        _isRunning = true;
        Debug.Log(_isRunning);
    }

    void OnSprintCancelled(InputAction.CallbackContext value)
    {
        _isRunning = false;
        Debug.Log(_isRunning);
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        _rb.velocity = new Vector2(_moveVector.x * (_isRunning ? _runSpeed : _moveSpeed), _rb.velocity.y);

        // Rotate the character based on the direction of movement
        if (_moveVector.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (_moveVector.x < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        _playerPosition.Value = new Vector2(transform.position.x, transform.position.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if player is grounded
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            _isJumping = false;
        }
    }

    void StartDeathSequence()
    {
        // Death Animation
        DisableInput();
        _onPlaySound.Raise(_deathSound);
        _onPlayerDied.Raise();
    }

    
}