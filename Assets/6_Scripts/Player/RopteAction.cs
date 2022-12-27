using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopteAction : MonoBehaviour
{
    public Transform player;
    Camera cam;
    RaycastHit hit;                // Ray를 활용하여 로플링 가능 여부를 확인하였습니다.
    public LayerMask GrapplingObj; // LayerMask 기능을 활용하여 후킹이 가능한 Object들을 관리하였습니다.
    LineRenderer lr;

    public Transform tip;

    private bool OnGraplling;

    Vector3 spot;
    SpringJoint sj;

    // SpringJoint가 가지고 있는 변수입니다. 
    [Range(0,20)]
    [SerializeField] float spring; // 스프링의 힘을 결정하는 값입니다. 0으로 설정하면 한계를 넘을 수 없고, 0이 아닌 값을 설정하면 한계 값이 유동적으로 변합니다.
    [Range(0, 20)]
    [SerializeField] float damper; // 스프링 힘이 감소되는 크기입니다. 0으로 설정하면 조인트가 계속해서 진동 운동을 합니다. 크기가 클수록 감속량이 커집니다.
    [Range(0, 20)]
    [SerializeField] float massScale; // rigid의 반전 질량 및 관성에 적용하는 값입니다. Joint가 연결된 질량과 현재 Object의 질량이 다른 Rigid를 연결할 때 사용하는 값입니다.


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 좌클릭을 누르고 있으면 로프를 유지하는 기능입니다.
        {
            RopeShot();
        }
        else if (Input.GetMouseButtonUp(0)) // 마우스 좌클릭 입력이 멈추면 로프 액션을 취소합니다.
        {
            EndShoot();
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void RopeShot() 
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, GrapplingObj))
        {
            OnGraplling = true;

            // 현재 위치에서 대상 위치까지 선을 그려 줍니다.
            spot = hit.point;
            lr.positionCount = 2;
            lr.SetPosition(0, this.transform.position);
            lr.SetPosition(1, hit.point);

            // 기존 Spring Joint값의 앵커를 해제하고 지정한 Sprint Joint 값을 사용해 ㅜㅈㅂ니다.
            sj = player.gameObject.AddComponent<SpringJoint>();
            sj.autoConfigureConnectedAnchor = false;
            sj.connectedAnchor = spot;

            // Spring Joint의 세부 값을 변경시키는 코드입니다.
            float dis = Vector3.Distance(this.transform.position, spot);

            sj.maxDistance = dis * 0.8f;
            sj.minDistance = dis * 0.25f;
            sj.spring = spring;
            sj.damper = damper;
            sj.massScale = massScale;
        }
    }

    // 후킹을 종료합니다.
    void EndShoot()
    {
        OnGraplling = false;
        lr.positionCount = 0;
        Destroy(sj);
    }

    // LineRenender를 이용하여 선을 그립니다.
    void DrawRope()
    {
        if (OnGraplling)
        {
            lr.SetPosition(0, tip.position);
        }
    }
}
