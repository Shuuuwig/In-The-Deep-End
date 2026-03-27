using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] protected InputActionAsset inputActionsUnity;
    InputAction.CallbackContext _context;
    InputAction _navigateMovesetAction;
    InputAction _confirmAction;
    InputAction _earlyEndAction;
    InputAction _cancelAction;
    InputAction _counterAction;
    protected float _horizontalValue;
    protected float _verticalValue;

    public static event EventHandler<InfoEventArgs<int>> NavigateMovesetEvent;
    public static event EventHandler<InfoEventArgs<bool>> ConfirmActionEvent;
    public static event EventHandler<InfoEventArgs<bool>> EarlyEndActionEvent;
    public static event EventHandler<InfoEventArgs<bool>> CancelActionEvent;
    public static event EventHandler<InfoEventArgs<bool>> CounterActionEvent;

    void Awake()
    {
        _navigateMovesetAction = inputActionsUnity.FindAction("UI/Navigate");
        _confirmAction = inputActionsUnity.FindAction("UI/Submit");
        _earlyEndAction = inputActionsUnity.FindAction("UI/EarlyEnd");
        _cancelAction = inputActionsUnity.FindAction("UI/Cancel");
        _counterAction = inputActionsUnity.FindAction("UI/Counter");
    }

    void OnEnable()
    {
        _navigateMovesetAction.Enable();
        // foreach (var action in _fireActions)
        //     action.Enable();
    }

    void OnDisable()
    {
        _navigateMovesetAction.Disable();
        // foreach (var action in _fireActions)
        //     action.Disable();
    }

    void Update()
    {
        _horizontalValue = _navigateMovesetAction.ReadValue<Vector2>().x;
        _verticalValue = _navigateMovesetAction.ReadValue<Vector2>().x;

        if (_verticalValue != 0)
        {
            if (_navigateMovesetAction.WasPressedThisFrame())
            {
                Debug.Log($"Vertical Value: {_verticalValue}");
                NavigateMovesetEvent?.Invoke(this, new InfoEventArgs<int>((int)_verticalValue));
            }
        }

        if (_confirmAction.WasPressedThisFrame())
        {
            ConfirmActionEvent?.Invoke(this, new InfoEventArgs<bool>(true));
        }

        if (_earlyEndAction.WasPressedThisFrame())
        {
            EarlyEndActionEvent?.Invoke(this, new InfoEventArgs<bool>(true));
        }

        if (_cancelAction.WasPressedThisFrame())
        {
            CancelActionEvent?.Invoke(this, new InfoEventArgs<bool>(true));
        }

        if (_counterAction.WasPressedThisFrame())
        {
            CounterActionEvent?.Invoke(this, new InfoEventArgs<bool>(true));
        }
    }
}