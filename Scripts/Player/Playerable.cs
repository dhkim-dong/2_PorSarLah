using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerable : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    private float applySpeed;

    [SerializeField] private float jumpForce;

    // ���� ����
    private bool isRun = false;
    private bool isGround = true;
    private bool isWall = false;
    private bool moveBool = false;
    private bool isJump = false;
    public bool freeze;
    public bool activeGrapple;

    // �������� Ȯ�� �ݶ��̴�2
    private CapsuleCollider capsuleCollider;

    [SerializeField] private float lookSensitivity; // ī�޶��� �ΰ���

    [SerializeField] private float cameraRotationLimit; // ī�޶��� �ִ� ����
    private float currentCameraRotationX = 0f; // 0�� ������ �ٶ󺸴� ��, 45(���� ��ŭ ���� ����)

    [SerializeField] private Camera theCamera;
    [SerializeField] private Transform chrBody;
    Rigidbody rigid;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        applySpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (freeze)
        {
            rigid.velocity = Vector3.zero;
        }
        else
        {
            IsGround();
            TryJump();
            TryRun();
            CameraRotation();
            CharacterRotation();
        }

        if (isGround && isJump)
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

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
            // ĸ���ݶ��̴��� bounds(����)�� y���� extents(half)��
            // ��Ȯ�� ������ �ϸ� ��� ���� ��ҿ��� ������ �߻��� �� �ִ� => ���������� ���� ���� �����ش�.
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space)&& isGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        anim.SetBool("isJump", true);
        rigid.velocity = transform.up * jumpForce;

        Invoke("JumpOut", 0.5f);
    }

    private void JumpOut()
    {
        isJump = true;
    }

    private bool enableMovementOnNextTouch;

    public void JumpToPoSition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }

    private Vector3 velocityToSet;

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

    private void TryRun()
    {
        if (activeGrapple) return;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("isRun", true);
            Running();
        }
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

            Vector3 lookForward = new Vector3(theCamera.transform.forward.x, 0f, theCamera.transform.forward.z).normalized; // ī�޶��� ����
            Vector3 lookRight = new Vector3(theCamera.transform.right.x, 0f, theCamera.transform.right.z).normalized; // ī�޶��� �¿�
            Vector3 moveDir = (lookForward * moveInput.y + lookRight * moveInput.x).normalized; // �̵� ����

            chrBody.forward = moveDir;
            rigid.MovePosition(transform.position + moveDir * applySpeed * Time.deltaTime);
        }

        


        //float _moveDirX = Input.GetAxisRaw("Horizontal"); //  ���� -1, �⺻ 0 , ������ 1���� ����Ѵ�.
        //float _moveDirZ = Input.GetAxisRaw("Vertical"); // �� 1, �⺻ 0 , �Ʒ� -1 ���� ����Ѵ�.

        //Vector3 _moveHorizontal = transform.right * _moveDirX;    //(1,0,0) / (0,0,0) / (-1,0,0) ���Ͱ��� ���
        //Vector3 _moveVertical = transform.forward * _moveDirZ;    // (0,0,1) / (0,0,0) / (0,0,-1) ���Ͱ��� ���

        //Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;    // (1,0,0) (0,0,1) = (1,0,1) 2 ����ȭ�� ���� 1�ʿ� �󸶸�ŭ ������ �� �ִ��� ������ �ʱ�ȭ ���ش�.

        //Vector3 dir = (_moveHorizontal + _moveVertical).normalized;

        //transform.forward = dir;

        //rigid.MovePosition(transform.position + _velocity * Time.deltaTime); // rigid�� Ȱ���� ĳ���� �̵� ���� ��ġ���� �� frame���� �̵���Ų��.  Tiem.deltaTime�� ���� �뷫 0.016�̴�.      
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
