using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particle_tracking : MonoBehaviour
{
    private Transform parentObjTr; // ��ƼŬ�� �Ѿư� �θ��� Transform
    // Start is called before the first frame update
    void Start()
    {
        parentObjTr = GetComponentInParent<Transform>(); // �θ� ������ inspectorâ�� Transform�� �����´�.
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = parentObjTr.position; // �θ� GameObject�� ������ �� ��� Obj�� ���� �����δ�.
    }
}
