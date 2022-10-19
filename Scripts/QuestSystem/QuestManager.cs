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
    public Text questTxt;

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
        // DontDestroyOnLoad(gameObject);
    }



    private void Update()
    {
        if(QuestNum >= EndNum) // 엔딩 조건 달성 시 엔딩 OBJ 출력
        {
            GameManager.instance.isEnding = true;
        }

    }

    public void QuestDescription()
    {
       questTxt.text = "퀘스트: " + questsList[0].decription;
    }

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
                    // quest ui manager
                    QuestUIManager.uiManager.questRunning = true;
                    QuestUIManager.uiManager.activeQuest.Add(questsList[i]);
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
    // 포기시 curQuest 삭제
    public void GiveUpQuest(int questID)
    {
        for (int i = 0; i < currentQuestList.Count; i++)
        {
            if(currentQuestList[i].id == questID && currentQuestList[i].progress == Quest.QuestProgress.ACCEPTED)
            {
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

                //REWARD(보상 메서드)
            }
        }
        //check for chain quests
        CheckChainQuest(questID);
    }

    // Check Chain Quest

    void CheckChainQuest(int questID) // 연계 퀘스트의 번호를 다음 번호로 변경
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
            if (currentQuestList[i].questObjective == questObjective && currentQuestList[i].progress == Quest.QuestProgress.ACCEPTED)
            {
                currentQuestList[i].questObjectiveCount += itemAmount;  
            }

            if (currentQuestList[i].questObjectiveCount >= currentQuestList[i].questobjectiveRequirement 
                && currentQuestList[i].progress == Quest.QuestProgress.ACCEPTED)
            {
                currentQuestList[i].progress = Quest.QuestProgress.COMPLETE;
            }
        }
    }

    // REMOVE ITEMS

    // BOOLS

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
