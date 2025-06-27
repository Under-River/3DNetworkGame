using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveAbility : PlayerAbility, IPunObservable
{
    public float SprintStaminaCostPerSecond = 10f;
    public float JumpStaminaCost = 10f;
	private float _yVelocity = 0f;
    private Vector3 _moveDir = Vector3.zero;
    private float _currentSpeed = 1f;
    private Vector3 _receivePosition = Vector3.zero;
    private Quaternion _receiveRotation = Quaternion.identity;
    public bool IsSprinting { get; private set; } = false;

    private CharacterController _characterController;
    private Camera _camera;
    private Animator _animator;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _camera = Camera.main;
        _animator = GetComponent<Animator>();
    }
    
    private Vector2 _inputAxis;

    private void Update()
    {
        if(!_photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, _receivePosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, _receiveRotation, Time.deltaTime * 10f);
            return;
        }

        UpdateMoveDirection();
        SetStaminaSprinting();
        CancelSprint();
        SetAnimator();
        Gravity();
        _characterController.Move(_moveDir * _owner.Stat.MoveSpeed * _currentSpeed * Time.deltaTime);
    }

    private void UpdateMoveDirection()
    {
        Vector3 forward = _camera.transform.forward;
        Vector3 right = _camera.transform.right;

        forward.y = 0;
        right.y = 0;

        forward = forward.normalized;
        right = right.normalized;

        _moveDir = forward * _inputAxis.y + right * _inputAxis.x;
    }

    public void InputAxis(InputAction.CallbackContext context)
    {
        if(!_photonView.IsMine) return;

        _inputAxis = context.ReadValue<Vector2>();
    }
    private void Gravity()
    {
        _yVelocity += Physics.gravity.y * Time.deltaTime;
        _moveDir.y = _yVelocity;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && _characterController.isGrounded
        && _owner.Stat.Stamina > JumpStaminaCost)
        {
            _owner.GetAbility<PlayerStamina>().UseStamina(JumpStaminaCost);
            _yVelocity = _owner.Stat.JumpPower;
            _animator.SetTrigger("Jump");
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if(context.performed && _owner.Stat.Stamina > 0)
        {
            _currentSpeed = _owner.Stat.SprintMultiplier;
            IsSprinting = true;
        }
        else if(context.canceled)
        {
            _currentSpeed = 1f;
            IsSprinting = false;
        }
    }
    public void SetStaminaSprinting()
    {
        if(!IsSprinting) return;

        _owner.GetAbility<PlayerStamina>().UseStaminaPerSecond(SprintStaminaCostPerSecond);
    }
    private void CancelSprint()
    {
        if(_owner.Stat.Stamina <= 0)
        {
            _currentSpeed = 1f;
            IsSprinting = false;
        }
    }
    
    public void SetAnimator()
    {
        if(_characterController.isGrounded)
        {
            _animator.SetFloat("Move", _moveDir.magnitude  * _currentSpeed);
            _animator.SetBool("IsGround", true);
        }
        else
        {
            _animator.SetBool("IsGround", false);
        }
    }

    // 데이터 동기화를 위한 데이터 전송 및 수신 기능
    // stream : 서버에서 주고받을 데이터가 담겨있는 변수
    // info : 송수신 성공/실패 여부에 대한 로그
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            // 데이터를 전송하는 상황 -> 데이터를 보내주면 되고
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if(stream.IsReading)
        {
            // 데이터를 수신하는 상황 -> 받은 데이터를 세팅하면 됩니다.
            // 보내준 순서대로 받는다.
            _receivePosition = (Vector3)stream.ReceiveNext();
            _receiveRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}