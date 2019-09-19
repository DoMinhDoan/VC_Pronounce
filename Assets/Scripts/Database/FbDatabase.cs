using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class FbDatabase : MonoBehaviour
{
    public struct VCInfo
    {
        public string vcKey;
        public string vcValue;

        public VCInfo(string name, string link)
        {
            vcKey = name;
            vcValue = link;
        }
    }

    List<VCInfo> m_IPAImages = new List<VCInfo>();
    List<VCInfo> m_VCAJson = new List<VCInfo>();

    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;


    public delegate void CallbackSaveIPALocalDatabase();
    public CallbackSaveIPALocalDatabase callbackSaveIPALocalDatabase = null;

    public delegate void CallbackSaveVCALocalDatabase();
    public CallbackSaveVCALocalDatabase callbackSaveVCALocalDatabase = null;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitDatase();
            }
            else
            {
                //Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
        
    }

    public void InitDatase()
    {
        InitializeFirebase();

        RegisterVCAInformation();
        RegisterIPAInformation();
    }

    protected virtual void InitializeFirebase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        app.SetEditorDatabaseUrl("https://vcpronounce.firebaseio.com/");
        if (app.Options.DatabaseUrl != null)
        {
            app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
        }

    }

    public void RegisterIPAInformation()
    {
        FirebaseDatabase.DefaultInstance
          .GetReference("IPA")
          .ValueChanged += (object sender2, ValueChangedEventArgs e2) =>
          {
              if (e2.DatabaseError != null)
              {
                  return;
              }

              if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0)
              {
                  foreach (var childSnapshot in e2.Snapshot.Children)
                  {
                      m_IPAImages.Add(new VCInfo(childSnapshot.Key, childSnapshot.Value.ToString()));
                  }

                  callbackSaveIPALocalDatabase += SaveLocalIPADatabase;
              }
          };
    }

    public void RegisterVCAInformation()
    {
        FirebaseDatabase.DefaultInstance
          .GetReference("VCA")
          .ValueChanged += (object sender2, ValueChangedEventArgs e2) =>
          {
              if (e2.DatabaseError != null)
              {
                  return;
              }

              if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0)
              {
                  foreach (var childSnapshot in e2.Snapshot.Children)
                  {
                      m_VCAJson.Add(new VCInfo(childSnapshot.Key, childSnapshot.Value.ToString()));
                  }

                  callbackSaveVCALocalDatabase += SaveLocalVCADatabase;
              }
          };
    }

    public List<VCInfo> GetIPAImages()
    {
        return m_IPAImages;
    }

    public List<VCInfo> GetVCAJson()
    {
        return m_VCAJson;
    }

    void SaveLocalIPADatabase()
    {
        var filePath = Path.Combine(Application.persistentDataPath, "IPA.dat");
        using (var fs = File.Open(filePath, FileMode.Create))
        {
            using (var writer = new BsonWriter(fs))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, m_IPAImages);
            }
        }
    }

    void SaveLocalVCADatabase()
    {        
        foreach(var vca in m_VCAJson)
        {
            StartCoroutine(DownloadVCAJson(vca.vcKey, vca.vcValue));
        }
        
    }

    IEnumerator DownloadVCAJson(string name, string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            var filePath = Path.Combine(Application.persistentDataPath, name + ".dat");
            using (var fs = File.Open(filePath, FileMode.Create))
            {
                using (var writer = new BsonWriter(fs))
                {
                    var serializer = new JsonSerializer();
                    var vcaObject = JsonConvert.DeserializeObject(www.downloadHandler.text);
                    serializer.Serialize(writer, vcaObject);
                }
            }
        }
    }
}
