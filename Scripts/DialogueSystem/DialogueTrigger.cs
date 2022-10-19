using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public static DialogueTrigger instance; // 싱글톤 활용

    public Dialogue[] info; // Dialogue 클래스의 데이터값을 직접 입력하여 대사내용 구현

    private void Start() // static 값을 초기화
    {
        instance = this;
    }

    public void Trigger(int txtNum) // 대사를 시작하기 위한 메서드
    {
        var system = FindObjectOfType<DialogueSystem>();
        system.Begin(info[txtNum]);
    }
}
