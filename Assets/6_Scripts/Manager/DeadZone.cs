using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadZone : MonoBehaviour
{
   // �ǵ�ġ �ʰ� ���������� ��� ��� Scene�� ������ϵ��� DeadZone�� �����Ͽ����ϴ�.
   // ����� ��ư�� ���Ե� Respawn UI�� ����� �װ��� �����ϴ� ������� ������Ʈ �����Դϴ�.

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            SceneManager.LoadScene("TeamProject");
        }
    }
}
