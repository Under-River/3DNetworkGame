using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotateAbility : PlayerAbility
{
    // 목표 : 마우스를 조작하면 캐릭터/카메라를 그 방향으로 회전시키고 싶다.
    public Transform CameraRoot;

    // 마우스 입력값을 누적할 변수
    private float _mx;
    private float _my;

    private InputAction _rotateAction;

    private void Start()
    {
        if(_photonView.IsMine)
        {
            CinemachineCamera camera = GameObject.FindWithTag("FollowCamera").GetComponent<CinemachineCamera>();
            camera.Follow = CameraRoot;
            
            _rotateAction = GetComponent<PlayerInput>().actions["Camera Rotate"];
        }
    }

    private void Update()
    {
        if(!_photonView.IsMine) return;

        if (_rotateAction == null) return;

        Vector2 rotateInput = _rotateAction.ReadValue<Vector2>();
        
        if (rotateInput.magnitude > 0.1f)
        {
            ProcessRotation(rotateInput.x, rotateInput.y);
        }
    }

    private void ProcessRotation(float x, float y)
    {
        _mx += x * _owner.Stat.RotateSpeed * Time.deltaTime;
        _my -= y * _owner.Stat.RotateSpeed * Time.deltaTime;

        _my = Mathf.Clamp(_my, -80f, 80f);

        transform.eulerAngles = new Vector3(0, _mx, 0);
        CameraRoot.localEulerAngles = new Vector3(_my, 0, 0);
    }
}
