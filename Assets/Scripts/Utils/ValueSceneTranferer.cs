using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;

public class ValueSceneTranferer : MonoBehaviour
{
    private static ValueSceneTranferer _instance; 
    public static ValueSceneTranferer Instance {
        get {
            if (_instance != null) {
                return _instance;
            } else {
                GameObject Values_GO = new GameObject("ValueSceneTranfererAuto");
                _instance = Values_GO.AddComponent<ValueSceneTranferer>();
                _instance.InitializeDictionary();
                return _instance;
            }
        }
        private set {
            _instance = value;
        }
    }   


    [SerializeField] private string[] attributes;

    private Dictionary<String, System.Object> valuesDict;


    private void Awake()
    {
        if(_instance != null) {
            if(_instance == this) {return;}

            Destroy(this.gameObject);
            return;
        } else {
            _instance = this;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);

        InitializeDictionary();

        Invoke("AddTestValues", 1f);
    }


    public void AddOrUpdateValue(string key, System.Object value) {
        if(valuesDict.ContainsKey(key)) {
            valuesDict[key] = value;
        } else {
            valuesDict.Add(key, value);
        }
    }

    public System.Object GetValue(string key) {
        if (!valuesDict.ContainsKey(key)) {
            return null;
        } else {
            return valuesDict[key];
        }
    }


    private void InitializeDictionary()
    {
        if(valuesDict != null) {return;}

        valuesDict = new Dictionary<string, object>();
        
        if(attributes == null) {return;}
        foreach (var elem in attributes)
        {
            valuesDict.Add(elem, null);
        }
    }

    private void AddTestValues() {
        AddOrUpdateValue("isAssistantActive", false);
        AddOrUpdateValue("startInAR", true);
    }
}
