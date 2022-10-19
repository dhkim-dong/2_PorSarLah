using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public Text txtName;      // NPC의 이름 출력
    public Text txtSentence;  // 대사 출력

    Queue<string> sentences = new Queue<string>(); // Queue 자료구조를 활용하여 대사를 낱개로 관리. String이 char의 배열임을 이용.
   
    // DialogueTrigger 클래스를 통해 Begin메서드를 호출하여 순차적으로 Next -> End 메서드 실행


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

    public void Next() // 낱개의 단어를 출력하는 메서드
    {
        AudioManager.instance.E_Sound_OnShot();
        if(sentences.Count == 0)
        {
            End(); // 모든 단어를 출력한 경우 대사를 종료하는 메서드
            return;
        }

        txtSentence.text = string.Empty;
        StopAllCoroutines(); // 중복 메서드 호출 시 에러 방지를 위한 Stop코루틴 사용
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
