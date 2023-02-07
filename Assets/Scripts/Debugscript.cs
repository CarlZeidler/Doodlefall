using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth;

public class Debugscript : MonoBehaviour
{
    private HighScoreManager highScoreManager;

    private FirebaseAuth auth;
    
    private void Start()
    {
        highScoreManager = FindObjectOfType<HighScoreManager>();
        auth = FirebaseAuth.DefaultInstance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Loading score");
            highScoreManager.LoadScoreBoard();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Adding score to Firebase");
            highScoreManager.AddScoreToFirebase();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Logging in Anonymously");
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Exception != null)
                    Debug.LogError(task.Exception);
            });
        }
    }
}
