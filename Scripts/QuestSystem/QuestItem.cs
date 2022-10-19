using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestItem : MonoBehaviour
{
    private bool isTrigger; // 아이템 습득을 위한 조건 변수
    private bool isQitem; // 빠른 반복 클릭으로 인한 버그을 제어해주기 위한 변수

    public Text txtName; // 퀘스트 Info 이름 정보
    public Text txtSentence; // 퀘스트 아이템 습득 시 대사 출력

    [SerializeField] private GameObject thisObj; // 퀘스트 아이템 획득 후 파괴를 위한 스크립트

    // Update is called once per frame
    void Update()
    {
        if (isQitem && isTrigger && Input.GetKeyDown(KeyCode.F)) // F키를 눌러 상호작용 가능
        {
            isTrigger = false;
            GameManager.instance.DialougeObj.SetActive(true); // 대사창 출력
            QuestManager.questManager.AddQuestItem("1", 1); // 퀘스트 아이템 습득 메서드.
            txtName.text = "나레이션"; // 퀘스트 Info 이름

            if (QuestManager.questManager.currentQuestList.Count <= 0) // Case1. 퀘스트 미수령시 Case2. 퀘스트 수령시 Case3. 예외사항 관리
            {
                txtSentence.text = string.Format("퀘스트를 먼저 받으세요.");
            }
            else if(QuestManager.questManager.currentQuestList[0].progress == Quest.QuestProgress.ACCEPTED || QuestManager.questManager.currentQuestList[0].progress == Quest.QuestProgress.COMPLETE)
            {
                txtSentence.text = string.Format("퀘스트 아이템을 획득하였습니다.");
                Destroy(thisObj,0.5f);
            }   
            else
            {
                txtSentence.text = string.Format("퀘스트를 먼저 받으세요.");
            }
        }
    }


    private void OnTriggerEnter(Collider other) // Trigger Enter event 발동 시에 트리거 관리
    {
        if (other.tag == "Player")
        {
            isTrigger = true;
            isQitem = true;
        }
    }

    private void OnTriggerExit(Collider other) // 진입시의 변수를 제어하기 위한 트리거 관리
    {
        if (other.tag == "Player")
        {
            isQitem = false;
        }
    }
}
