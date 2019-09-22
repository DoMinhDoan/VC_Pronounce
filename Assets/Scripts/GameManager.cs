using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject m_mainMenu;
    public GameObject m_gamePlay;
    public GameObject m_gameIPA;
    public GameObject m_gameTopic;
    public GameObject m_result;
    public GameObject m_gameSetting;       

    // Start is called before the first frame update
    private void Start()
    {
        GotoMainMenu();
        SliderValueChanged();
    }

    public void GoToVCClicked()
    {
        StartCoroutine(GotoVC());
    }

    public void GoToAlphabetClicked()
    {
        StartCoroutine(GotoAlphabet());
    }

    public void GoToIPAClicked()
    {
        StartCoroutine(GotoIPA());
    }

    public void GotoTopicClicked()
    {
        m_mainMenu.SetActive(false);
        m_gameTopic.SetActive(true);
        m_result.SetActive(false);
    }

    IEnumerator GotoVC()
    {
        yield return new WaitForEndOfFrame();

        ProcessPronounce();
    }

    IEnumerator GotoAlphabet()
    {
        yield return new WaitForEndOfFrame();

        ProcessAlphabet();
    }

    IEnumerator GotoIPA()
    {
        yield return new WaitForEndOfFrame();

        ProcessIPA();
    }
    

    void ProcessPronounce()
    {
        GotoPronounce();

        int loop = m_gameSetting.GetComponent<GameSetting>().GetLoopValue();
        m_gamePlay.GetComponent<GamePlay>().ProcessPronounceList(loop);
    }

    void ProcessAlphabet()
    {
        GotoPronounce();

        int loop = m_gameSetting.GetComponent<GameSetting>().GetLoopValue();
        m_gamePlay.GetComponent<GamePlay>().ProcessAlphabetList(loop);
    }

    void ProcessIPA()
    {
        m_mainMenu.SetActive(false);
        m_gameIPA.SetActive(true);
        m_result.SetActive(false);
    }

    public void GotoPronounce()
    {
        m_mainMenu.SetActive(false);
        m_gamePlay.SetActive(true);
        m_result.SetActive(false);

        m_gamePlay.GetComponent<GamePlay>().Init(m_gameSetting.GetComponent<GameSetting>());
    }

    public void GotoMainMenu()
    {
        m_mainMenu.SetActive(true);
        m_gamePlay.SetActive(false);
        m_result.SetActive(false);
        m_gameIPA.SetActive(false);
    }
    

    public void SliderValueChanged()
    {
        m_mainMenu.GetComponent<MainMenu>().UpdateMainMenu(m_gameSetting.GetComponent<GameSetting>().GetLoopValue(), m_gameSetting.GetComponent<GameSetting>().GetTimerValue());
    }
}
