using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public Text txtName;      // UI�� ��� �� NPC�� �̸� ���. Hierarchy�� DialougeTxt�� ���� ����
    public Text txtSentence;  // UI�� ��� �� ��� ���. Hierarchy�� DialougeSentece�� ���� ����

    Queue<string> sentences = new Queue<string>(); // Queue �ڷᱸ���� Ȱ���Ͽ� ��縦 ������ ����. String�� char�� �迭���� �̿�.
   
    // DialogueTrigger Ŭ������ ���� Begin-> Next -> End �޼���� ��ȯ�Ǵ� ������ �ۼ��Ͽ����ϴ�.


    public void Begin(Dialogue info)
    {
        sentences.Clear(); // ���� �����Ͱ� �����ϴ� ���ܻ����� �����ϱ� ���� Queue�� Clear���־����ϴ�.

        txtName.text = info.name; // Inspector�� �ִ� Dialogue�� data�� NPC �̸��� �����ɴϴ�.

        foreach(var sentence in info.sentences) // Inspector�� �ִ� Dialouge�� sentence�� �����ɴϴ�. ���Լ��� ������� ��縦 ����ϱ� ���� Queue�� ����Ͽ����ϴ�.
        {
            sentences.Enqueue(sentence);
        }

        Next(); // Begin�� �������� Next �޼��带 ȣ���Ѵ�.
    }

    public void Next() // ������ �ܾ ����ϴ� �޼���
    {
        AudioManager.instance.E_Sound_OnShot(); // �ܾ� ��°� �Բ� ȿ������ ����մϴ�.
        if(sentences.Count == 0) // ��� �ܾ ����ϸ� �����մϴ�.
        {
            End(); // ��� �ܾ ����� ��� ��縦 �����ϴ� �޼���
            return;
        }

        txtSentence.text = string.Empty; // ������ ���� �ִ� ������ �����ִ� ����ڵ� �Դϴ�.
        StopAllCoroutines(); // �ߺ� �޼��� ȣ�� �� ���� ������ ���� Stop�ڷ�ƾ�� ����Ͽ����ϴ�.
        StartCoroutine(TypeSentence(sentences.Dequeue())); // ������ ��¼ӵ��� �ڷ�ƾ�� �̿��Ͽ� �ۼ��Ͽ����ϴ�.
    }

    IEnumerator TypeSentence(string sentence) // ���� ������ 0.05f ���� ����մϴ�.
    {
        foreach(var letter in sentence)
        {
            txtSentence.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void End() // ������ ��� ��ȭ�� ������ ��� UI�� ������� �մϴ�.
    {
        txtSentence.text = string.Empty;
        GameManager.instance.DialougeObj.SetActive(false);
    }

    private void Update() // ���� ��� ����� ���Ͽ� ����Ű�� �Է��� �� �Ѿ �� �ֵ��� �Ͽ����ϴ�.
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Next();
        }
    }
}
