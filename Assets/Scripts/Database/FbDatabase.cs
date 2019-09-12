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

public class FbDatabase : MonoBehaviour
{
    public struct IPAInfo
    {
        public string ipaKey;
        public string ipaValue;

        public IPAInfo(string name, string link)
        {
            ipaKey = name;
            ipaValue = link;
        }
    }

    List<IPAInfo> m_IPAImages = new List<IPAInfo>();

    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;


    public delegate void CallbackSaveLocalDatabase();
    public CallbackSaveLocalDatabase callbackSaveLocalDatabase = null;

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
                      m_IPAImages.Add(new IPAInfo(childSnapshot.Key, childSnapshot.Value.ToString()));
                  }

                  callbackSaveLocalDatabase += SaveLocalDatabase;
              }
          };
    }
    
    public List<IPAInfo> GetIPAImages()
    {
        return m_IPAImages;
    }

    void SaveLocalDatabase()
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
}
