using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerable : MonoBehaviour
{
    [SerializeField] private float walkSpeed; // player�� �ȴ� �ӵ��Դϴ�. 
    [SerializeField] private float runSpeed;  // player�� �ٴ� �ӵ��Դϴ�.
    private float applySpeed;                 // bool�� ���ؼ� ���� �� walkSpeed�� runsSpeed�� ������ �����Դϴ�.

    [SerializeField] private float jumpForce; // ������ ���̸� ������ ũ���Դϴ�.

    // ���� ����
    private bool isRun = false;         // ���� �ٴ� �����ΰ�?
    private bool isGround = true;       // Player�� �ٴ� Pos���� �Ʒ��� Ray�� ���� �浹�� ���� ��� ������ �ν��Ͽ� ������ üũ�ϱ� ���� Bool
    private bool moveBool = false;      // �������� üũ�ϴ� ����. �������� ������ Idle ������ �ִϸ��̼� �ƴϸ� isRun���� �˻��Ѵ�.
    private bool isJump = false;        // ���� ��ҿ��� �������� ���� ���� ����� ��ũ�� ���߱� �߰��� jump Ȯ�� bool
    
    // ��ŷ ����
    public bool freeze;                 // ��ŷ�ÿ� �̵��ӵ��� zero�� ������Ű�� ���� bool
    public bool activeGrapple;          // ��ŷ �߿� �̵������� ���� ���� bool
    private bool enableMovementOnNextTouch; // Y ���������� �̵��� �� �浹 �˻簡 �߻��ϸ� �̵� �� �� �ְ� Ȯ�����ִ� bool�Դϴ�.
    private Vector3 velocityToSet;          // ������ �̵��� ���� �ӵ��� �����ϱ� ���� ����

    // �������� Ȯ�� �ݶ��̴�
    private CapsuleCollider capsuleCollider;

    [SerializeField] private float lookSensitivity; // ī�޶��� �ΰ���

    [SerializeField] private float cameraRotationLimit; // ī�޶��� �ִ� ����
    private float currentCameraRotationX = 0f; // 0�� ������ �ٶ󺸴� ��, 45(���� ��ŭ ���� ����)

    [SerializeField] private Camera theCamera; // GameObject.Find����� ������� �ʰ� Hierarchy���� ���� ī�޶� �Ҵ��ϱ� ���� ����

    Rigidbody rigid; // ���� ������ ���� rigidbody  
    Animator anim;   // �ִϸ��̼� ������ ���� animator


    void Start()
    {
        // �ش� ��ũ��Ʈ�� GameObject inspectorâ���� GetComponent�� �ʱ�ȭ ���ش�.
        capsuleCollider = GetComponent<CapsuleCollider>(); 
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        // �ʱ��� �̵��ӵ��� �ȴ� �ӵ��� �Ҵ��Ͽ����ϴ�.
        applySpeed = walkSpeed;
    }

    void Update()
    {
        // ��ŷ ���� �̵��� �������ش�.
        if (freeze)
        {
            rigid.velocity = Vector3.zero;
        }
        // player�� ���(�̵�, ����, ī�޶� ȸ��)�� ������ ���� ���� 
        else
        {
            IsGround();
            TryJump();
            TryRun();
            CameraRotation();
            CharacterRotation();
        }

        // player�� ���� ��ũ�� ���߱� ���� bool ����
        if (isGround && isJump)
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    // ������ �������� ȣ���Ͽ� ����ȿ���� ����� Object���� �浹 �˻縦 ������ �ϱ� ���� FixedUpdate���� �������� ������� �־����ϴ�.
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
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y+ 0.1f);  // Vector3 transform�� ����.. ���� ���� ���� ���� ���̰� �ִ�.!
            // ĸ���ݶ��̴��� bounds(����)[ĳ������ Ű] y���� extents(half)��
            // ��Ȯ�� ������ �ϸ� ��� ���� ��ҿ��� ������ �߻��� �� �ִ� => ���������� ���� ���� �����ش�.
    }

    // ������ �����ϱ� ���� ������ Ȯ�� �ϱ� ���� �޼����Դϴ�.
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space)&& isGround)
        {
            Jump();
        }
    }

    // ������ ����ȿ���� ����Ǵ� �޼����Դϴ�.
    private void Jump()
    {
        anim.SetBool("isJump", true);
        rigid.velocity = transform.up * jumpForce;

        Invoke("JumpOut", 0.5f);
        // ������ ���������� ���Ͽ� ������ Check�� �� ������ ���ڸ��� ������ ĵ���Ǵ� �������� �߻��Ͽ����ϴ�.
        // �̸� �ذ��ϱ� ���ؼ� ��� ���� �ö��� �� �˻��ϱ� ���Ͽ� Invoke�� ���Ͽ� �����ð��� �߰��Ͽ����ϴ�. 
    }

    // Invoke�� Ȱ���ϱ� ���� �޼����Դϴ�.
    private void JumpOut()
    {
        isJump = true;
    }

    // applySpeed�� Walk �Ǵ� Run�� �������ִ� �޼����Դϴ�.
    private void TryRun()
    {
        // ��ŷ ���̶�� �̵��� �����ϴ�.
        if (activeGrapple) return;

        // L_Shift�� �Է��ϸ� ĳ���Ͱ� �޸��ϴ�.
        if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("isRun", true);
            Running();
        }
        // L_ShiftŰ�� �Է��� �ߴܵǸ� �ȴ� ���°� �˴ϴ�.(isRun�� �ƴϸ� �ȴ� ������ �����Ǿ����ϴ�.)
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

        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); // �����¿� �Է°� ����
        moveBool = (moveInput != Vector2.zero); // Ű���� �Է����� ������ false ��� �װ� �ƴϸ� true

        if (moveBool)
        {
            anim.SetBool("isWalk", true); // �ִϸ��̼� ���

            float _moveDirX = Input.GetAxisRaw("Horizontal"); //  ���� -1, �⺻ 0 , ������ 1���� ����Ѵ�.
            float _moveDirZ = Input.GetAxisRaw("Vertical"); // �� 1, �⺻ 0 , �Ʒ� -1 ���� ����Ѵ�.

            Vector3 _moveHorizontal = transform.right * _moveDirX;    //(1,0,0) / (0,0,0) / (-1,0,0) ���Ͱ��� ���
            Vector3 _moveVertical = transform.forward * _moveDirZ;    // (0,0,1) / (0,0,0) / (0,0,-1) ���Ͱ��� ���

            Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;    // (1,0,0) (0,0,1) = (1,0,1) 2 ����ȭ�� ���� 1�ʿ� �󸶸�ŭ ������ �� �ִ��� ������ �ʱ�ȭ ���ش�.

            rigid.MovePosition(transform.position + _velocity * Time.deltaTime); // rigid�� Ȱ���� ĳ���� �̵� ���� ��ġ���� �� frame���� �̵���Ų��.  Tiem.deltaTime�� ���� �뷫 0.016�̴�. 
        }
    }

    private void CharacterRotation() // ĳ���� �¿� ȸ��
    {
        float _yRotation = Input.GetAxisRaw("Mouse X"); // y��� ȸ�� ���Ѿ� �¿� ȸ���� �Ѵ�. yRotation���� �����Ű�� ����.
        Vector3 _chracterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(_chracterRotationY)); // ���콺�� ���� ������ ���� ���ʹϾ����� ��������ִ� �Լ�.
        //theCamera.transform.localEulerAngles = _chracterRotationY;
    }

    private void CameraRotation() // ���콺 ���� ������
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y"); // ���콺�� 2���� X,Y���� ����Ƽ�� 3���� ������ X��.(X,Y,Z)�� X
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); // ������ �����Ǵ� ����? ī�޶� ���� ������ ������ؼ�. �ö󰥶� ���ְ� �������� �����ش�.

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    // CaluateJumpVelocity : ���� ��ġ, Ÿ���� ��ġ, Ÿ���� ���̸� ���ؼ� ������ �̵� �Ÿ��� �����ϴ� �޼����Դϴ�.
    // �� ����� �����ϱ� ���� ������ ����� �� �������� ���Ͽ� ��Ʃ�� ������ ������ �״�� ������ ����Ͽ����ϴ�.
    public void JumpToPoSition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);     // CalculateJumpVelocity ������ �Ϸ�� �� �ӵ��� ���� ��Ű�� ���� Invoke ���

        Invoke(nameof(ResetRestrictions), 3f); // �̵� ������ Invoke�� ���Ͽ� ����
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
