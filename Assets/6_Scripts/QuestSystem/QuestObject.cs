using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestObject : MonoBehaviour
{
    private bool inTrigger = false;

    public List<int> availableQuestIDs = new List<int>();
    public List<int> receivableQuestIDs = new List<int>();

    public GameObject questMarker; // UI �̹����� �ڽ����� ���� �ִ� GameObject�Դϴ�. SetActive�� Ȱ���Ͽ� Ȱ��, ��Ȱ��ȭ�� �����մϴ�.
    public Image theImage; // ! �Ǵ� ? �̹���

    public Sprite questAvailableSprite;  // UI Image�� ������ Sprite �̹����Դϴ�.
    public Sprite questReceivableSprite; // UI Image�� ������ Sprtie �̹����Դϴ�.

    public BoxCollider boxCollider;      // Trigger Event�� �浹�� ����Ǵ� boxCollider�Դϴ�. �ݺ����� �Է¿� ���� ������ �����ϱ� ���Ͽ� ����Ͽ����ϴ�.
    [SerializeField] private int questIndex; // Main Quest�� ������ questIndex�� ���ƾ� ����Ʈ�� �������ֱ� ���Ͽ� �߰��Ͽ����ϴ�.

    void Start()
    {
        SetQuestMaker();
        boxCollider = GetComponent<BoxCollider>();
    }

    void SetQuestMaker()
    {
        if (QuestManager.questManager.CheckCompleteQuests(this)) // ����Ʈ �Ϸ� �̹��� ����
        {
            questMarker.SetActive(true);
            theImage.sprite = questReceivableSprite;      
        }
        else if (QuestManager.questManager.CheckAvailableQuests(this)) // ����Ʈ ���� ���� �̹��� ����
        {
            questMarker.SetActive(true);
            theImage.sprite = questAvailableSprite;
        }
        else if (QuestManager.questManager.CheckAcceptedQuests(this)) // ����Ʈ ���� �̹��� ����
        {
            questMarker.SetActive(true);
            theImage.sprite = questReceivableSprite;
            theImage.color = Color.gray;
        }
        else // �� �̿ܿ� ��Ȱ��ȭ
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
            // ��� ���
            GameManager.instance.DialougeObj.SetActive(true);
            // ����Ʈ ���� ����
            QuestManager.questManager.isQuest = true;
            // QuestNum�� Index�� ���� ���� ���(�� ���� ���°� ���� ���� ��� ���� �޼��� ���)(Const7�� ���� ������ ���� ��� �Է�, ����� ��� int ErrorNum ���� ����)
            if (QuestManager.questManager.QuestNum != questIndex)
            {
                DialogueTrigger.instance.Trigger(7); // ���� �޼��� ��� 7
            }
            else // Quest ������ ���� �� ����Ʈ ����
            {
                AudioManager.instance.V_Sound();
                DialogueTrigger.instance.Trigger(questIndex);
                QuestManager.questManager.QuestNum++;
                boxCollider.enabled = false; // ���� ����Ʈ ���� + ��ȸ�� ����Ʈ�� ���Ͽ� 1���� ���� �ɵ��� ����. ��ȿ���������� �۾� ��¥�� ���߱� ���� ������ ��� ä��. ���� ���� ����
            }
            QuestManager.questManager.QuestRequest(this); // �̱������� ������ QuestManager�� QuestRequest�� ȣ��.
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
