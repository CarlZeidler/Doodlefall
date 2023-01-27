using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Unity.VisualScripting;
using UnityEditor;

public class SaveManager : MonoBehaviour
{
    private PlayerInfo playerInfo;
    private LoginScreenScript loginScript;
    
    //Singleton variables
    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get { return _instance; }
    }
    
    //Functions that gets called after load or save is completed
    public delegate void OnLoadedDelegate(DataSnapshot snapshot);
    public delegate void OnSaveDelegate();
    public delegate void OnRegistrationDelegate(bool anonymousRegistration);
    public delegate void OnSigninDelegate(DataSnapshot snapshot);
    
    //Firebase namespaces
    private FirebaseAuth auth;
    private FirebaseDatabase db;

    //Key aliases
    const string FBKEY_USERS_PATH = "users";
    const string FBKEY_USERSDATA_PATH = "user_data";
    
    private void Awake()
    {
        //Singleton setup
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);

            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseDatabase.DefaultInstance;
            db.SetPersistenceEnabled(false);
        });
        
        playerInfo = FindObjectOfType<PlayerInfo>();
        loginScript = FindObjectOfType<LoginScreenScript>();
    }

    public void RegisterNewUser(string email, string password, OnRegistrationDelegate onRegistrationDelegate)
    {
        Debug.Log("Starting user registration");

        PlayerStats playerStats = new PlayerStats();

        string jsonString = JsonUtility.ToJson(playerStats);
        
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);
            else
            {
                FirebaseUser newUser = task.Result;
                playerInfo.userID = newUser.UserId;
                playerInfo.playerEmail = newUser.Email;
                playerInfo.playerIsAnonymous = false;
                Debug.LogFormat("local data saved: {0}, {1}, {2}", playerInfo.userID, playerInfo.playerEmail, playerInfo.playerIsAnonymous.ToString());
                Debug.LogFormat("User registered: {0}, user ID: {1}", newUser.Email, newUser.UserId);
                
                Debug.LogFormat("Attempting to start data save: {0}", jsonString);
                db.RootReference.Child("users").Child(newUser.UserId)
                    .SetRawJsonValueAsync(jsonString);
                Debug.Log("User profile data created");
                onRegistrationDelegate.Invoke(false);
            }
        });
    }

    public void UserSignIn(string email, string password, OnSigninDelegate onSigninDelegate)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);
            else
            {
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in: {0}, user ID: {1}", newUser.DisplayName, newUser.UserId);
                playerInfo.playerID = newUser.DisplayName;
                playerInfo.playerName = playerInfo.playerID;
                playerInfo.userID = newUser.UserId;
                playerInfo.playerEmail = newUser.Email;
                playerInfo.playerIsAnonymous = false;

                db.RootReference.Child(FBKEY_USERS_PATH).Child(newUser.UserId).Child(FBKEY_USERSDATA_PATH)
                    .GetValueAsync().ContinueWithOnMainThread(task1 =>
                {
                    if (task.Exception != null)
                        Debug.LogWarning(task1.Exception);
                    else
                    {
                        onSigninDelegate.Invoke(task1.Result);
                    }
                });
            }
        });
    }
    
    public void SaveToFirebase(string path, string data, OnSaveDelegate onSaveDelegate = null)
    {
        db.RootReference.Child(path).SetRawJsonValueAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogWarning(task.Exception);
            
            onSaveDelegate?.Invoke();
        });
    }

    public void LoadFromFirebase(string path)
    {
        db.RootReference.Child(FBKEY_USERS_PATH).Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);

            loginScript.SignInCallback(task.Result);

        });
    }

    public void AnonymousSignIn(string playerName, OnRegistrationDelegate onRegistrationDelegate)
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);
            else
            {
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in anonymously: {0}, user ID: {1}", newUser.DisplayName, newUser.UserId);
                onRegistrationDelegate.Invoke(true);
                playerInfo.playerName = playerName;
                playerInfo.playerIsAnonymous = true;
            }
        });
    }

    public void RegisterUserName(string userName, OnSaveDelegate onSaveDelegate)
    {
        UserProfile newUserProfile = new UserProfile
        {
            DisplayName = userName
        };

        auth.CurrentUser.UpdateUserProfileAsync(newUserProfile).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);
            else
            {
                onSaveDelegate.Invoke();
                playerInfo.playerID = auth.CurrentUser.DisplayName;
                playerInfo.playerName = playerInfo.playerID;
            }
        });
    }
    
    public void RemoveDataFromFirebase(string path)
    {
        db.RootReference.Child(path).RemoveValueAsync();
    }
}
