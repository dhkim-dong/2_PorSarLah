using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 대사 시스템의 대사의 data를 관리하는 class입니다.
 * 유니티 Inspector에서 데이터를 직접 입력해주는 형식으로 작성하였습니다.
 * 
 * System.Serializable을 이용하여
 * 유니티 Inspector 창에서 +키로 원하는 만큼 추가할 수 있게 만들었습니다.
 * 
 * 다음에 대사 시스템을 만들때는
 * Json 파일을 이용하거나 서버로 부터 data를 받아오는 방식으로 만들어 보고 싶습니다.
 */


[System.Serializable]
public class Dialogue
{
    public string name;  // 대화
    public List<string> sentences; // List 자료구조로 대사 관리
}
