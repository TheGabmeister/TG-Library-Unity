using System;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * This is a template for a PlayerInputHandler using the new input system.
 * Based on a tutorial by SppedTutor: https://www.youtube.com/watch?v=lclDl-NGUMg
 * Documentation: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.11/manual/Workflow-Actions.html
 */

[SerializeField] InputActionAsset _inputActionAsset;

InputAction _moveAction;
InputAction _jumpAction;
InputAction _lookAction;
InputAction _sprintAction;
InputAction _fireAction;

public Action FireAction = () => { };

public Vector2 MoveInput { get; private set; }
public Vector2 LookInput { get; private set; }
public bool JumpTriggered { get; private set; }
public float SprintValue { get; private set; }

private void OnEnable()
{
    _inputActionAsset.Enable();
}

private void OnDisable()
{
    _inputActionAsset.Disable();
}

private void Start()
{
    _moveAction = _inputActionAsset.FindActionMap("Player").FindAction("Move");
    _jumpAction = _inputActionAsset.FindActionMap("Player").FindAction("Jump");
    _lookAction = _inputActionAsset.FindActionMap("Player").FindAction("Look");
    _sprintAction = _inputActionAsset.FindActionMap("Player").FindAction("Sprint");
    _fireAction = _inputActionAsset.FindActionMap("Player").FindAction("Fire");

    _moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
    _moveAction.canceled += context => MoveInput = Vector2.zero;

    _lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
    _lookAction.canceled += context => LookInput = Vector2.zero;

    _jumpAction.performed += context => JumpTriggered = true;
    _jumpAction.canceled += context => JumpTriggered = false;

    _sprintAction.performed += context => SprintValue = context.ReadValue<float>();
    _sprintAction.canceled += context => SprintValue = 0f;

    _fireAction.started += context => Fire(context);
}

void Fire(InputAction.CallbackContext context)
{
    FireAction?.Invoke();
}