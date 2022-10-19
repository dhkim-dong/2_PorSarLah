using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // �̱��� Ȱ��

    [SerializeField] AudioClip textClip; // ��� ȿ������ ����� ����� Ŭ��
    [SerializeField] AudioClip vilageClip; // ��� �ֺ����� ����� ����� Ŭ��
    [SerializeField] AudioClip gunClip; // �Ѿ� �߻� ȿ����
    [SerializeField] AudioClip ZombieClip; // ���� ���� ȿ����
    private AudioSource audio; // ������� ������ Inspectorâ�� audiosource

    // Start is called before the first frame update
    void Start() // �ʱ�ȭ
    {
        instance = this;
        audio = GetComponent<AudioSource>();
    }

    public void E_Sound_OnShot() // ��� ȿ���� ���
    {
        audio.PlayOneShot(textClip);
    }

    public void V_Sound() // ��� �ֺ��� ���
    {
        audio.PlayOneShot(vilageClip);
    }

    public void Gun_Sound()
    {
        audio.PlayOneShot(gunClip);
    }

    public void Zombie_Die()
    {
        audio.PlayOneShot(ZombieClip);
    }
}
