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
    public string decription;               // ����
    public string hint;                     // ����Ʈ�� ��Ʈ
    public string congratulation;           // ����Ʈ �Ϸ� ���
    public string summery;                  // ����Ʈ ���
    public int nextQuest;                   // ���� ����Ʈ�� �����ϱ� ���� int ��

    public string questObjective;           // ����Ʈ �̸�
    public int questObjectiveCount;         // ���� ����Ʈ�� �䱸 ��
    public int questobjectiveRequirement;   // ����Ʈ �ϷḦ ���� ��

    public int expReward; // ����ġ ����
    public int goldReward; // ��� ����
    public string itemReward; // ������ ����

}
