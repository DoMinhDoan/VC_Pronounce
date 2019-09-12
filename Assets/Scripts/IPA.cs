using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static FbDatabase;

public class IPA : MonoBehaviour
{
    public Text m_IPATitle;
    public Image m_IPAImage;
    public GameObject m_gameManager;
    public Button m_nextButton;
    public Button m_previousButton;

    Dictionary<string, Sprite> m_IPASpriteCached = new Dictionary<string, Sprite>();
    private int m_currentImageIndex = 0;
    private FbDatabase m_fbDatabase;

    private List<IPAInfo> m_IPAs;

    private void Start()
    {
        var fbDatabase = m_gameManager.GetComponent<FbDatabase>();
        if (fbDatabase.GetIPAImages().Count > 0)
        {
            fbDatabase.callbackSaveLocalDatabase();
        }
    }

    private void OnEnable()
    {
        m_fbDatabase = m_gameManager.GetComponent<FbDatabase>();
        m_IPAs = LoadLocalDatabase();

        m_currentImageIndex = m_IPAs.Count - 1;

        m_nextButton.interactable = true;
        m_previousButton.interactable = true;

        ProcessCurrentFrame();

        CheckNext();
        CheckPrevious();
    }

    void ProcessCurrentFrame()
    {
        if(m_IPAs.Count > 0)
        {
            m_IPATitle.text = m_IPAs[m_currentImageIndex].ipaKey;
            AddIPASprite(m_IPAs[m_currentImageIndex].ipaKey, m_IPAs[m_currentImageIndex].ipaValue);
        }
    }

    public void NextImageClicked()
    {
        if (m_currentImageIndex < m_IPAs.Count - 1)
        {
            m_previousButton.interactable = true;
            m_currentImageIndex++;

            ProcessCurrentFrame();
        }
        CheckNext();
    }

    public void PreviousImageClicked()
    {
        if (m_currentImageIndex >= 1)
        {
            m_nextButton.interactable = true;
            m_currentImageIndex--;

            ProcessCurrentFrame();
        }
        CheckPrevious();
    }

    public void BackPressed()
    {
        m_gameManager.GetComponent<GameManager>().GotoMainMenu();
    }

    void CheckNext()
    {
        if (m_currentImageIndex == m_IPAs.Count - 1)
        {
            m_nextButton.interactable = false;
        }
    }

    void CheckPrevious()
    {
        if (m_currentImageIndex == 0)
        {
            m_previousButton.interactable = false;
        }
    }

    public void AddIPASprite(string name, string url)
    {
        if (!m_IPASpriteCached.ContainsKey(name))
        {
            string ipaPath = GetPathIPAImages();
            Sprite _sprite = LoadSprite(ipaPath + name + ".jpg");
            if (_sprite != null)
            {
                m_IPASpriteCached[name] = _sprite;
                m_IPAImage.sprite = m_IPASpriteCached[name];
            }
            else
            {
                StartCoroutine(GetSpriteFromURL(name, url));
            }
        }
        else
        {
            m_IPAImage.sprite = m_IPASpriteCached[name];
        }
    }

    IEnumerator GetSpriteFromURL(string name, string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D _texture = ((DownloadHandlerTexture)www.downloadHandler).texture as Texture2D;
            Sprite _sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f));
            if (_sprite != null)
            {
                m_IPASpriteCached[name] = _sprite;
                m_IPAImage.sprite = _sprite;
            }

            // Encode texture into JPG
            byte[] bytes = _texture.EncodeToJPG();

            string ipaPath = GetPathIPAImages();
            CreateFolder(ipaPath);

            string ipaFullPath = ipaPath + "/" + name + ".jpg";
            File.WriteAllBytes(ipaFullPath, bytes);
        }
    }

    private Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (System.IO.File.Exists(path))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        return null;
    }

    private string GetPathIPAImages()
    {
        return Application.persistentDataPath + "/IPA/";
    }

    public void CreateFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(path);
        }
    }

    List<IPAInfo> LoadLocalDatabase()
    {
        List<IPAInfo> IPAs = new List<IPAInfo>();
        var filePath = Path.Combine(Application.persistentDataPath, "IPA.dat");
        using (var fs = File.OpenRead(filePath))
        {
            using (var reader = new BsonReader(fs))
            {
                reader.ReadRootValueAsArray = true;
                var deserializer = new JsonSerializer();
                IPAs = deserializer.Deserialize<List<IPAInfo>>(reader);
            }
        }

        return IPAs;
    }
}