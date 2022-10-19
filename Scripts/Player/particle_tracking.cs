using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particle_tracking : MonoBehaviour
{
    private Transform parentObjTr; // 파티클이 쫓아갈 부모의 Transform
    // Start is called before the first frame update
    void Start()
    {
        parentObjTr = GetComponentInParent<Transform>(); // 부모 영역의 inspector창의 Transform을 가져온다.
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = parentObjTr.position; // 부모 GameObject가 움직일 때 대상 Obj도 따라 움직인다.
    }
}
