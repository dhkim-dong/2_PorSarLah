using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    /*
     * 싱글톤 활용. 대화 시스템을 사용하는 것을 용이하게 하기 위해서 싱글톤으로 관리하였습니다.
     * Hierarchy에서 DialogueTrigger를 상속받은 빈 GameObject에서 대화 데이터를 관리합니다.
     * */

    public static DialogueTrigger instance; 
 
    public Dialogue[] info; // Dialogue의 Data를 Inspector에서 추가할 수 있습니다.(System.Serializable)로 설정하였으므로

    private void Start() // static 값을 초기화
    {
        instance = this;
    }

    public void Trigger(int txtNum) // 대사를 시작하기 위한 메서드
    {
        var system = FindObjectOfType<DialogueSystem>();
        system.Begin(info[txtNum]);
    }
}
