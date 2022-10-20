using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    const int MIN_TIME_VALUE = 0;
    const int MAX_TIME_VALUE = 86400; // 24 * 60 * 60      (24�ð� 60�� 60��)

    public Image timer1;  // ȭ�� UI�� ��� �� Image
    public Text timeText; // ȭ�� UI�� ��� �� Text

    public Light sunLight; // ���̶�Űâ�� DirectLight�� �����´�.
    private Vector3 initAngle; // DirectLight�� ������ �����ϱ� ���� Vector3��

    // Start is called before the first frame update
    void Start()
    {
        initAngle = sunLight.transform.localEulerAngles;

        DateTime dt = DateTime.Now; // C# System�� ��ϵ� ����ü DateTime�� ���� ȣ���Ѵ�.
        int HH = Int32.Parse(dt.ToString("HH")); // �ð��� string data�� int�� �Ľ�
        int mm = Int32.Parse(dt.ToString("mm")); // ���� string data�� int�� �Ľ�
        int ss = Int32.Parse(dt.ToString("ss")); // ���� string data�� int�� �Ľ�

        // 24�ð� 60�� 60�ʷ� ������ 86400������ �������� ���� �ð� / ��ü �ð��� ���� �޾ƿ´�.
        // �޾ƿ� 0~1�� ���� �������� UI�� ���� �ð��帧�� ǥ���Ͽ����ϴ�.
        timer1.fillAmount = (float)(HH * 3600 + mm * 60 + ss) / MAX_TIME_VALUE; 
    }

    void Update()
    {
        ValueChanged(timer1, timeText);

        // fillAmount���� 0~1�� ���ѹݺ��ǰ� ����� ����
        if (timer1.fillAmount != 1)        
            timer1.fillAmount += 0.001f;
        else
            timer1.fillAmount = 0;
    }

    // ���ڸ� 00 ~ 59���� ��� ��Ű�� ���� ��� �޼���
    // ���ڰ� 10���� ������ �տ� "0" �� �־� UI �ڸ����� ������Ű�� ���Ͽ� �����Ͽ����ϴ�.
    public string StartZero(int num) 
    {
        return (num < 10) ? "0" + num : "" + num;
    }

    public void ValueChanged(Image image, Text text) // image�� fillamount ���� ���� image�� ���¿� text���� ���ϴ� �޼��� (�Ű� ������ UI �̹�����, UI Text���� �����´�)
    {
        // Update���� 0.001f ��ŭ �����ϴ� fillAmount ���� ���� �ð� UI�� �����ϵ��� �����Ͽ����ϴ�.
        int diff = MAX_TIME_VALUE - MIN_TIME_VALUE;
        int value = MIN_TIME_VALUE + (int)(diff * image.fillAmount);

        string h, m, s;
        int hh = value / 3600;
        int mm = (value % 3600) / 60;
        int ss = (value % 60);

        h = StartZero(hh);
        m = StartZero(mm);
        s = StartZero(ss);

        text.text = "Time" + " " + h + " : " + m + " : " + s; // UI�� ��µ� ���� ���� Time 00 : 00 : 00

        // Direction Light�� ���� ���� sunLight�� X���� ȸ������ ������ ������� �㳷 ȿ���� �����غ��ҽ��ϴ�.
        sunLight.transform.rotation =
            Quaternion.Euler(image.fillAmount * 360.0f + 270.0f, initAngle.y, initAngle.z); // �ð� �̹����� fillamount(0 ~ 1)���� ���� X���� ȸ���Ͽ� ���� ���� ����.
    }
}
