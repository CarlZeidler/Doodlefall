using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HighScoreManager : MonoBehaviour
{
    
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private TMP_Text scoreDisplay;
    [SerializeField] private GameObject highScorePanel;

    private const string FBKEY_SCORES_PATH = "scores";
    private int currentScore = 0;
    private FirebaseDatabase db;
    private PlayerInfo playerInfo;
    
    private void Start()
    {
        db = FirebaseDatabase.DefaultInstance;
        playerInfo = FindObjectOfType<PlayerInfo>();
        InitializeUI();
    }

    private void InitializeUI()
    {
        nameDisplay.text = playerInfo.playerName;
        scoreDisplay.text = currentScore.ToString();
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
            UserID = FirebaseAuth.DefaultInstance.CurrentUser,
            name = playerInfo.playerName,
            score = currentScore,
            Date = DateTime.Now
        };
        string jsonString = JsonUtility.ToJson(highScoreEntry);
        db.RootReference.Child(FBKEY_SCORES_PATH).SetValueAsync(jsonString).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogWarning(task.Exception);
        });
    }
}

[Serializable]
public class HighScoreEntry
{
    public FirebaseUser UserID;
    public string name = "";
    public int score;
    public DateTime Date;
}