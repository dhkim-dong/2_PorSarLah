using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    /*
    퀘스트 NPC의 상태 ENUM 값 AVAILABLE,ACCEPTED, COMPLETED에 따라 구현한 2D sprite UI Image가
    플레이어의 위치에 따라 다르게 보이는 문제점이 발생하였습니다.
    이를 해결하기 위한 방법으로 UI Canvas가 속해 있는 gameObject들을 Player Cam의 정면을 바라보도록 하는 billboard 기법을 채용하였습니다. 
    */

    // 바라 보는 방향의 세부 transform을 Vector3로 받아와서 변경해주기 위하여 선언하였습니다.
    Vector3 cameraDir;

    void Update()
    {
        cameraDir = Camera.main.transform.forward; // 화면에 비추는 카메라의 정면 Vector3값을 받아온다.
        cameraDir.y = 0; // 카메라의 y Vector3값은 그대로 가져오면 안되기 때문에 "0"으로 초기화

        transform.rotation = Quaternion.LookRotation(cameraDir); // 스크립트가 들어간 GameObject의 각도를 카메라 정면 각도 동일하게 해준다.
    }
}
