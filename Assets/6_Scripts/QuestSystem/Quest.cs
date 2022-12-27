    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest 
{
    public enum QuestProgress{ NOT_AVAILABLE, AVAILABLE, ACCEPTED, COMPLETE, DONE }

    public string title;                    // title for the Quest
    public int id;                          // ID number for the Quest
    public QuestProgress progress;          // state of the current Quest(enum)
    public string decription;               // 설명
    public string hint;                     // 퀘스트의 힌트
    public string congratulation;           // 퀘스트 완료 대사
    public string summery;                  // 퀘스트 요약
    public int nextQuest;                   // 연계 퀘스트를 구현하기 위한 int 값

    public string questObjective;           // 퀘스트 이름
    public int questObjectiveCount;         // 현재 퀘스트의 요구 수
    public int questobjectiveRequirement;   // 퀘스트 완료를 위한 수

    public int expReward; // 경험치 보상
    public int goldReward; // 골드 보상
    public string itemReward; // 아이템 보상

}
