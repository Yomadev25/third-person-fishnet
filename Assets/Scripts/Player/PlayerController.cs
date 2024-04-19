using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;

[RequireComponent(typeof(InputHandler))]
public class PlayerController : NetworkBehaviour
{
    public enum Posture
    {
        Stand,
        Crouch,
        Prone
    }

    [Header("Movement")]
    [SerializeField]
    private float _normalSpeed;
    [SerializeField]
    private float _crouchSpeed;
    [SerializeField]
    private float _proneSpeed;
    [SerializeField]
    private float _sprintMultiplier;
    [SerializeField]
    private float _maxForce;
    [SerializeField]
    private float _jumpForce;

    [Header("Slope Movement")]
    [SerializeField]
    private float _maxSlopeAngle;

    [Header("Ground Check")]
    [SerializeField]
    private Transform _groundCheck;
    [SerializeField]
    private float _characterHeight;
    [SerializeField]
    private LayerMask _groundLayer;

    [Header("Camera Holder")]
    [SerializeField]
    private GameObject _cameraHolder;
    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera _thirdPersonCamera;
    [SerializeField]
    private Transform[] _cameraPositions;
    [SerializeField]
    private float _lookSpeed;
    [SerializeField]
    private float _minTilt;
    [SerializeField]
    private float _maxTilt;

    [Header("References")]
    [SerializeField]
    private Rigidbody _rb;
    [SerializeField]
    private InputHandler _inputHandler;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Transform _orientation;

    private float _speed;
    private Posture _currentPosture;
    private Vector2 _move;
    private Vector2 _look;
    private Vector2 _moveDirection;
    private float _lookRotation;
    private bool _readyToJump;
    private bool _grounded;
    private Camera _camera;
    private RaycastHit _slopeHit;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!_rb)
        {
            Debug.LogErrorFormat("{0} not found.", nameof(Rigidbody));
        }

        if (!_inputHandler)
        {
            Debug.LogErrorFormat("{0} not found.", nameof(InputHandler));
        }

        if (!_animator)
        {
            Debug.LogErrorFormat("{0} not found.", nameof(Animator));
        }

        if (!_cameraHolder)
        {
            Debug.LogError("Camera Holder not found.");
        }

        if (!_thirdPersonCamera)
        {
            Debug.LogErrorFormat("{0} not found.", nameof(Cinemachine.CinemachineVirtualCamera));
        }

        if (!_camera)
        {
            _camera = Camera.main;
        }

        if (base.IsOwner)
        {
            InitInputTrigger();

            _speed = _normalSpeed;
            _thirdPersonCamera.Follow = _cameraPositions[(int)_currentPosture];
            _thirdPersonCamera.LookAt = _cameraPositions[(int)_currentPosture];
        }
        else
        {
            _cameraHolder.SetActive(false);
            this.enabled = false;
        }
    }

    private void Update()
    {
        GetInputValue();
        GroundCheck();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        Rotate();
    }

    private void InitInputTrigger()
    {
        _inputHandler.onCrouch.AddListener(() =>
        {
            if (_currentPosture != Posture.Crouch)
            {
                ChangePosture(Posture.Crouch);
            }
            else
            {
                ChangePosture(Posture.Stand);
            }
        });

        _inputHandler.onProne.AddListener(() =>
        {
            if (_currentPosture != Posture.Prone)
            {
                ChangePosture(Posture.Prone);
            }
            else
            {
                ChangePosture(Posture.Stand);
            }
        });

        _inputHandler.onJump.AddListener(() =>
        {
            Jump();
            Invoke(nameof(ResetJump), 0.5f);
        });
    }

    private void GetInputValue()
    {
        _move = _inputHandler.move;
        _look = _inputHandler.look;
    }

    private void ChangePosture(Posture posture)
    {
        if (_currentPosture == posture) return;
        _speed = 0.5f;

        _animator.SetInteger("Posture", (int)posture);
        _thirdPersonCamera.Follow = _cameraPositions[(int)posture];
        _thirdPersonCamera.LookAt = _cameraPositions[(int)posture];

        _currentPosture = posture;
    }

    //Add this method in animation event
    public void OnChangedPosture()
    {
        switch (_currentPosture)
        {
            case Posture.Stand:
                _speed = _normalSpeed;
                break;
            case Posture.Crouch:
                _speed = _crouchSpeed;
                break;
            case Posture.Prone:
                _speed = _proneSpeed;
                break;
            default:
                break;
        }
    }

    private void Move()
    {
        Vector3 currentVelocity = _rb.velocity;
        Vector3 moveDirection = new Vector3(_move.x, 0, _move.y);

        //Reduce velocity when move backward
        if (moveDirection.z < 0)
        {
            moveDirection.z *= 0.3f;
        }
        moveDirection *= _speed * (_inputHandler.sprint? _sprintMultiplier : 1f);
        
        Vector3 targetVelocity = transform.TransformDirection(moveDirection);
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z); //Let rigidbody's gravity work normally
        Vector3.ClampMagnitude(velocityChange, _maxForce);

        _rb.AddForce(velocityChange, ForceMode.VelocityChange);

        //Add slope force
        if (OnSlope())
        {
            _rb.AddForce(GetSlopeMoveDirection(moveDirection) * _speed, ForceMode.Acceleration);
            _rb.drag = 10f;
        }
        else
        {
            _rb.drag = 0f;
        }

        _animator.SetFloat("Direction X", moveDirection.x);
        _animator.SetFloat("Direction Y", moveDirection.z);
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up * _look.x * _lookSpeed);
        _lookRotation += (-_look.y * _lookSpeed);
        _lookRotation = Mathf.Clamp(_lookRotation, _minTilt, _maxTilt);
        _cameraHolder.transform.eulerAngles = new Vector3(_lookRotation, _cameraHolder.transform.eulerAngles.y, _cameraHolder.transform.eulerAngles.z);
    }

    private void Jump()
    {
        if (!_readyToJump) return;
        if (!_grounded) return;

        _readyToJump = false;

        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }

    private void GroundCheck()
    {
        _grounded = Physics.Raycast(transform.position, Vector3.down, _characterHeight / 2 + 0.2f, _groundLayer);
    }

    private bool OnSlope()
    {
        if (_grounded)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _characterHeight / 2 * 0.3f, _groundLayer))
            {
                float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle < _maxSlopeAngle && angle != 0;
            }
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 moveDirection)
    {
        return Vector3.ProjectOnPlane(moveDirection, _slopeHit.normal).normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (_characterHeight / 2 + 0.2f));
    }
}
