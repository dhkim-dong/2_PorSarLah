using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public Text txtName;      // NPC�� �̸� ���
    public Text txtSentence;  // ��� ���

    Queue<string> sentences = new Queue<string>(); // Queue �ڷᱸ���� Ȱ���Ͽ� ��縦 ������ ����. String�� char�� �迭���� �̿�.
   
    // DialogueTrigger Ŭ������ ���� Begin�޼��带 ȣ���Ͽ� ���������� Next -> End �޼��� ����


    public void Begin(Dialogue info)
    {
        sentences.Clear();

        txtName.text = info.name;

        foreach(var sentence in info.sentences)
        {
            sentences.Enqueue(sentence);
        }

        Next();
    }

    public void Next() // ������ �ܾ ����ϴ� �޼���
    {
        AudioManager.instance.E_Sound_OnShot();
        if(sentences.Count == 0)
        {
            End(); // ��� �ܾ ����� ��� ��縦 �����ϴ� �޼���
            return;
        }

        txtSentence.text = string.Empty;
        StopAllCoroutines(); // �ߺ� �޼��� ȣ�� �� ���� ������ ���� Stop�ڷ�ƾ ���
        StartCoroutine(TypeSentence(sentences.Dequeue()));
    }

    IEnumerator TypeSentence(string sentence)
    {
        foreach(var letter in sentence)
        {
            txtSentence.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void End()
    {
        txtSentence.text = string.Empty;
        GameManager.instance.DialougeObj.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Next();
        }
    }
}
