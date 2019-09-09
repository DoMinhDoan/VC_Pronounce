using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogSteps : MonoBehaviour
{
    public TextMeshProUGUI m_title;
    public Text m_description;

    public void UpdateValue(string title, string desc)
    {
        m_title.text = title;
        m_description.text = desc;
    }
}
