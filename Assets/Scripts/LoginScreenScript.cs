using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject mainMenuCanvas;
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
        saveManager.UserSignIn(emailField.text, passwordField.text);
    }

    public void StartRegistration()
    {
        saveManager.RegisterNewUser(emailField.text, passwordField.text, RegistrationCallback);
    }

    public void StartAnonymousSignIn()
    {
        saveManager.AnonymousSignIn(RegistrationCallback);
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
            nameConfirmCanvas.SetActive(false);
            mainMenuCanvas.SetActive(false);            
        }
        else
        {
            loginCanvas.SetActive(false);
            nameConfirmCanvas.SetActive(true);
        }
    }
}
