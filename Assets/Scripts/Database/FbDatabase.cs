using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Linq;


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
          .GetReference("")
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
              }
          };
    }
    
    public List<IPAInfo> GetIPAImages()
    {
        return m_IPAImages;
    }
}
