using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Des : MonoBehaviour
{
  // ������ źȯ�� �ڵ����� �����ϱ� ���ؼ� �߰��Ͽ����ϴ�.
  // ObjectPooling�� ���ؼ� źȯ�� �����Ͽ� �� ȿ�������� �޸� ������ �ϴ� ���� ��ǥ�� ������Ʈ�� �����Դϴ�.

    void Update()
    {
        Destroy(gameObject, 2f);
    }
}
