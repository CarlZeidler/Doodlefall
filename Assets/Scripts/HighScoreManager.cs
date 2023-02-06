using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
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
            UserID = FirebaseAuth.DefaultInstance.CurrentUser.ToString(),
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

    // public void LoadScoreBoard()
    // {
    //     DataSnapshot snap;
    //     HighScoreEntry[] scoreArray;
    //         
    //     db.RootReference.Child(FBKEY_SCORES_PATH).GetValueAsync().ContinueWithOnMainThread(task =>
    //     {
    //         if (task.Exception != null)
    //             Debug.LogWarning(task.Exception);
    //         else
    //         {
    //            snap = task.Result;
    //         }
    //     });
    //     
    //     scoreArray = new HighScoreEntry[snap.ChildrenCount];
    //     foreach(DataSnapshot.child child in snap);
    // }
}

[Serializable]
public class HighScoreEntry
{
    public string UserID;
    public string name = "";
    public int score;
    public string Date;
}