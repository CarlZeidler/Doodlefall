using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HighScoreManager : MonoBehaviour
{
    [SerializeField] private GameObject highScorePanel;
    [SerializeField] private TMP_Text scoreDisplay;
    [SerializeField] private GameObject highScoreEntriesPanel;
    
    private const string FBKEY_SCORES_PATH = "scores";
    private const string FBKEY_SCOREENTRY_PATH = "entry";
    public int currentScore;
    private FirebaseDatabase db;
    private PlayerInfo playerInfo;
    private HighScoreEntry[] scoreEntries;
    private bool scoreBoardLoaded;
    private List<GameObject> highScoreListObjects = new List<GameObject>();

    public delegate void onScoreFetchedDelegate(List<HighScoreEntry> scoreList);
    
    private void Start()
    {
        db = FirebaseDatabase.DefaultInstance;
        playerInfo = FindObjectOfType<PlayerInfo>();
    }

    public void LoadScoreBoard()
    {
        FetchScoreBoard(ShowScoreBoard);
    }

    public void ShowScoreBoard(List<HighScoreEntry> scoreList)
    {
        scoreList.Sort((x, y) => y.score.CompareTo(x.score));
        foreach (var item in scoreList.Take(10))
        {
            GameObject thisScorePanel = Instantiate(highScorePanel, highScoreEntriesPanel.transform);
            HighScorePanelScript panelScript = thisScorePanel.GetComponent<HighScorePanelScript>();
            panelScript.updateFields(item.name, item.score, item.Date);
            highScoreListObjects.Add(thisScorePanel);
        }
    }

    public void ClearScoreBoard()
    {
        foreach (GameObject item in highScoreListObjects)
        {
            Destroy(item);
        }
    }
    
    public void FetchScoreBoard(onScoreFetchedDelegate onScoreFetchedDelegate)
    {
        db.RootReference.Child(FBKEY_SCORES_PATH).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogWarning(task.Exception);
            
            List<HighScoreEntry> scoreList = new List<HighScoreEntry>();

            int scoreListPosition = 0;
            
            foreach (var item in task.Result.Children)
            {
                string jsonString = (string)item.Value;
                scoreList.Insert(scoreListPosition,  JsonUtility.FromJson<HighScoreEntry>(jsonString));
                scoreListPosition++;
            }
            
            onScoreFetchedDelegate(scoreList);
            });
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
}

[Serializable]
public class HighScoreEntry
{
    public string UserID;
    public string name = "";
    public int score;
    public string Date;
}