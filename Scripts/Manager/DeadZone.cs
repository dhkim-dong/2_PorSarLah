using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadZone : MonoBehaviour
{
   // 의도치 않게 지형밖으로 벗어날 경우 Scene을 재시작하도록 DeadZone를 생성하였습니다.
   // 재시작 버튼이 포함된 Respawn UI를 만들어 그것을 실행하는 방식으로 업데이트 예정입니다.

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            SceneManager.LoadScene("TeamProject");
        }
    }
}
