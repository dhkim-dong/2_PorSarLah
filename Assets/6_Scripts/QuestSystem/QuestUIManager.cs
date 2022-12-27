using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    // 소스파일로 사용한 Script에는 UI를 활용하여 퀘스트를 accept, giveup, complete 할 수 있는 구조였지만, 개발할 때 UI를 이용한 퀘스트 시스템을 사용하지 않고 변경하기 위해서 해당 Script는 사용하지 않음
    // 기존 QuestManager에서 사용한 메서드와의 호환을 위해 삭제는 하지 않고, UI기능을 추가할 수 있기 때문에 보존을 선택.

    public static QuestUIManager uiManager;

    // Bools
    public bool questAvailable = false;
    public bool questRunning = false;
    private bool questpanelActive = false;
    private bool questingPanel = false;

    // Panels

    public GameObject questPanel;
    public GameObject questLogPanel;

    // QuestObject
    private QuestObject currentQuestObject;

    // QuestList
    public List<Quest> availableQuest = new List<Quest>();
    public List<Quest> activeQuest = new List<Quest>();

    // Buttons 
    public GameObject qButton;
    public GameObject qLogButton;
    private List<GameObject> qButtons = new List<GameObject>();

    private GameObject acceptButton;
    private GameObject giveUpButton;
    private GameObject completeButton;

    // Spacer
    public Transform qButtonSpacer1;
    public Transform qButtonSpacer2;
    public Transform qLogButtonSpacer;

    // Quest Info
    public Text questTitle;
    public Text questDescription;
    public Text questSummary;

    // Quest Log Info
    public Text questLogTitle;
    public Text questLogDescription;
    public Text questLogSummary;

    private void Awake()
    {
        if(uiManager == null)
        {
            uiManager = this;
        }
        else if(uiManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        HideQuestPanel();
    }

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            questpanelActive = !questpanelActive;
        }
    }

    public void CheckQuests(QuestObject questObject)
    {
        currentQuestObject = questObject;
        QuestManager.questManager.QuestRequest(questObject);
        if((questRunning || questAvailable) && !questpanelActive)
        {
            //ShowQuestPanel();
        }
        else
        {
            Debug.Log("No Quests Available");
        }
    }

    public void ShowQuestPanel()
    {
        questpanelActive = true;
        questPanel.SetActive(questpanelActive);
    }

    public void HideQuestPanel()
    {
        questpanelActive = false;
        questAvailable = false;
        questRunning = false;

        //clear Text
        questTitle.text = "";
        questDescription.text = "";
        questSummary.text = "";

        //clear Lists

        availableQuest.Clear();
        activeQuest.Clear();

        //clear button list
        for (int i = 0; i < qButtons.Count; i++)
        {
            Destroy(qButtons[i]);
        }
        qButtons.Clear();

        questPanel.SetActive(questpanelActive);
    }

    void FillQuestButtons()
    {
        foreach(Quest availableQuest in availableQuest)
        {
            GameObject questButton = Instantiate(qButton);

            questButton.transform.SetParent(qButtonSpacer1, false);
            qButtons.Add(questButton);
        }

        foreach (Quest activeQuest in availableQuest)
        {
            GameObject questButton = Instantiate(qButton);

            questButton.transform.SetParent(qButtonSpacer1, false);
            qButtons.Add(questButton);
        }
    }

    //SHow Button

    public void ShowSelectedQuest(int questID)
    {
        for (int i = 0; i < availableQuest.Count; i++)
        {
            if (availableQuest[i].id == questID)
            {
                questTitle.text = availableQuest[i].title;
                if (availableQuest[i].progress == Quest.QuestProgress.AVAILABLE)
                {
                    questDescription.text = availableQuest[i].decription;
                    questSummary.text = availableQuest[i].questObjective + " : " + availableQuest[i].questObjectiveCount + " / " + availableQuest[i].questobjectiveRequirement;
                }
            }
        }

        for (int i = 0; i < activeQuest.Count; i++)
        {
            if (activeQuest[i].id == questID)
            {
                questTitle.text = activeQuest[i].title;
                if (activeQuest[i].progress == Quest.QuestProgress.ACCEPTED)
                {
                    questDescription.text = activeQuest[i].hint;
                    questSummary.text = activeQuest[i].questObjective + " : " + activeQuest[i].questObjectiveCount + " / " + activeQuest[i].questobjectiveRequirement;
                }
                else if(activeQuest[i].progress == Quest.QuestProgress.COMPLETE)
                {
                    questDescription.text = activeQuest[i].congratulation;
                    questSummary.text = activeQuest[i].questObjective + " : " + activeQuest[i].questObjectiveCount + " / " + activeQuest[i].questobjectiveRequirement;
                }
            }
        }
    }
}
