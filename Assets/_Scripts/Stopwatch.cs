using UnityEngine;
using UnityEngine.UI;

public class Stopwatch : MonoBehaviour
{
    private int m_Time = 0;
    private int s_Time = 0;
    private int ms_Time = 0;

    private Text tmp;
    private EventHandler eH;

    private void Start()
    {
        tmp = GetComponent<Text>();
        eH = FindObjectOfType<EventHandler>();
    }

    private void FixedUpdate()
    {
        m_Time = ((int)eH.timeElapsed) / 60;
        s_Time = ((int)eH.timeElapsed) % 60;
        ms_Time = (int)(eH.timeElapsed * 100 - ((int)eH.timeElapsed) * 100);

        tmp.text = FormatString();
    }

    private string FormatString()
    {
        string formated = "";

        if (m_Time < 10)
            formated = "0" + m_Time + ":";
        else
            formated = m_Time + ":";

        if (s_Time < 10)
            formated += "0" + s_Time + ".";
        else
            formated += s_Time + ".";

        if (ms_Time < 10)
            formated += "0" + ms_Time;
        else
            formated += ms_Time;

        return formated;
    }


}
