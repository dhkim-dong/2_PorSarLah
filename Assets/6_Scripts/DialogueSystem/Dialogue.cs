using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * ��� �ý����� ����� data�� �����ϴ� class�Դϴ�.
 * ����Ƽ Inspector���� �����͸� ���� �Է����ִ� �������� �ۼ��Ͽ����ϴ�.
 * 
 * System.Serializable�� �̿��Ͽ�
 * ����Ƽ Inspector â���� +Ű�� ���ϴ� ��ŭ �߰��� �� �ְ� ��������ϴ�.
 * 
 * ������ ��� �ý����� ���鶧��
 * Json ������ �̿��ϰų� ������ ���� data�� �޾ƿ��� ������� ����� ���� �ͽ��ϴ�.
 */


[System.Serializable]
public class Dialogue
{
    public string name;  // ��ȭ
    public List<string> sentences; // List �ڷᱸ���� ��� ����
}
