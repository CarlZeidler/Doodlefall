using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LoginScreenScript : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private GameObject loginCanvas;
    [SerializeField] private GameObject nameConfirmCanvas;
    [SerializeField] private GameObject playerSetupCanvas;
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField anonymousField;
    [SerializeField] private Button signInButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button anonymousSignInButton;
    #endregion
    
    private SaveManager saveManager;
    private GameManager gameManager;
    private MainMenuScript mainMenu;
    private PlayerStats playerStats;

    private void Update()
    {
         
    }

    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void StartSignIn()
    {
        SaveManager.Instance.UserSignIn(emailField.text, passwordField.text, SignInCallback);
        // saveManager.UserSignIn(emailField.text, passwordField.text, SignInCallback);
    }

    public void StartRegistration()
    {
        SaveManager.Instance.RegisterNewUser(emailField.text, passwordField.text, RegistrationCallback);
        // saveManager.RegisterNewUser(emailField.text, passwordField.text, RegistrationCallback);
    }

    public void StartAnonymousSignIn()
    {
        SaveManager.Instance.AnonymousSignIn(anonymousField.text, RegistrationCallback);
        // saveManager.AnonymousSignIn(anonymousField.text, RegistrationCallback);
    }

    public void StartRegisteringUserName()
    {
        SaveManager.Instance.RegisterUserName(usernameField.text, UserNameRegistrationCallback);
        // saveManager.RegisterUserName(usernameField.text, UserNameRegistrationCallback);
    }

    public void SaveDataLocally()
    {
        
    }
    
    public void ActivateButtons()
    {
        if (!string.IsNullOrEmpty(emailField.text) && !string.IsNullOrEmpty(passwordField.text))
        {
            signInButton.interactable = true;
            registerButton.interactable = true;
        }
        if (!string.IsNullOrEmpty(anonymousField.text))
            anonymousSignInButton.interactable = true;
    }

    private void RegistrationCallback(bool anonymousRegistration)
    {
        if (anonymousRegistration)
        {
            loginCanvas.SetActive(false);
            playerSetupCanvas.SetActive(true);            
        }
        else
        {
            loginCanvas.SetActive(false);
            nameConfirmCanvas.SetActive(true);
        }
    }

    private void UserNameRegistrationCallback()
    {
        nameConfirmCanvas.SetActive(false);
        playerSetupCanvas.SetActive(true);
        mainMenu.LoadPlayerInfo();
    }

    public void SignInCallback(DataSnapshot snapshot)
    {
        loginCanvas.SetActive(false);
        playerSetupCanvas.SetActive(true);
        
        string jsonString = JsonUtility.ToJson(snapshot);
        
        playerStats = JsonUtility.FromJson<PlayerStats>(jsonString);
        mainMenu.LoadPlayerInfo();
    }
}
