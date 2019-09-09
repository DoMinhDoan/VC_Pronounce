using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    [SerializeField]
    private Toggle m_alphabetActive;
    [SerializeField]
    private Toggle m_exampleActive;
    [SerializeField]
    private Slider m_loop;
    [SerializeField]
    private Slider m_timer;
    [SerializeField]
    private Toggle m_auto;
    
    public int GetLoopValue()
    {
        return (int)m_loop.value;
    }

    public int GetTimerValue()
    {
        return (int)m_timer.value;
    }

    public bool AlphabetActive()
    {
        return m_alphabetActive.isOn;
    }

    public bool ExampleActive()
    {
        return m_exampleActive.isOn;
    }

    public bool AutoActive()
    {
        return m_auto.isOn;
    }
}
