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
    
    private FirebaseAuth auth;
    private FirebaseDatabase db;

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
    }

    public void RegisterNewUser(string email, string password, OnRegistrationDelegate onRegistrationDelegate)
    {
        Debug.Log("Starting user registration");
        // UserProfile newUserProfile = new UserProfile
        // {
        //     DisplayName = userName
        // };
        
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);
            else
            {
                FirebaseUser newUser = task.Result;
                // newUser.UpdateUserProfileAsync(newUserProfile).ContinueWithOnMainThread(task =>
                // {
                //     if (task.Exception != null)
                //         Debug.LogError(task.Exception);
                //     else
                //         Debug.LogFormat("Updated username: {0}", newUser.DisplayName);
                // });
                Debug.LogFormat("User registered: {0}, user ID: {1}", newUser.Email, newUser.UserId);
                onRegistrationDelegate.Invoke(false);
                
            }
        });
    }

    public void UserSignIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);
            else
            {
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in: {0}, user ID: {1}", newUser.DisplayName, newUser.UserId);
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
        } );
    }

    public void LoadFromFirebase(string path, OnLoadedDelegate onLoadedDelegate)
    {
        db.RootReference.Child("users").Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);

            onLoadedDelegate(task.Result);
        });
    }

    public void AnonymousSignIn(OnRegistrationDelegate onRegistrationDelegate)
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
            }
        });
    }

    public void RemoveDataFromFirebase(string path)
    {
        db.RootReference.Child(path).RemoveValueAsync();
    }
}
