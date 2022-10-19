using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤 구현

    public GameObject[] PlayObj;    // 게임 엔딩 시 숨길 OBJ
    public GameObject EndObj;       // 게임 엔딩 시 출력할 OBJ
    public GameObject DialougeObj;  // 게임 대사 출력 관리

    public bool isEnding; // 엔딩 체크 불값
    public bool isQuestItem; // 퀘스트 아이템 체크 불값
    public bool isNpc; // NPC와의 대화 체크 불값
    public bool isHook; // 후킹 상태 확인 불값
    public bool isInvetory; // 인벤토리 상태 창

    [SerializeField] GameObject questObj; // 퀘스트 아이템 제거를 위한 
    [SerializeField] Text questItem; // UI에 출력할 퀘스트 대사
    [SerializeField] GameObject endEffect; // 엔딩에 사용할 이펙트 OBJ

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
            questItem.text = string.Format("퀘스트 아이템과 상호작용을 하려면 F키를 누르세요.");
        }
        else if(isNpc)
        {
            questObj.SetActive(true);
            questItem.text = string.Format("NPC와 상호작용을 하려면 E키를 누르세요");
        }
        else if (isHook)
        {
            questObj.SetActive(true);
            questItem.text = string.Format("로프 액션을 하려면 마우스 우클릭 키를 누르세요");
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
