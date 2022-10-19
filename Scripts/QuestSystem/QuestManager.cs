using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager questManager; // �̱��� 

    public List<Quest> questsList = new List<Quest>();      // ��ü ����Ʈ ���
    public List<Quest> currentQuestList = new List<Quest>(); //  ���� ����Ʈ ���

    public int QuestNum; // ������ üũ�ϱ� ���� ���� ����Ʈ ��ȣ 
    public int EndNum; //  ���� �� ������ �޼��ϴ� ����Ʈ ��ȣ
    public bool isQuest; // ���� ����Ʈ ���������� üũ�ϴ� �Ұ�
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
        if(QuestNum >= EndNum) // ���� ���� �޼� �� ���� OBJ ���
        {
            GameManager.instance.isEnding = true;
        }

    }

    public void QuestDescription()
    {
       questTxt.text = "����Ʈ: " + questsList[0].decription;
    }

    public void QuestRequest(QuestObject NPCQuestObject)
    {
        // ���� �� �ִ� ����Ʈ
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
                        // ����Ʈ ����
                        QuestUIManager.uiManager.questAvailable = true;
                        // UI�� �Ұ� ����
                        QuestUIManager.uiManager.availableQuest.Add(questsList[i]);
                        // UI�� ����Ʈ ���� ����
                    }
                }
            }
        }

        // ���� ���� ����Ʈ
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
    // ������ curQuest �߰�
    public void AcceptQuest(int questID) // questID���� ����Ʈ�� Request�޼���� ������ �ش� ����Ʈ�� currentQuest�� ���� �� ����Ʈ ���� ���� ����
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
    // ����� curQuest ����
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
    // �Ϸ� �� ����Ʈ ����(progress) ���� �� ���� �޼���
    // ���� ����Ʈ�� ���� �� CheckChainQuest Ȯ��
    public void CompleteQuest(int questID)
    {
        for (int i = 0; i < currentQuestList.Count; i++)
        {
            if (currentQuestList[i].id == questID && currentQuestList[i].progress == Quest.QuestProgress.COMPLETE)
            {
                currentQuestList[i].progress = Quest.QuestProgress.DONE;
                currentQuestList.Remove(currentQuestList[i]);

                //REWARD(���� �޼���)
            }
        }
        //check for chain quests
        CheckChainQuest(questID);
    }

    // Check Chain Quest

    void CheckChainQuest(int questID) // ���� ����Ʈ�� ��ȣ�� ���� ��ȣ�� ����
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
    // ����Ʈ �ϷḦ ���� ����Ʈ ������ ���� �޼���
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
