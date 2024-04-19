using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    [Header("Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool sprint;
    public UnityEvent onJump;
    public UnityEvent onCrouch;
    public UnityEvent onProne;

    private void OnEnable()
    {
        SubscribeMobileInputHud();
    }

    private void OnDestroy()
    {
        UnsubscribeMobileInputHud();
    }

    public void MoveInput(Vector2 moveDirection)
    {
        move = moveDirection;
    }

    public void LookInput(Vector2 lookDirection)
    {
        look = lookDirection;
    }

    public void JumpInput()
    {
        onJump?.Invoke();
    }

    public void SprintInput(bool isSprint)
    {
        sprint = isSprint;
    }

    public void CrouchInput()
    {
        onCrouch?.Invoke();
    }

    public void ProneInput()
    {
        onProne?.Invoke();
    }

    #region PLAYER INPUT ACTION BEHAVIOURS
    //Be call from Player Input Action's sending message behaviour
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        LookInput(value.Get<Vector2>());
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            JumpInput();
        }
    }

    public void OnCrouch(InputValue value)
    {
        if (value.isPressed)
        {
            CrouchInput();
        }
    }

    public void OnProne(InputValue value)
    {
        if (value.isPressed)
        {
            ProneInput();
        }
    }
    #endregion

    #region MOBILE INPUT ACTION
    private void SubscribeMobileInputHud()
    {
        MobileInputHud.onMove += MoveInput;
        MobileInputHud.onLook += LookInput;
        MobileInputHud.onJump += JumpInput;
        MobileInputHud.onCrouch += CrouchInput;
        MobileInputHud.onProne += ProneInput;
    }

    private void UnsubscribeMobileInputHud()
    {
        MobileInputHud.onMove -= MoveInput;
        MobileInputHud.onLook -= LookInput;
        MobileInputHud.onJump -= JumpInput;
        MobileInputHud.onCrouch -= CrouchInput;
        MobileInputHud.onProne -= ProneInput;
    }
    #endregion
}
