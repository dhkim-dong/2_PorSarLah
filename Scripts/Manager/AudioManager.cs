using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // 싱글톤 활용

    [SerializeField] AudioClip textClip; // 대사 효과음에 사용할 오디오 클립
    [SerializeField] AudioClip vilageClip; // 대사 주변음에 사용할 오디오 클립
    [SerializeField] AudioClip gunClip; // 총알 발사 효과음
    [SerializeField] AudioClip ZombieClip; // 좀비 죽음 효과음
    private AudioSource audio; // 오디오를 관리할 Inspector창의 audiosource

    void Start() // 초기화
    {
        instance = this;
        audio = GetComponent<AudioSource>();
    }

    public void E_Sound_OnShot() // 대사 효과음 출력
    {
        audio.PlayOneShot(textClip);
    }

    public void V_Sound() // 대사 주변음 출력
    {
        audio.PlayOneShot(vilageClip);
    }

    public void Gun_Sound() // 총알 발사 효과음
    {
        audio.PlayOneShot(gunClip);
    }

    public void Zombie_Die() // 좀비의 죽음 효과음
    {
        audio.PlayOneShot(ZombieClip);
    }
}
