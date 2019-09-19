using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{
    public Image m_gameplayImage;
    public GameObject m_gameplayVC;
    public GameObject m_gameplayExamle;
    public GameObject m_dialogSteps;
    public GameObject m_gameManager;

    private bool m_showingDescription = false;

    private List<PronounceInfo> m_vowels = new List<PronounceInfo>();
    private List<PronounceInfo> m_consonants = new List<PronounceInfo>();
    private List<PronounceInfo> m_alphabet = new List<PronounceInfo>();
    private List<PronounceInfo> m_list = new List<PronounceInfo>();

    private TimeSpan m_startTime;
    private TimeSpan m_currentTime;

    // caching
    private GameSetting m_gameSetting;

    float m_pronounceOffsetY = 0;


    private void Awake()
    {
        ReadPronounceDatabase("Vowels", ref m_vowels);
        ReadPronounceDatabase("Consonants", ref m_consonants);
        ReadPronounceDatabase("Alphabet", ref m_alphabet);

        m_pronounceOffsetY = m_gameplayVC.transform.localPosition.y;
    }

    private void Start()
    {
        var fbDatabase = m_gameManager.GetComponent<FbDatabase>();
        if (fbDatabase.GetVCAJson().Count > 0)
        {
            fbDatabase.callbackSaveVCALocalDatabase();
        }
    }

    public void Init(GameSetting gameSetting)
    {
        m_gameSetting = gameSetting;

        if (m_gameSetting.ExampleActive())
        {
            m_gameplayVC.transform.localPosition = new Vector3(m_gameplayVC.transform.localPosition.x, m_pronounceOffsetY, m_gameplayVC.transform.localPosition.z);
            m_gameplayExamle.SetActive(true);
        }
        else
        {
            m_gameplayVC.transform.localPosition = new Vector3(m_gameplayVC.transform.localPosition.x, 0, m_gameplayVC.transform.localPosition.z);
            m_gameplayExamle.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        TimeSpan timeSpan = DateTime.Now.TimeOfDay;
        if (m_list.Count > 1)
        {
            if ((m_gameSetting.AutoActive() && timeSpan.Subtract(m_currentTime).TotalSeconds >= m_gameSetting.GetTimerValue()) || (!m_gameSetting.AutoActive() && Input.GetMouseButtonUp(0) && !m_showingDescription))
            {
                m_currentTime = timeSpan;

                m_list.RemoveAt(m_list.Count - 1);

                ProcessNextPronounce();
            }
        }
        else
        {
            m_gameManager.GetComponent<GameManager>().GotoMainMenu();
        }

    }

    void ReadPronounceDatabase(string filename, ref List<PronounceInfo> list)
    {

        var filePath = Path.Combine(Application.persistentDataPath, filename + ".dat");
        if(File.Exists(filePath))
        {
            using (var fs = File.OpenRead(filePath))
            {
                using (var reader = new BsonReader(fs))
                {
                    reader.ReadRootValueAsArray = true;
                    var deserializer = new JsonSerializer();
                    list = deserializer.Deserialize<List<PronounceInfo>>(reader);
                }
            }
        }
        else
        {
            TextAsset database = Resources.Load<TextAsset>(filename);
            list = JsonConvert.DeserializeObject<List<PronounceInfo>>(database.text);
        }
    }

    public void UpdateGamePlay(List<PronounceInfo> pronounces)
    {
        var color = new Color(float.Parse(StringToColor(pronounces[pronounces.Count - 1].Color)[0]) / 255, float.Parse(StringToColor(pronounces[pronounces.Count - 1].Color)[1]) / 255, float.Parse(StringToColor(pronounces[pronounces.Count - 1].Color)[2]) / 255, 1);

        m_gameplayImage.color = color;
        m_gameplayVC.GetComponentInChildren<TextMeshProUGUI>().text = pronounces[pronounces.Count - 1].Key;

        if (m_gameSetting.ExampleActive())
        {
            m_gameplayExamle.GetComponentInChildren<TextMeshProUGUI>().text = pronounces[pronounces.Count - 1].Examples;
        }
    }

    public void ProcessPronounceList(int loop)
    {
        CreationPronounceList(loop);
        RandomPronounceList();

        m_startTime = DateTime.Now.TimeOfDay;
        m_currentTime = m_startTime;

        ProcessNextPronounce();
    }

    public void ProcessAlphabetList(int loop)
    {
        CreationAlphabetList(loop);
        RandomPronounceList();

        m_startTime = DateTime.Now.TimeOfDay;
        m_currentTime = m_startTime;

        ProcessNextPronounce();
    }

    private void ProcessNextPronounce()
    {
        UpdateGamePlay(m_list);
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

            if (m_gameSetting.AlphabetActive())
            {
                foreach (var alphabet in m_alphabet)
                {
                    m_list.Add(alphabet);
                }
            }
        }
    }

    private void CreationAlphabetList(int loop)
    {
        for (int i = 0; i < loop; i++)
        {
            foreach (var alphabet in m_alphabet)
            {
                m_list.Add(alphabet);
            }
        }
    }

    private void RandomPronounceList()
    {
        m_list.Shuffle();
    }

    public void PronounceClicked()
    {
        if ( !m_gameSetting.AutoActive() && m_list[m_list.Count - 1].Steps != "")
        {
            m_showingDescription = true;
            m_dialogSteps.SetActive(m_showingDescription);

            m_dialogSteps.GetComponent<DialogSteps>().UpdateValue("/" + m_list[m_list.Count - 1].Key + "/", m_list[m_list.Count - 1].Steps);
        }
    }

    public void ClosePronounceClicked()
    {
        m_dialogSteps.SetActive(false);
        StartCoroutine(ClosePronounce());
    }

    IEnumerator ClosePronounce()
    {
        yield return new WaitForEndOfFrame();
        m_showingDescription = false;
    }

    List<string> StringToColor(string value)
    {
        var result = value.Split(',').ToList<string>();
        if (result.Count == 3)
        {
            return result;
        }
        throw new Exception("Wrong Color Format");
    }
}
