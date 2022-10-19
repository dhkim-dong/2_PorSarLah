using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // UI Sprite �̹����� Camera�� ������ �ٶ󺸰� ���� ��ũ��Ʈ
    Vector3 cameraDir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraDir = Camera.main.transform.forward; // ȭ�鿡 ���ߴ� ī�޶��� ���� Vector3���� �޾ƿ´�.
        cameraDir.y = 0; // ī�޶��� y Vector3���� �״�� �������� �ȵǱ� ������ "0"���� �ʱ�ȭ

        transform.rotation = Quaternion.LookRotation(cameraDir); // ��ũ��Ʈ�� �� GameObject�� ������ ī�޶� ���� ���� �����ϰ� ���ش�.
    }
}
