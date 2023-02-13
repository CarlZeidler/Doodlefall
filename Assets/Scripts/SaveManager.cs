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
using UnityEditor;

public class SaveManager : MonoBehaviour
{
    private PlayerInfo _playerInfo;
    private StartupScreenScript _startupScript;
    
    //Singleton variables
    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get { return _instance; }
    }
    
    //Functions that gets called after load or save is completed
    public delegate void OnLoadedDelegate(string jsonString);
    public delegate void OnSaveDelegate();
    public delegate void OnRegistrationDelegate(bool anonymousRegistration, PlayerStats playerStats);
    public delegate void OnSigninDelegate(string jsonString);
    
    //Firebase namespaces
    private FirebaseAuth auth;
    private FirebaseDatabase db;

    //Key aliases
    const string FBKEY_USERS_PATH = "users";
    const string FBKEY_USERSDATA_PATH = "user_data";

    private void Start()
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
        
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _startupScript = FindObjectOfType<StartupScreenScript>();
    }

    #region USER REGISTRATION FUNCTIONS

    ////////////////////////////// USER REGISTRATION FUNCTIONS //////////////////////////////
    
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
                _playerInfo.userID = newUser.UserId;
                _playerInfo.playerEmail = newUser.Email;
                _playerInfo.playerIsAnonymous = false;
                Debug.LogFormat("local data saved: {0}, {1}, {2}", _playerInfo.userID, _playerInfo.playerEmail, _playerInfo.playerIsAnonymous.ToString());
                Debug.LogFormat("User registered: {0}, user ID: {1}", _playerInfo.playerEmail, _playerInfo.userID);
                
                Debug.LogFormat("Attempting to start data save: {0}", jsonString);
                db.RootReference.Child(FBKEY_USERS_PATH).Child(newUser.UserId).Child(FBKEY_USERSDATA_PATH)
                    .SetRawJsonValueAsync(jsonString);
                Debug.Log("User profile data created");

                onRegistrationDelegate.Invoke(false, playerStats);
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
                _playerInfo.playerID = auth.CurrentUser.DisplayName;
                _playerInfo.playerName = auth.CurrentUser.DisplayName;
                onSaveDelegate.Invoke();
            }
        });
    }

    public void AnonymousRegistrationIn(string playerName, OnRegistrationDelegate onRegistrationDelegate)
    {
        PlayerStats playerStats = new PlayerStats();
        
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);
            else
            {
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in anonymously: {0}, user ID: {1}", playerName, newUser.UserId);
                _playerInfo.playerName = playerName;
                _playerInfo.playerIsAnonymous = true;
                onRegistrationDelegate.Invoke(true, playerStats);
            }
        });
    }
    
    #endregion

    #region USER SIGN-IN FUNCTIONS
    ////////////////////////////// USER SIGN-IN FUNCTIONS //////////////////////////////
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
                _playerInfo.playerID = newUser.DisplayName;
                _playerInfo.playerName = _playerInfo.playerID;
                _playerInfo.userID = newUser.UserId;
                _playerInfo.playerEmail = newUser.Email;
                _playerInfo.playerIsAnonymous = false;
                
                db.RootReference.Child(FBKEY_USERS_PATH).Child(newUser.UserId).Child(FBKEY_USERSDATA_PATH)
                    .GetValueAsync().ContinueWithOnMainThread(task1 =>
                {
                    if (task.Exception != null)
                        Debug.LogWarning(task1.Exception);
                    else
                    {
                        string jsonString = task1.Result.GetRawJsonValue();
                        onSigninDelegate.Invoke(jsonString);
                    }
                });
            }
        });
    }

    #endregion

    #region Save/Load operations
    ////////////////////////////// SAVE/LOAD OPERATIONS FUNCTIONS //////////////////////////////
    public void SaveToFirebase(PlayerStats playerStats)
    {
        string jsonString = JsonUtility.ToJson(playerStats);
        
        db.RootReference.Child(FBKEY_USERS_PATH).Child(auth.CurrentUser.UserId).Child(FBKEY_USERSDATA_PATH)
            .SetRawJsonValueAsync(jsonString).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogWarning(task.Exception);
        });
    }

    public void LoadFromFirebase(OnLoadedDelegate onLoadedDelegate)
    {
        db.RootReference.Child(FBKEY_USERS_PATH).Child(auth.CurrentUser.UserId).Child(FBKEY_USERSDATA_PATH)
            .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);
            else
            {
                string jsonString = task.Result.GetRawJsonValue();
                onLoadedDelegate.Invoke(jsonString);
                _startupScript.SignInCallback(jsonString);
            }

        });
    }
    #endregion
    
    
    public void RemoveDataFromFirebase(string path)
    {
        db.RootReference.Child(path).RemoveValueAsync();
    }
}
