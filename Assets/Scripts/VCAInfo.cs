using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PronounceInfo
{
    public string Key;
    public string Color;
    public string Steps;
    public string Examples;
}

public class PracticeInfo
{
    public string Key;
    public string Image;
    public string Sound;
    public string Description;

    public PracticeInfo(string key, string image, string sound, string desc)
    {
        Key = key;
        Image = image;
        Sound = sound;
        Description = desc;
    }
}