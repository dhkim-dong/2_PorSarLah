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
        int HH = Int32.Parse(dt.ToString("HH"));
        int mm = Int32.Parse(dt.ToString("mm"));
        int ss = Int32.Parse(dt.ToString("ss"));

        timer1.fillAmount = (float)(HH * 3600 + mm * 60 + ss) / MAX_TIME_VALUE;
    }

    // Update is called once per frame
    void Update()
    {
        ValueChanged(timer1, timeText);

        if (timer1.fillAmount != 1)          // fillAmount���� 0~1�� ���ѹݺ��ǰ� ����� ����
            timer1.fillAmount += 0.001f;
        else
            timer1.fillAmount = 0;
    }

    public string StartZero(int num) // ���ڸ� 00 ~ 59���� ��� ��Ű�� ���� ��� �޼���
    {
        return (num < 10) ? "0" + num : "" + num;
    }

    public void ValueChanged(Image image, Text text) // image�� fillamount ���� ���� image�� ���¿� text���� ���ϴ� �޼��� (�Ű� ������ UI �̹�����, UI Text���� �����´�)
    {
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

        sunLight.transform.rotation =
            Quaternion.Euler(image.fillAmount * 360.0f + 270.0f, initAngle.y, initAngle.z); // �ð� �̹����� fillamount(0 ~ 1)���� ���� X���� ȸ���Ͽ� ���� ���� ����.
    }
}
