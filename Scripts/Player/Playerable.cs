using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerable : MonoBehaviour
{
    [SerializeField] private float walkSpeed; // player의 걷는 속도입니다. 
    [SerializeField] private float runSpeed;  // player가 뛰는 속도입니다.
    private float applySpeed;                 // bool에 의해서 결정 될 walkSpeed나 runsSpeed를 가져올 변수입니다.

    [SerializeField] private float jumpForce; // 점프의 높이를 결정할 크기입니다.

    // 상태 변수
    private bool isRun = false;         // 현재 뛰는 상태인가?
    private bool isGround = true;       // Player의 바닥 Pos에서 아래로 Ray를 쏴서 충돌이 있을 경우 땅으로 인식하여 점프를 체크하기 위한 Bool
    private bool moveBool = false;      // 움직임을 체크하는 변수. 움직임이 없으면 Idle 상태의 애니메이션 아니면 isRun인지 검사한다.
    private bool isJump = false;        // 높은 장소에서 떨어지는 등의 점프 모션의 싱크를 맞추기 추가한 jump 확인 bool
    
    // 후킹 관리
    public bool freeze;                 // 후킹시에 이동속도를 zero로 고정시키기 위한 bool
    public bool activeGrapple;          // 후킹 중에 이동로직을 막기 위한 bool
    private bool enableMovementOnNextTouch; // Y 포물선으로 이동한 후 충돌 검사가 발생하면 이동 할 수 있게 확인해주는 bool입니다.
    private Vector3 velocityToSet;          // 포물선 이동을 위한 속도를 저장하기 위한 변수

    // 착지여부 확인 콜라이더
    private CapsuleCollider capsuleCollider;

    [SerializeField] private float lookSensitivity; // 카메라의 민감도

    [SerializeField] private float cameraRotationLimit; // 카메라의 최대 각도
    private float currentCameraRotationX = 0f; // 0은 정면을 바라보는 값, 45(각도 만큼 위로 본다)

    [SerializeField] private Camera theCamera; // GameObject.Find기능을 사용하지 않고 Hierarchy에서 직접 카메라를 할당하기 위해 선언

    Rigidbody rigid; // 물리 구현을 위한 rigidbody  
    Animator anim;   // 애니메이션 구현을 위한 animator


    void Start()
    {
        // 해당 스크립트의 GameObject inspector창에서 GetComponent로 초기화 해준다.
        capsuleCollider = GetComponent<CapsuleCollider>(); 
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        // 초기의 이동속도는 걷는 속도로 할당하였습니다.
        applySpeed = walkSpeed;
    }

    void Update()
    {
        // 후킹 시의 이동을 제한해준다.
        if (freeze)
        {
            rigid.velocity = Vector3.zero;
        }
        // player의 기능(이동, 점프, 카메라 회전)을 프레임 마다 실행 
        else
        {
            IsGround();
            TryJump();
            TryRun();
            CameraRotation();
            CharacterRotation();
        }

        // player의 점프 싱크를 맞추기 위한 bool 관리
        if (isGround && isJump)
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    // 일정한 간격으로 호출하여 물리효과가 적용된 Object과의 충돌 검사를 원할히 하기 위해 FixedUpdate에서 움직임을 실행시켜 주었습니다.
    private void FixedUpdate()
    {
        if (freeze)
        {

        }
        else
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                Move();
            }
            else
            {
                anim.SetBool("isWalk", false);
            }
        }    
    }



    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y+ 0.1f);  // Vector3 transform의 차이.. 월드 값과 로컬 값의 차이가 있다.!
            // 캡슐콜라이더의 bounds(영역)[캐릭터의 키] y값의 extents(half)값
            // 정확히 반으로 하면 계단 같은 장소에서 문제가 발생할 수 있다 => 오차수정을 위해 값을 더해준다.
    }

    // 점프를 구현하기 전에 변수를 확인 하기 위한 메서드입니다.
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space)&& isGround)
        {
            Jump();
        }
    }

    // 점프의 물리효과가 적용되는 메서드입니다.
    private void Jump()
    {
        anim.SetBool("isJump", true);
        rigid.velocity = transform.up * jumpForce;

        Invoke("JumpOut", 0.5f);
        // 땅과의 물리연산을 통하여 점프를 Check할 때 점프를 하자마자 점프가 캔슬되는 문제점이 발생하였습니다.
        // 이를 해결하기 위해서 어느 정도 올라갔을 때 검사하기 위하여 Invoke를 통하여 지연시간을 추가하였습니다. 
    }

    // Invoke를 활용하기 위한 메서드입니다.
    private void JumpOut()
    {
        isJump = true;
    }

    // applySpeed에 Walk 또는 Run을 결정해주는 메서드입니다.
    private void TryRun()
    {
        // 후킹 중이라면 이동을 막습니다.
        if (activeGrapple) return;

        // L_Shift를 입력하면 캐릭터가 달립니다.
        if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("isRun", true);
            Running();
        }
        // L_Shift키의 입력이 중단되면 걷는 상태가 됩니다.(isRun이 아니면 걷는 것으로 구성되었습니다.)
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            anim.SetBool("isRun", false);
            RunningCancel();
        }
    }

    private void Running()
    {
        isRun = true;
        applySpeed = runSpeed;
    }

    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    private void Move()
    {
        if (activeGrapple) return;

        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); // 상하좌우 입력값 감지
        moveBool = (moveInput != Vector2.zero); // 키값을 입력하지 않으면 false 출력 그게 아니면 true

        if (moveBool)
        {
            anim.SetBool("isWalk", true); // 애니메이션 출력

            float _moveDirX = Input.GetAxisRaw("Horizontal"); //  왼쪽 -1, 기본 0 , 오른쪽 1값을 출력한다.
            float _moveDirZ = Input.GetAxisRaw("Vertical"); // 위 1, 기본 0 , 아래 -1 값을 출력한다.

            Vector3 _moveHorizontal = transform.right * _moveDirX;    //(1,0,0) / (0,0,0) / (-1,0,0) 백터값이 출력
            Vector3 _moveVertical = transform.forward * _moveDirZ;    // (0,0,1) / (0,0,0) / (0,0,-1) 백터값이 출력

            Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;    // (1,0,0) (0,0,1) = (1,0,1) 2 정규화를 통해 1초에 얼마만큼 움직일 수 있는지 값으로 초기화 해준다.

            rigid.MovePosition(transform.position + _velocity * Time.deltaTime); // rigid를 활용한 캐릭터 이동 현재 위치에서 매 frame마다 이동시킨다.  Tiem.deltaTime의 값은 대략 0.016이다. 
        }
    }

    private void CharacterRotation() // 캐릭터 좌우 회전
    {
        float _yRotation = Input.GetAxisRaw("Mouse X"); // y축로 회전 시켜야 좌우 회전을 한다. yRotation값을 변경시키는 이유.
        Vector3 _chracterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(_chracterRotationY)); // 마우스에 의해 정해진 값을 쿼터니언값으로 변경시켜주는 함수.
        //theCamera.transform.localEulerAngles = _chracterRotationY;
    }

    private void CameraRotation() // 마우스 상하 움직임
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y"); // 마우스는 2차원 X,Y공간 유니티는 3차원 공간의 X값.(X,Y,Z)중 X
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); // 각도가 반전되는 이유? 카메라 각의 반전을 고려안해서. 올라갈때 빼주고 내려갈때 더해준다.

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    // CaluateJumpVelocity : 나의 위치, 타겟의 위치, 타겟의 높이를 통해서 포물선 이동 거리를 구현하는 메서드입니다.
    // 이 결과를 도출하기 위한 수학적 계산은 잘 이해하지 못하여 유튜브 영상의 로직을 그대로 가져와 사용하였습니다.
    public void JumpToPoSition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);     // CalculateJumpVelocity 연산이 완료된 후 속도를 적용 시키기 위해 Invoke 사용

        Invoke(nameof(ResetRestrictions), 3f); // 이동 제한을 Invoke를 통하여 관리
    }  

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rigid.velocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponentInChildren<Hooking>().StopGrapple();  
        }
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float graviry = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * graviry * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / graviry)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / graviry));

        return velocityXZ + velocityY;
    }
}
