using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static FbDatabase;

public class Topic : MonoBehaviour
{
    public GameObject m_gameManager;
    public GameObject m_topicPrefab;

    public int m_maxNumberItemH = 5;

    // Start is called before the first frame update
    void Start()
    {
        var fbDatabase = m_gameManager.GetComponent<FbDatabase>();
        if (fbDatabase.GetTopic().Count > 0)
        {
            fbDatabase.callbackSaveLocalDatabase("TOPIC");
        }
    }

    private void OnEnable()
    {
        var fbDatabase = m_gameManager.GetComponent<FbDatabase>();
        List<VCInfo> topic = fbDatabase.GetTopic();
        for (int i = 0; i < topic.Count; i++)
        {
            GameObject obj = Instantiate(m_topicPrefab, gameObject.transform);
            float x = obj.GetComponent<RectTransform>().rect.width * (i % m_maxNumberItemH) + ((int)((i % m_maxNumberItemH) + 1) * 50);
            float y = obj.GetComponent<RectTransform>().rect.height * (int)(i / m_maxNumberItemH) + ((int)((i / m_maxNumberItemH) + 1) * 50);
            obj.transform.localPosition = new Vector3(x, -y, obj.transform.localPosition.z);

            string topicName = topic[i].vcKey;
            obj.GetComponentInChildren<Text>().text = topic[i].vcKey;

            obj.GetComponent<Button>().onClick.AddListener(delegate () { StartTopic(topicName); });
        }
    }

    public void StartTopic(string topic)
    {
        var fbDatabase = m_gameManager.GetComponent<FbDatabase>();
        fbDatabase.RegisterPractiveInformation(topic);
    }
}
