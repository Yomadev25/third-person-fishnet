using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInputHud : MonoBehaviour
{
    [Header("Mobile Input")]
    [SerializeField]
    private Joystick _moveJoystick;
    [SerializeField]
    private Joystick _lookJoystick;
    [SerializeField]
    private Button _jumpButton;
    [SerializeField]
    private Button _crouchButton;
    [SerializeField]
    private Button _proneButton;

    public static event Action<Vector2> onMove;
    public static event Action<Vector2> onLook;
    public static event Action onJump;
    public static event Action onCrouch;
    public static event Action onProne;

    private void Awake()
    {
        if (_moveJoystick)
        {
            _moveJoystick.onDrag += OnMove;
        }
        else
        {
            Debug.LogErrorFormat("{0} not found.", nameof(Joystick));
        }

        if (_lookJoystick)
        {
            _lookJoystick.onDrag += OnLook;
        }
        else
        {
            Debug.LogErrorFormat("{0} not found.", nameof(Joystick));
        }

        if (_jumpButton)
        {
            _jumpButton.onClick.AddListener(OnJump);
        }
        else
        {
            Debug.LogErrorFormat("{0} not found.", nameof(Button));
        }

        if (_crouchButton)
        {
            _crouchButton.onClick.AddListener(OnCrouch);
        }
        else
        {
            Debug.LogErrorFormat("{0} not found.", nameof(Button));
        }

        if (_proneButton)
        {
            _proneButton.onClick.AddListener(OnProne);
        }
        else
        {
            Debug.LogErrorFormat("{0} not found.", nameof(Button));
        }
    }

    private void OnDestroy()
    {
        _moveJoystick.onDrag -= OnMove;
        _lookJoystick.onDrag -= OnLook;
    }

    private void OnMove()
    {
        onMove?.Invoke(_moveJoystick.Direction);
    }

    private void OnLook()
    {
        onLook?.Invoke(_lookJoystick.Direction);
    }

    private void OnJump()
    {
        onJump?.Invoke();
    }

    private void OnCrouch()
    {
        onCrouch?.Invoke();
    }

    private void OnProne()
    {
        onProne?.Invoke();
    }
}
