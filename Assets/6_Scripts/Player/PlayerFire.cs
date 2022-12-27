using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab; // 총알 프리펩 병렬화로 연결

    [SerializeField]
    private Transform cam;          // 카메라가 바라 보는 방향(forward)로 탄환을 발사하기 위해 카메라를 가져왔습니다.

    [SerializeField]
    private Transform bulletPos; // 총구의 발사 위치
    public float bulletSpeed; // 총알의 속도

    // 공격 애니메이션을 추가하기 위해서 animator를 가져왔습니다.
    // 애니메이션 관리를 하나의 Class에서 구현하는 것을 목표로 하고 있습니다.
    // 해당 애니메이션은 피드백 이 후 공격 모션을 추가 해달라는 요청에 의해 빠르게 만들어 보았습니다.
    Animator anim;           
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // 마우스 좌클릭으로 공격 구현
        {
            AudioManager.instance.Gun_Sound(); // 총알 발사 시 효과음 출력
            Use(); // 코루틴을 활용해서 애니메이션과의 싱크 및 공격 기능을 순차적으로 구성하기 위해 만들었습니다.
        }
    }

    public void Use()
    {
        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        // 공격 시의 애니메이션을 작동을 위한 신호를 송출합니다.
        anim.SetTrigger("doAttack");
        // 프리펩 생성 형식으로 탄환을 구성하였습니다. 
        // GameObject의 생성 및 파괴로 구성된 방식은 Memory 관리(최적화)에 비효율적이므로 업데이트 예정 사항으로
        // ObjectPooling _ Queue형태의 자료구조의 SetActive(활성, 비활성화) 형태로 탄환을 관리하는 버전으로 구현할 생각입니다.
        GameObject instantBullet = Instantiate(bulletPrefab, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>(); // 탄환이 갖고 있는 Rigid을 이용해서 움직임을 구현하였습니다.
        bulletRigid.velocity = cam.forward * bulletSpeed;      // 카메라의 정면으로 탄환이 발사됩니다.
        yield return null;
    }
}
    