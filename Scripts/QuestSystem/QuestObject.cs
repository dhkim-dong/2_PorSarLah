using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestObject : MonoBehaviour
{
    private bool inTrigger = false;

    public List<int> availableQuestIDs = new List<int>();
    public List<int> receivableQuestIDs = new List<int>();

    public GameObject questMarker; 
    public Image theImage; // ! 또는 ? 이미지

    public Sprite questAvailableSprite;
    public Sprite questReceivableSprite;

    public BoxCollider boxCollider;
    [SerializeField] private int questIndex;

    // Start is called before the first frame update
    void Start()
    {
        SetQuestMaker();
        boxCollider = GetComponent<BoxCollider>();
    }

    void SetQuestMaker()
    {
        if (QuestManager.questManager.CheckCompleteQuests(this)) // 퀘스트 완료 이미지 세팅
        {
            questMarker.SetActive(true);
            theImage.sprite = questReceivableSprite;      
        }
        else if (QuestManager.questManager.CheckAvailableQuests(this)) // 퀘스트 수령 가능 이미지 세팅
        {
            questMarker.SetActive(true);
            theImage.sprite = questAvailableSprite;
        }
        else if (QuestManager.questManager.CheckAcceptedQuests(this)) // 퀘스트 수락 이미지 세팅
        {
            questMarker.SetActive(true);
            theImage.sprite = questReceivableSprite;
            theImage.color = Color.gray;
        }
        else // 그 이외에 비활성화
        {
            questMarker.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetQuestMaker();
        if (inTrigger && Input.GetKeyDown(KeyCode.E))
        {          
            // quest ui manager
            inTrigger = false;
            // 대사 출력
            GameManager.instance.DialougeObj.SetActive(true);
            // 퀘스트 변수 관리
            QuestManager.questManager.isQuest = true;
            // QuestNum과 Index가 맞지 않을 경우(즉 진행 상태가 맞지 않을 경우 에러 메세지 출력)(Const7은 빠른 개발을 위한 상수 입력, 상수값 대신 int ErrorNum 대입 가능)
            if (QuestManager.questManager.QuestNum != questIndex)
            {
                DialogueTrigger.instance.Trigger(7); // 에러 메세지 상수 7
            }
            else // Quest 순서가 맞을 시 퀘스트 진행
            {
                AudioManager.instance.V_Sound();
                DialogueTrigger.instance.Trigger(questIndex);
                QuestManager.questManager.QuestNum++;
                boxCollider.enabled = false; // 선형 퀘스트 구조 + 일회성 퀘스트를 위하여 1번만 말을 걸도록 만듦. 비효율적이지만 작업 날짜를 맞추기 위한 차선의 방법 채택. 향후 개선 가능
            }
            QuestManager.questManager.QuestRequest(this);
            QuestUIManager.uiManager.CheckQuests(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            inTrigger = true;
        }
    }
}
