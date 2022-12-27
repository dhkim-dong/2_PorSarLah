using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 *  ���� Play�� ����Ǵ� ������ �����ϴ� Manager �Լ��� �̱������� �����Ͽ����ϴ�.
 *  SetActive�� Ȱ���Ͽ� UI�� �θ� GameObject�� ��� / ���� ����� �����Ͽ����ϴ�.
 *  FPS�� ���� ��ɿ� ���콺 UI Point�� ���� / ǥ�� ����� �����Ͽ����ϴ�.
 *  Npc ��ȭ Item ȹ�� Hooking Inventory���� ���º����� GameManager�� �����Ͽ����ϴ�.
 *  Application Quit���� �������� ����� �����Ͽ����ϴ�.
 */
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

    void Update()
    {
        // �κ��丮 ����� �ƴ� ��쿡�� ���콺 Ŀ���� ������� �մϴ�.
        if (!isInvetory) 
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        // ���� Ending�ÿ� ���� GameObject���� �迭�� foreach���� Ȱ���Ͽ� ����� EndObj�� Ȱ��ȭ�մϴ�.
        if (isEnding)
        {
            foreach(GameObject e in PlayObj)
            {
                e.SetActive(false);
            }

            EndObj.SetActive(true);
            endEffect.SetActive(true);
        }

        // GamePlay�� ���� Ŀ�ǵ带 UI�� ���Ͽ� �������ִ� ��縦 ����ϰ� ��������ϴ�.
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
