using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // �̱��� ����

    public GameObject[] PlayObj;    // ���� ���� �� ���� OBJ
    public GameObject EndObj;       // ���� ���� �� ����� OBJ
    public GameObject DialougeObj;  // ���� ��� ��� ����

    public bool isEnding; // ���� üũ �Ұ�
    public bool isQuestItem; // ����Ʈ ������ üũ �Ұ�
    public bool isNpc; // NPC���� ��ȭ üũ �Ұ�
    public bool isHook; // ��ŷ ���� Ȯ�� �Ұ�
    public bool isInvetory; // �κ��丮 ���� â

    [SerializeField] GameObject questObj; // ����Ʈ ������ ���Ÿ� ���� 
    [SerializeField] Text questItem; // UI�� ����� ����Ʈ ���
    [SerializeField] GameObject endEffect; // ������ ����� ����Ʈ OBJ

    private void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInvetory)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (isEnding)
        {
            foreach(GameObject e in PlayObj)
            {
                e.SetActive(false);
            }

            EndObj.SetActive(true);
            endEffect.SetActive(true);
        }

        if (isQuestItem)
        {
            questObj.SetActive(true);
            questItem.text = string.Format("����Ʈ �����۰� ��ȣ�ۿ��� �Ϸ��� FŰ�� ��������.");
        }
        else if(isNpc)
        {
            questObj.SetActive(true);
            questItem.text = string.Format("NPC�� ��ȣ�ۿ��� �Ϸ��� EŰ�� ��������");
        }
        else if (isHook)
        {
            questObj.SetActive(true);
            questItem.text = string.Format("���� �׼��� �Ϸ��� ���콺 ��Ŭ�� Ű�� ��������");
        }
        else
        {
            questObj.SetActive(false);
        }

    }

    public void GameQuit()
    {
        Application.Quit();
    }
}
