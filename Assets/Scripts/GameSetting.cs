using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    [SerializeField]
    private Toggle m_alphabetActive;
    [SerializeField]
    private Slider m_loop;
    [SerializeField]
    private Slider m_timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
}
