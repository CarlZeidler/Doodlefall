using System;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEditor.VersionControl;
using Random = UnityEngine.Random;

public class FirebaseTest : MonoBehaviour
{
    private FirebaseDatabase db;
    private FirebaseAuth auth;

    // private void Start()
    // {
    //     FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
    //         {
    //             if (task.Exception != null)
    //                 Debug.LogError(task.Exception);
    //
    //             db = FirebaseDatabase.DefaultInstance;
    //
    //             db.RootReference.Child("Hello").SetValueAsync("World!");
    //         }
    //     );
    // }

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);

            auth = FirebaseAuth.DefaultInstance;
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            AnonymousSignIn();

        if (Input.GetKeyDown(KeyCode.G))
            DataTest(auth.CurrentUser.UserId, Random.Range(0, 100).ToString());
    }

    public void RegisterNewUser(string email, string password)
    {
        Debug.Log("Starting registration");
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);
            else
            {
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("User registered {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            }
            
            
                
        });
    }

    public void SignIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);
            else
            {
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("User Signed in Successfully {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            }
        });
    }
    
    public void AnonymousSignIn()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogWarning(task.Exception);
            else
            {
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            }
        });
    }
    
    private void DataTest(string userID, string data)
    {
        Debug.Log("Trying to write data...");
        var db = FirebaseDatabase.DefaultInstance;
        db.RootReference.Child("users").Child(userID).SetValueAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogWarning(task.Exception);
            else
                Debug.Log("DataTestWrite: Complete");
        });
    }

}
