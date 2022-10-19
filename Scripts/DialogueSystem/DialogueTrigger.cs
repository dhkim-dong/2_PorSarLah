using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public static DialogueTrigger instance; // �̱��� Ȱ��

    public Dialogue[] info; // Dialogue Ŭ������ �����Ͱ��� ���� �Է��Ͽ� ��系�� ����

    private void Start() // static ���� �ʱ�ȭ
    {
        instance = this;
    }

    public void Trigger(int txtNum) // ��縦 �����ϱ� ���� �޼���
    {
        var system = FindObjectOfType<DialogueSystem>();
        system.Begin(info[txtNum]);
    }
}
