using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab; // �Ѿ� ������ ����ȭ�� ����

    [SerializeField]
    private Transform cam;          // ī�޶� �ٶ� ���� ����(forward)�� źȯ�� �߻��ϱ� ���� ī�޶� �����Խ��ϴ�.

    [SerializeField]
    private Transform bulletPos; // �ѱ��� �߻� ��ġ
    public float bulletSpeed; // �Ѿ��� �ӵ�

    // ���� �ִϸ��̼��� �߰��ϱ� ���ؼ� animator�� �����Խ��ϴ�.
    // �ִϸ��̼� ������ �ϳ��� Class���� �����ϴ� ���� ��ǥ�� �ϰ� �ֽ��ϴ�.
    // �ش� �ִϸ��̼��� �ǵ�� �� �� ���� ����� �߰� �ش޶�� ��û�� ���� ������ ����� ���ҽ��ϴ�.
    Animator anim;           
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // ���콺 ��Ŭ������ ���� ����
        {
            AudioManager.instance.Gun_Sound(); // �Ѿ� �߻� �� ȿ���� ���
            Use(); // �ڷ�ƾ�� Ȱ���ؼ� �ִϸ��̼ǰ��� ��ũ �� ���� ����� ���������� �����ϱ� ���� ��������ϴ�.
        }
    }

    public void Use()
    {
        StartCoroutine(Shot());
    }

    IEnumerator Shot()
    {
        // ���� ���� �ִϸ��̼��� �۵��� ���� ��ȣ�� �����մϴ�.
        anim.SetTrigger("doAttack");
        // ������ ���� �������� źȯ�� �����Ͽ����ϴ�. 
        // GameObject�� ���� �� �ı��� ������ ����� Memory ����(����ȭ)�� ��ȿ�����̹Ƿ� ������Ʈ ���� ��������
        // ObjectPooling _ Queue������ �ڷᱸ���� SetActive(Ȱ��, ��Ȱ��ȭ) ���·� źȯ�� �����ϴ� �������� ������ �����Դϴ�.
        GameObject instantBullet = Instantiate(bulletPrefab, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>(); // źȯ�� ���� �ִ� Rigid�� �̿��ؼ� �������� �����Ͽ����ϴ�.
        bulletRigid.velocity = cam.forward * bulletSpeed;      // ī�޶��� �������� źȯ�� �߻�˴ϴ�.
        yield return null;
    }
}
    