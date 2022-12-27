using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 *  게임 Play에 공통되는 사항을 관리하는 Manager 함수를 싱글톤으로 구성하였습니다.
 *  SetActive를 활용하여 UI의 부모 GameObject의 출력 / 숨김 기능을 구현하였습니다.
 *  FPS의 슈팅 기능에 마우스 UI Point를 숨김 / 표시 기능을 구현하였습니다.
 *  Npc 대화 Item 획득 Hooking Inventory등의 상태변수를 GameManager로 관리하였습니다.
 *  Application Quit으로 게임종료 기능을 구현하였습니다.
 */
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

    void Update()
    {
        // 인벤토리 기능이 아닐 경우에는 마우스 커서를 사라지게 합니다.
        if (!isInvetory) 
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        // 게임 Ending시에 숨길 GameObject들을 배열과 foreach문을 활용하여 숨기고 EndObj를 활성화합니다.
        if (isEnding)
        {
            foreach(GameObject e in PlayObj)
            {
                e.SetActive(false);
            }

            EndObj.SetActive(true);
            endEffect.SetActive(true);
        }

        // GamePlay의 조작 커맨드를 UI를 통하여 설명해주는 대사를 출력하게 만들었습니다.
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
