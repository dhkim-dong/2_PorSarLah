using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager questManager; // 싱글톤 

    public List<Quest> questsList = new List<Quest>();      // 전체 퀘스트 목록
    public List<Quest> currentQuestList = new List<Quest>(); //  현재 퀘스트 목록

    public int QuestNum; // 엔딩을 체크하기 위한 현재 퀘스트 번호 
    public int EndNum; //  도달 시 엔딩에 달성하는 퀘스트 번호
    public bool isQuest; // 현재 퀘스트 진행중인지 체크하는 불값

    public Text questTxt; // UI의 대화 창에 Quest 상태를 출력하기 위한 Text 변수입니다.

    private void Awake()
    {
        if (questManager == null)
        {
            questManager = this;
        }
        else if (questManager != this)
        {
            Destroy(gameObject);
        }
    }



    private void Update()
    {
        if(QuestNum >= EndNum) // 엔딩 조건 달성 시 엔딩 OBJ 출력
        {
            GameManager.instance.isEnding = true;
        }

    }

    // 퀘스트 내용을 화면 UI에 출력합니다.
    public void QuestDescription()
    {
       questTxt.text = "퀘스트: " + questsList[0].decription;
    }

    // 퀘스트 수락하고 퀘스트와 연계된 Bool과 List의 data를 결정해주는 메서드입니다.
    public void QuestRequest(QuestObject NPCQuestObject)
    {
        // 받을 수 있는 퀘스트
        if(NPCQuestObject.availableQuestIDs.Count > 0)
        {
            for (int i = 0; i < questsList.Count; i++)
            {
                for (int j = 0; j < NPCQuestObject.availableQuestIDs.Count; j++)
                {
                    if (questsList[i].id == NPCQuestObject.availableQuestIDs[j] && questsList[i].progress == Quest.QuestProgress.AVAILABLE)
                    {
                        Debug.Log("Quest ID: " + NPCQuestObject.availableQuestIDs[j] + " " + questsList[i].progress);
                        // Debug
                        AcceptQuest(NPCQuestObject.availableQuestIDs[j]);
                        // 퀘스트 수락
                        QuestUIManager.uiManager.questAvailable = true;
                        // UI의 불값 관리
                        QuestUIManager.uiManager.availableQuest.Add(questsList[i]);
                        // UI의 퀘스트 변수 관리
                    }
                }
            }
        }

        // 수행 중인 퀘스트
        for(int i = 0; i < currentQuestList.Count; i++)
        {
            for (int j = 0; j < NPCQuestObject.receivableQuestIDs.Count; j++)
            {
                if (currentQuestList[i].id == NPCQuestObject.receivableQuestIDs[j] 
                    && currentQuestList[i].progress == Quest.QuestProgress.COMPLETE)
                {                
                    Debug.Log("Quest ID: " + NPCQuestObject.receivableQuestIDs[j] + "is" + currentQuestList[i].progress);

                    CompleteQuest(NPCQuestObject.receivableQuestIDs[j]);
                    // quest ui manager와 연계하는 기능입니다. 
                    // 현재 제작한 게임에서는 사용하지 않았습니다.
                    // QuestUIManager.uiManager.questRunning = true;
                    // QuestUIManager.uiManager.activeQuest.Add(questsList[i]);
                }
            }
        }
    }


    // ACCEPT QUEST
    // 수락시 curQuest 추가
    public void AcceptQuest(int questID) // questID값의 퀘스트를 Request메서드로 받으면 해당 퀘스트를 currentQuest에 저장 및 퀘스트 상태 변수 관리
    {
        for (int i = 0; i < questsList.Count; i++)
        {
            if (questsList[i].id == questID && questsList[i].progress == Quest.QuestProgress.AVAILABLE)
            {
                currentQuestList.Add(questsList[i]);
                questsList[i].progress = Quest.QuestProgress.ACCEPTED;             
            }
        }

        QuestDescription();
    }

    // GIVE UP QUEST
    public void GiveUpQuest(int questID)
    {
        for (int i = 0; i < currentQuestList.Count; i++)
        {
            if(currentQuestList[i].id == questID && currentQuestList[i].progress == Quest.QuestProgress.ACCEPTED)
            {
                // 현재 QuestList으l progress(ENUM)가 수락 상태에서 가능한 상태로 되돌린다.
                // Quest 아이템 획득 Count를 0으로 초기화 해준다.
                // List에서 제거해준다.
                currentQuestList[i].progress = Quest.QuestProgress.AVAILABLE;
                currentQuestList[i].questObjectiveCount = 0;
                currentQuestList.Remove(currentQuestList[i]);
            }
        }
    }

    // COMPLETE QUEST
    // 완료 시 퀘스트 상태(progress) 변경 및 보상 메서드
    // 연계 퀘스트가 있을 시 CheckChainQuest 확인
    public void CompleteQuest(int questID)
    {
        for (int i = 0; i < currentQuestList.Count; i++)
        {
            if (currentQuestList[i].id == questID && currentQuestList[i].progress == Quest.QuestProgress.COMPLETE)
            {
                currentQuestList[i].progress = Quest.QuestProgress.DONE;
                currentQuestList.Remove(currentQuestList[i]);

                // REWARD(보상 메서드)
                // 보상 부분은 업데이트 해야합니다.
            }
        }
        // 연계된 퀘스트가 있을 시 해당 퀘스트로 넘어갑니다.
        CheckChainQuest(questID);
    }

    // Check Chain Quest

    void CheckChainQuest(int questID) // questID를 매개변수로 연계 퀘스트의 진행을 관리하였습니다.
    {
        int tempID = 0;
        for (int i = 0; i < questsList.Count; i++)
        {
            if (questsList[i].id == questID && questsList[i].nextQuest > 0)
            {
                tempID = questsList[i].nextQuest;
            }
        }

        if(tempID > 0)
        {
            for (int i = 0; i < questsList.Count; i++)
            {
                if (questsList[i].id == tempID && questsList[i].progress == Quest.QuestProgress.NOT_AVAILABLE)
                {
                    questsList[i].progress = Quest.QuestProgress.AVAILABLE;
                }
            }
        }
    }

    // ADD ITEMS
    // 퀘스트 완료를 위한 퀘스트 아이템 습득 메서드
    public void AddQuestItem(string questObjective, int itemAmount)
    {
        for (int i = 0; i < currentQuestList.Count; i++)
        {
            // Amount의 값만큼 quest에 필요한 목표 값을 증가시킵니다.
            if (currentQuestList[i].questObjective == questObjective && currentQuestList[i].progress == Quest.QuestProgress.ACCEPTED)
            {
                currentQuestList[i].questObjectiveCount += itemAmount;  
            }
            // 획득 Count가 목표치 보다 크거나 같다면 퀘스트가 완료됩니다.
            if (currentQuestList[i].questObjectiveCount >= currentQuestList[i].questobjectiveRequirement 
                && currentQuestList[i].progress == Quest.QuestProgress.ACCEPTED)
            {
                currentQuestList[i].progress = Quest.QuestProgress.COMPLETE;
            }
        }
    }

    // BOOLS : 현재 Quest의 Progress를 확인 시켜주는 메서드입니다. 메서드의 이름과 Progress과 일치하다면 해당 Bool값을 True로 아니면 false를 반환합니다.

    public bool RequestAvailableQuest(int questID)
    {
        for (int i = 0; i < questsList.Count; i++)
        {
            if (questsList[i].id == questID && questsList[i].progress == Quest.QuestProgress.AVAILABLE)
            {
                return true;
            }
        }
        return false;
    }

    public bool RequestAcceptQuest(int questID)
    {
        for (int i = 0; i < questsList.Count; i++)
        {
            if (questsList[i].id == questID && currentQuestList[i].progress == Quest.QuestProgress.ACCEPTED)
            {
                return true;
            }
        }
        return false;
    }

    public bool RequestCompleteQuest(int questID)
    {
        for (int i = 0; i < questsList.Count; i++)
        {
            if (questsList[i].id == questID && questsList[i].progress == Quest.QuestProgress.COMPLETE)
            {
                return true;
            }
        }
        return false;
    }

    // BOOLS 2

    public bool CheckAvailableQuests(QuestObject NPCQuestObject)
    {
        for (int i = 0; i < questsList.Count; i++)
        {
            for (int j = 0; j < NPCQuestObject.availableQuestIDs.Count; j++)
            {
                if (questsList[i].id == NPCQuestObject.availableQuestIDs[j] && questsList[i].progress == Quest.QuestProgress.AVAILABLE)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool CheckAcceptedQuests(QuestObject NPCQuestObject)
    {
        for (int i = 0; i < questsList.Count; i++)
        {
            for (int j = 0; j < NPCQuestObject.availableQuestIDs.Count; j++)
            {
                if (questsList[i].id == NPCQuestObject.availableQuestIDs[j] && questsList[i].progress == Quest.QuestProgress.ACCEPTED)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool CheckCompleteQuests(QuestObject NPCQuestObject)
    {
        for (int i = 0; i < questsList.Count; i++)
        {
            for (int j = 0; j < NPCQuestObject.availableQuestIDs.Count; j++)
            {
                if (questsList[i].id == NPCQuestObject.availableQuestIDs[j] && questsList[i].progress == Quest.QuestProgress.COMPLETE)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
