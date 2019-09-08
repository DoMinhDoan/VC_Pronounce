using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameSetting))]
public class GameManager : MonoBehaviour
{

    public GameObject m_mainMenu;
    public GameObject m_gamePlay;
    public GameObject m_result;

    public Image m_gameplayImage;
    public Text m_gameplayText;    

    private List<PronounceInfo> m_vowels = new List<PronounceInfo>();
    private List<PronounceInfo> m_consonants = new List<PronounceInfo>();
    private List<PronounceInfo> m_alphabet = new List<PronounceInfo>();

    private List<PronounceInfo> m_list = new List<PronounceInfo>();
    private TimeSpan m_startTime;
    private TimeSpan m_currentTime;
    private int m_timerValue;
    private bool m_auto;
    private bool m_alphabetEnable;

    private void Awake()
    {
        ReadPronounceDatabase("Vowels.json", ref m_vowels);
        ReadPronounceDatabase("Consonants.json", ref m_consonants);
        ReadPronounceDatabase("Alphabet.json", ref m_alphabet);
    }

    void ReadPronounceDatabase(string filename, ref List<PronounceInfo> list)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, filename);
        using (var fs = File.OpenText(filePath))
        {
            using (JsonTextReader reader = new JsonTextReader(fs))
            {
                var serializer = new JsonSerializer();
                list = serializer.Deserialize<List<PronounceInfo>>(reader);
            }
            fs.Close();
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        GotoMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        TimeSpan timeSpan = DateTime.Now.TimeOfDay;
        if(m_list.Count > 1)
        {
            if ((m_auto && timeSpan.Subtract(m_currentTime).TotalSeconds >= m_timerValue) || (!m_auto && Input.GetMouseButtonDown(0)))
            {
                m_currentTime = timeSpan;

                m_list.RemoveAt(m_list.Count - 1);

                ProcessNextPronounce();
            }
        }
        else
        {
            GotoMainMenu();
        }
        
    }

    public void GoToVCAuto()
    {
        m_timerValue = GetComponent<GameSetting>().GetTimerValue();
        m_auto = true;

        GotoPronounce();
        ProcessPronounceList();        
    }

    public void GoToVCClick()
    {
        m_auto = false;

        GotoPronounce();
        ProcessPronounceList();
    }

    private void GotoPronounce()
    {
        m_alphabetEnable = GetComponent<GameSetting>().AlphabetActive();
        m_mainMenu.SetActive(false);
        m_gamePlay.SetActive(true);
        m_result.SetActive(false);
    }

    private void ProcessPronounceList()
    {
        int loop = GetComponent<GameSetting>().GetLoopValue();
        CreationPronounceList(loop);
        RandomPronounceList();

        m_startTime = DateTime.Now.TimeOfDay;
        m_currentTime = m_startTime;

        ProcessNextPronounce();
    }

    private void ProcessNextPronounce()
    {
        var color = new Color(float.Parse(StringToColor(m_list[m_list.Count - 1].Color)[0]) / 255, float.Parse(StringToColor(m_list[m_list.Count - 1].Color)[1]) / 255, float.Parse(StringToColor(m_list[m_list.Count - 1].Color)[2]) / 255, 1);

        m_gameplayImage.color = color;
        m_gameplayText.text = m_list[m_list.Count - 1].Key;
    }

    private void CreationPronounceList(int loop)
    {
        for (int i = 0; i < loop; i++)
        {
            foreach (var vowel in m_vowels)
            {
                m_list.Add(vowel);
            }

            foreach (var consonant in m_consonants)
            {
                m_list.Add(consonant);
            }

            if(m_alphabetEnable)
            {
                foreach (var alphabet in m_alphabet)
                {
                    m_list.Add(alphabet);
                }
            }
        }
    }

    private void RandomPronounceList()
    {
        m_list.Shuffle();
    }

    private void GotoMainMenu()
    {
        m_mainMenu.SetActive(true);
        m_gamePlay.SetActive(false);
        m_result.SetActive(false);
    }

    List<string> StringToColor(string value)
    {
        var result = value.Split(',').ToList<string>();
        if(result.Count == 3)
        {
            return result;
        }
        throw new Exception("Wrong Color Format");
    }
}
