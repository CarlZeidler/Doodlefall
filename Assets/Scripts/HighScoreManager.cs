using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json.Serialization;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HighScoreManager : MonoBehaviour
{
    [SerializeField] private GameObject highScorePanel;
    [SerializeField] private TMP_Text scoreDisplay;
    
    private const string FBKEY_SCORES_PATH = "scores";
    private const string FBKEY_SCOREENTRY_PATH = "entry";
    public int currentScore = 0;
    private FirebaseDatabase db;
    private PlayerInfo playerInfo;
    private HighScoreEntry[] scoreEntries;

    private void Start()
    {
        db = FirebaseDatabase.DefaultInstance;
        playerInfo = FindObjectOfType<PlayerInfo>();
    }

    public void UpdateCurrentScore(int incomingScoreValue)
    {
        currentScore += incomingScoreValue;
        scoreDisplay.text = currentScore.ToString();
    }

    public void AddScoreToFirebase()
    {
        HighScoreEntry highScoreEntry = new HighScoreEntry
        {
            UserID = FirebaseAuth.DefaultInstance.CurrentUser.UserId,
            name = playerInfo.playerName,
            score = currentScore,
            Date = DateTime.Now.ToString()
        };

        if (highScoreEntry.name == "")
            highScoreEntry.name = "default";
        
        
        string jsonString = JsonUtility.ToJson(highScoreEntry);
        
        db.RootReference.Child(FBKEY_SCORES_PATH).Child(FBKEY_SCOREENTRY_PATH + "_" + highScoreEntry.name + "_" + DateTime.Now).SetValueAsync(jsonString).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogWarning(task.Exception);
        });
    }

    public HighScoreEntry[] LoadScoreBoard()
    {
        Debug.Log("Loading Scoreboard");
    
        db.RootReference.Child(FBKEY_SCORES_PATH).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogWarning(task.Exception);
            Debug.Log("Data fetched");
            
            DataSnapshot snap = task.Result;
            
            HighScoreEntry[] scoreArray = new HighScoreEntry[snap.ChildrenCount];

            int scoreArrayLength = scoreArray.Length;
            int scoreArrayPosition = 0;
            
            Debug.Log("Array length " + scoreArrayLength);

            foreach (var item in task.Result.Children)
            {
                Debug.Log("Loop #: " + scoreArrayPosition);
                string jsonString = (string)item.Value;
                scoreArray[scoreArrayPosition] = JsonUtility.FromJson<HighScoreEntry>(jsonString);
                Debug.Log(jsonString);
                scoreArrayPosition++;
            }
            
            Debug.Log(scoreArray);
            
            return scoreArray;
        });
        return null;
    }
}

[Serializable]
public class HighScoreEntry
{
    public string UserID;
    public string name = "";
    public int score;
    public string Date;
}