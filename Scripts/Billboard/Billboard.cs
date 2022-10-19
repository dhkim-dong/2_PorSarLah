using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // UI Sprite 이미지가 Camera의 정면을 바라보게 보는 스크립트
    Vector3 cameraDir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraDir = Camera.main.transform.forward; // 화면에 비추는 카메라의 정면 Vector3값을 받아온다.
        cameraDir.y = 0; // 카메라의 y Vector3값은 그대로 가져오면 안되기 때문에 "0"으로 초기화

        transform.rotation = Quaternion.LookRotation(cameraDir); // 스크립트가 들어간 GameObject의 각도를 카메라 정면 각도 동일하게 해준다.
    }
}
