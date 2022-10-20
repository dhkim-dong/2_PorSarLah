using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Des : MonoBehaviour
{
  // 생성된 탄환을 자동으로 제거하기 위해서 추가하였습니다.
  // ObjectPooling을 통해서 탄환을 관리하여 더 효율적으로 메모리 관리를 하는 것을 목표로 업데이트할 예정입니다.

    void Update()
    {
        Destroy(gameObject, 2f);
    }
}
