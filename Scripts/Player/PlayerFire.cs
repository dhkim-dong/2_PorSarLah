using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab; // 총알 프리펩 병렬화로 연결

    [SerializeField]
    private Transform cam;

    [SerializeField]
    private Transform bulletPos; // 총구의 발사 위치
    public float bulletSpeed; // 총알의 속도

    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject bullet = Instantiate(bulletPrefab);
            AudioManager.instance.Gun_Sound();
            bullet.transform.position = bulletPos.transform.position;
            Use();
        }
    }

    public void Use()
    {
        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        anim.SetTrigger("doAttack");
        yield return new WaitForSeconds(0.5f);
        GameObject instantBullet = Instantiate(bulletPrefab, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = cam.forward * bulletSpeed;
        yield return null;

    }
}
