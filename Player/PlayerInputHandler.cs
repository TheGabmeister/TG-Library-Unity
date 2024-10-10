using UnityEngine;
using UnityEngine.InputSystem;

/*
 * This is a template for a PlayerInputHandler using the new input system.
 * Based on a tutorial by SppedTutor: https://www.youtube.com/watch?v=lclDl-NGUMg
 * Documentation: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.11/manual/Workflow-Actions.html
 */

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] InputActionAsset _inputActionAsset;

    InputAction _moveAction;
    InputAction _jumpAction;
    InputAction _lookAction;
    InputAction _sprintAction;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public float SprintValue { get; private set; }

    private void OnEnable()
    {
        _moveAction.Enable();
        _lookAction.Enable();
        _jumpAction.Enable();
        _sprintAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _lookAction.Disable();
        _jumpAction.Disable();
        _sprintAction.Disable();
    }

    private void Start()
    {
        _moveAction = _inputActionAsset.FindActionMap("Player").FindAction("Move");
        _jumpAction = _inputActionAsset.FindActionMap("Player").FindAction("Jump");
        _lookAction = _inputActionAsset.FindActionMap("Player").FindAction("Look");
        _sprintAction = _inputActionAsset.FindActionMap("Player").FindAction("Sprint");

        _moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        _moveAction.canceled += context => MoveInput = Vector2.zero;

        _lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        _lookAction.canceled += context => LookInput = Vector2.zero;

        _jumpAction.performed += context => JumpTriggered = true;
        _jumpAction.canceled += context => JumpTriggered = false;

        _sprintAction.performed += context => SprintValue = context.ReadValue<float>();
        _sprintAction.canceled += context => SprintValue = 0f;
    }
}