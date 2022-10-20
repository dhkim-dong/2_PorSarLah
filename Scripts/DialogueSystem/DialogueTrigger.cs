using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    /*
     * �̱��� Ȱ��. ��ȭ �ý����� ����ϴ� ���� �����ϰ� �ϱ� ���ؼ� �̱������� �����Ͽ����ϴ�.
     * Hierarchy���� DialogueTrigger�� ��ӹ��� �� GameObject���� ��ȭ �����͸� �����մϴ�.
     * */

    public static DialogueTrigger instance; 
 
    public Dialogue[] info; // Dialogue�� Data�� Inspector���� �߰��� �� �ֽ��ϴ�.(System.Serializable)�� �����Ͽ����Ƿ�

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
