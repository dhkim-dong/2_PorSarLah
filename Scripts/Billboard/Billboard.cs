using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    /*
    ����Ʈ NPC�� ���� ENUM �� AVAILABLE,ACCEPTED, COMPLETED�� ���� ������ 2D sprite UI Image��
    �÷��̾��� ��ġ�� ���� �ٸ��� ���̴� �������� �߻��Ͽ����ϴ�.
    �̸� �ذ��ϱ� ���� ������� UI Canvas�� ���� �ִ� gameObject���� Player Cam�� ������ �ٶ󺸵��� �ϴ� billboard ����� ä���Ͽ����ϴ�. 
    */

    // �ٶ� ���� ������ ���� transform�� Vector3�� �޾ƿͼ� �������ֱ� ���Ͽ� �����Ͽ����ϴ�.
    Vector3 cameraDir;

    void Update()
    {
        cameraDir = Camera.main.transform.forward; // ȭ�鿡 ���ߴ� ī�޶��� ���� Vector3���� �޾ƿ´�.
        cameraDir.y = 0; // ī�޶��� y Vector3���� �״�� �������� �ȵǱ� ������ "0"���� �ʱ�ȭ

        transform.rotation = Quaternion.LookRotation(cameraDir); // ��ũ��Ʈ�� �� GameObject�� ������ ī�޶� ���� ���� �����ϰ� ���ش�.
    }
}
