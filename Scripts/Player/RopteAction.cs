using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopteAction : MonoBehaviour
{
    public Transform player;
    Camera cam;
    RaycastHit hit;                // Ray�� Ȱ���Ͽ� ���ø� ���� ���θ� Ȯ���Ͽ����ϴ�.
    public LayerMask GrapplingObj; // LayerMask ����� Ȱ���Ͽ� ��ŷ�� ������ Object���� �����Ͽ����ϴ�.
    LineRenderer lr;

    public Transform tip;

    private bool OnGraplling;

    Vector3 spot;
    SpringJoint sj;

    // SpringJoint�� ������ �ִ� �����Դϴ�. 
    [Range(0,20)]
    [SerializeField] float spring; // �������� ���� �����ϴ� ���Դϴ�. 0���� �����ϸ� �Ѱ踦 ���� �� ����, 0�� �ƴ� ���� �����ϸ� �Ѱ� ���� ���������� ���մϴ�.
    [Range(0, 20)]
    [SerializeField] float damper; // ������ ���� ���ҵǴ� ũ���Դϴ�. 0���� �����ϸ� ����Ʈ�� ����ؼ� ���� ��� �մϴ�. ũ�Ⱑ Ŭ���� ���ӷ��� Ŀ���ϴ�.
    [Range(0, 20)]
    [SerializeField] float massScale; // rigid�� ���� ���� �� ������ �����ϴ� ���Դϴ�. Joint�� ����� ������ ���� Object�� ������ �ٸ� Rigid�� ������ �� ����ϴ� ���Դϴ�.


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ��Ŭ���� ������ ������ ������ �����ϴ� ����Դϴ�.
        {
            RopeShot();
        }
        else if (Input.GetMouseButtonUp(0)) // ���콺 ��Ŭ�� �Է��� ���߸� ���� �׼��� ����մϴ�.
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

            // ���� ��ġ���� ��� ��ġ���� ���� �׷� �ݴϴ�.
            spot = hit.point;
            lr.positionCount = 2;
            lr.SetPosition(0, this.transform.position);
            lr.SetPosition(1, hit.point);

            // ���� Spring Joint���� ��Ŀ�� �����ϰ� ������ Sprint Joint ���� ����� �̤����ϴ�.
            sj = player.gameObject.AddComponent<SpringJoint>();
            sj.autoConfigureConnectedAnchor = false;
            sj.connectedAnchor = spot;

            // Spring Joint�� ���� ���� �����Ű�� �ڵ��Դϴ�.
            float dis = Vector3.Distance(this.transform.position, spot);

            sj.maxDistance = dis * 0.8f;
            sj.minDistance = dis * 0.25f;
            sj.spring = spring;
            sj.damper = damper;
            sj.massScale = massScale;
        }
    }

    // ��ŷ�� �����մϴ�.
    void EndShoot()
    {
        OnGraplling = false;
        lr.positionCount = 0;
        Destroy(sj);
    }

    // LineRenender�� �̿��Ͽ� ���� �׸��ϴ�.
    void DrawRope()
    {
        if (OnGraplling)
        {
            lr.SetPosition(0, tip.position);
        }
    }
}
