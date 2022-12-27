using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestItem : MonoBehaviour
{
    private bool isTrigger; // ������ ������ ���� ���� ����
    private bool isQitem; // ���� �ݺ� Ŭ������ ���� ������ �������ֱ� ���� ����

    public Text txtName; // ����Ʈ Info �̸� ����
    public Text txtSentence; // ����Ʈ ������ ���� �� ��� ���

    [SerializeField] private GameObject thisObj; // ����Ʈ ������ ȹ�� �� �ı��� ���� ��ũ��Ʈ

    // Update is called once per frame
    void Update()
    {
        if (isQitem && isTrigger && Input.GetKeyDown(KeyCode.F)) // FŰ�� ���� ��ȣ�ۿ� ����
        {
            isTrigger = false;
            GameManager.instance.DialougeObj.SetActive(true); // ���â ���
            QuestManager.questManager.AddQuestItem("1", 1); // ����Ʈ ������ ���� �޼���.
            txtName.text = "�����̼�"; // ����Ʈ Info �̸�

            if (QuestManager.questManager.currentQuestList.Count <= 0) // Case1. ����Ʈ �̼��ɽ� Case2. ����Ʈ ���ɽ� Case3. ���ܻ��� ����
            {
                txtSentence.text = string.Format("����Ʈ�� ���� ��������.");
            }
            else if(QuestManager.questManager.currentQuestList[0].progress == Quest.QuestProgress.ACCEPTED || QuestManager.questManager.currentQuestList[0].progress == Quest.QuestProgress.COMPLETE)
            {
                txtSentence.text = string.Format("����Ʈ �������� ȹ���Ͽ����ϴ�.");
                Destroy(thisObj,0.5f);
            }   
            else
            {
                txtSentence.text = string.Format("����Ʈ�� ���� ��������.");
            }
        }
    }


    private void OnTriggerEnter(Collider other) // Trigger Enter event �ߵ� �ÿ� Ʈ���� ����
    {
        if (other.tag == "Player")
        {
            isTrigger = true;
            isQitem = true;
        }
    }

    private void OnTriggerExit(Collider other) // ���Խ��� ������ �����ϱ� ���� Ʈ���� ����
    {
        if (other.tag == "Player")
        {
            isQitem = false;
        }
    }
}
