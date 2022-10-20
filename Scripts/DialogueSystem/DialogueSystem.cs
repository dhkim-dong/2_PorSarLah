using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public Text txtName;      // UI에 출력 될 NPC의 이름 출력. Hierarchy의 DialougeTxt에 직렬 연결
    public Text txtSentence;  // UI에 출력 될 대사 출력. Hierarchy의 DialougeSentece에 직렬 연결

    Queue<string> sentences = new Queue<string>(); // Queue 자료구조를 활용하여 대사를 낱개로 관리. String이 char의 배열임을 이용.
   
    // DialogueTrigger 클래스를 통해 Begin-> Next -> End 메서드로 순환되는 구조로 작성하였습니다.


    public void Begin(Dialogue info)
    {
        sentences.Clear(); // 더미 데이터가 존재하는 예외사항을 방지하기 위해 Queue를 Clear해주었습니다.

        txtName.text = info.name; // Inspector에 있는 Dialogue의 data의 NPC 이름을 가져옵니다.

        foreach(var sentence in info.sentences) // Inspector에 있는 Dialouge의 sentence를 가져옵니다. 선입선출 방식으로 대사를 출력하기 위해 Queue를 사용하였습니다.
        {
            sentences.Enqueue(sentence);
        }

        Next(); // Begin의 마지막에 Next 메서드를 호출한다.
    }

    public void Next() // 낱개의 단어를 출력하는 메서드
    {
        AudioManager.instance.E_Sound_OnShot(); // 단어 출력과 함께 효과음을 출력합니다.
        if(sentences.Count == 0) // 모든 단어를 출력하면 종료합니다.
        {
            End(); // 모든 단어를 출력한 경우 대사를 종료하는 메서드
            return;
        }

        txtSentence.text = string.Empty; // 이전에 남아 있는 문장을 없애주는 방어코드 입니다.
        StopAllCoroutines(); // 중복 메서드 호출 시 에러 방지를 위한 Stop코루틴을 사용하였습니다.
        StartCoroutine(TypeSentence(sentences.Dequeue())); // 문자의 출력속도를 코루틴을 이용하여 작성하였습니다.
    }

    IEnumerator TypeSentence(string sentence) // 받은 문장을 0.05f 마다 출력합니다.
    {
        foreach(var letter in sentence)
        {
            txtSentence.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void End() // 정해진 모든 대화가 끝나면 대사 UI를 사라지게 합니다.
    {
        txtSentence.text = string.Empty;
        GameManager.instance.DialougeObj.SetActive(false);
    }

    private void Update() // 빠른 대사 출력을 위하여 엔터키를 입력할 시 넘어갈 수 있도록 하였습니다.
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Next();
        }
    }
}
