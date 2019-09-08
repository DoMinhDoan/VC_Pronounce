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


    /*private Dictionary<string, Color> m_vowels = new Dictionary<string, Color>() {
        { "ɑ:", new Color(160, 89, 169) },  //1
        { "aɪ", new Color(244, 215, 78) },  //2
        { "aʊ", new Color(244, 215, 78) },  //3
        { "ɔ:", new Color(99, 182, 114) },  //4
        { "ɔɪ", new Color(99, 182, 114) },  //5
        { "oʊ", new Color(99, 182, 114) },  //6
        { "e", new Color(255, 0, 0) },      //7
        { "eɪ", new Color(255, 0, 0) },     //8
        { "æ", new Color(255, 0, 0) },      //9
        { "ɪ", new Color(51, 61, 185) },    //10
        { "i:", new Color(51, 61, 185) },   //11
        { "i", new Color(51, 61, 185) },    //11p
        { "ʊ", new Color(69, 111, 73) },    //12
        { "u:", new Color(69, 111, 73) },   //13
        { "u", new Color(69, 111, 73) },    //13p
        { "ʌ", new Color(89, 57, 184) },    //14
        { "ɜ:", new Color(89, 57, 184) },   //15
        { "ə", new Color(89, 57, 184) },    //15p
    };*/

    /*private Dictionary<string, Color> m_consonants = new Dictionary<string, Color>() {
        { "p", new Color(92, 184, 111) },   //1
        { "b", new Color(92, 184, 111) },   //2
        { "f", new Color(92, 184, 111) },   //3
        { "v", new Color(92, 184, 111) },   //4
        { "k", new Color(81, 180, 203) },   //5
        { "ɡ", new Color(81, 180, 203) },   //6
        { "θ", new Color(100, 70, 192) },   //7
        { "ð", new Color(100, 70, 192) },   //8
        { "s", new Color(255, 255, 0) },      //9
        { "z", new Color(255, 255, 0) },    //10
        { "ʃ", new Color(255, 255, 0) },    //11
        { "ʒ", new Color(255, 255, 0) },    //12
        { "t", new Color(255, 0, 0) },    //13
        { "d", new Color(255, 0, 0) },    //14
        { "ʧ", new Color(255, 0, 0) },    //15
        { "ʤ", new Color(255, 0, 0) },    //16
        { "j", new Color(255, 0, 0) },    //17
        { "m", new Color(49, 59, 172) },    //18
        { "n", new Color(49, 59, 172) },    //19
        { "ŋ", new Color(49, 59, 172) },    //20
        { "w", new Color(78, 116, 77) },    //21
        { "r", new Color(78, 116, 77) },    //22
        { "h", new Color(192, 92, 208) },    //23
        { "l", new Color(223, 133, 107) },    //24
    };*/

    /*private Dictionary<string, Color> m_alphabet = new Dictionary<string, Color>() {
        { "A", new Color(0, 0, 0) },   //1
        { "B", new Color(0, 0, 0) },   //2
        { "C", new Color(0, 0, 0) },   //3
        { "D", new Color(0, 0, 0) },   //4
        { "E", new Color(0, 0, 0) },   //5
        { "F", new Color(0, 0, 0) },   //6
        { "G", new Color(0, 0, 0) },   //7
        { "H", new Color(0, 0, 0) },   //8
        { "I", new Color(0, 0, 0) },      //9
        { "J", new Color(0, 0, 0) },    //10
        { "K", new Color(0, 0, 0) },    //11
        { "L", new Color(0, 0, 0) },    //12
        { "M", new Color(0, 0, 0) },    //13
        { "N", new Color(0, 0, 0) },    //14
        { "O", new Color(0, 0, 0) },    //15
        { "P", new Color(0, 0, 0) },    //16
        { "Q", new Color(0, 0, 0) },    //17
        { "R", new Color(0, 0, 0) },    //18
        { "S", new Color(0, 0, 0) },    //19
        { "T", new Color(0, 0, 0) },    //20
        { "U", new Color(0, 0, 0) },    //21
        { "V", new Color(0, 0, 0) },    //22
        { "W", new Color(0, 0, 0) },    //23
        { "X", new Color(0, 0, 0) },    //24
        { "Y", new Color(0, 0, 0) },    //25
        { "Z", new Color(0, 0, 0) },    //26
    };*/

    List<PronounceInfo> m_vowels = new List<PronounceInfo>();
    List<PronounceInfo> m_consonants = new List<PronounceInfo>();
    List<PronounceInfo> m_alphabet = new List<PronounceInfo>();

    /*struct GamePlay
    {
        public string m_gameplayText;
        public Color m_gameplayColor;

        public GamePlay(string text, Color color)
        {
            m_gameplayText = text;
            m_gameplayColor = color;
        }
    }*/

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
