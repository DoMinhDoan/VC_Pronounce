using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text m_loopValue;
    public Text m_timerValue;

    private string m_loopText;
    private string m_timerText;

    private void Start()
    {
        m_loopText = m_loopValue.text;
        m_timerText = m_timerValue.text;
    }
    public void UpdateMainMenu(int loop, int timer)
    {
        m_loopValue.text = m_loopText + " : " + loop.ToString();
        m_timerValue.text = m_timerText + " : " + timer.ToString() + "s";
    }
}
