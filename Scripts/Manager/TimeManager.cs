using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    const int MIN_TIME_VALUE = 0;
    const int MAX_TIME_VALUE = 86400; // 24 * 60 * 60      (24시간 60분 60초)

    public Image timer1;  // 화면 UI에 출력 될 Image
    public Text timeText; // 화면 UI에 출력 될 Text

    public Light sunLight; // 하이라키창의 DirectLight을 가져온다.
    private Vector3 initAngle; // DirectLight의 각도를 조절하기 위한 Vector3값

    // Start is called before the first frame update
    void Start()
    {
        initAngle = sunLight.transform.localEulerAngles;

        DateTime dt = DateTime.Now; // C# System에 등록된 구조체 DateTime의 값을 호출한다.
        int HH = Int32.Parse(dt.ToString("HH"));
        int mm = Int32.Parse(dt.ToString("mm"));
        int ss = Int32.Parse(dt.ToString("ss"));

        timer1.fillAmount = (float)(HH * 3600 + mm * 60 + ss) / MAX_TIME_VALUE;
    }

    // Update is called once per frame
    void Update()
    {
        ValueChanged(timer1, timeText);

        if (timer1.fillAmount != 1)          // fillAmount값을 0~1이 무한반복되게 만드는 로직
            timer1.fillAmount += 0.001f;
        else
            timer1.fillAmount = 0;
    }

    public string StartZero(int num) // 숫자를 00 ~ 59까지 출력 시키기 위한 출력 메서드
    {
        return (num < 10) ? "0" + num : "" + num;
    }

    public void ValueChanged(Image image, Text text) // image의 fillamount 값에 따라 image의 형태와 text값이 변하는 메서드 (매개 변수로 UI 이미지와, UI Text값을 가져온다)
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

        text.text = "Time" + " " + h + " : " + m + " : " + s; // UI에 출력될 숫자 형식 Time 00 : 00 : 00

        sunLight.transform.rotation =
            Quaternion.Euler(image.fillAmount * 360.0f + 270.0f, initAngle.y, initAngle.z); // 시계 이미지의 fillamount(0 ~ 1)값에 따라 X축을 회전하여 낮과 밤을 구현.
    }
}
