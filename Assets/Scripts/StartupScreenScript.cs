using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Database;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartupScreenScript : MonoBehaviour
{
    #region SERIALIZED FIELDS

    [SerializeField] private GameObject loginCanvas;
    [SerializeField] private GameObject nameConfirmCanvas;
    [SerializeField] private GameObject playerSetupCanvas;
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField anonymousField;
    [SerializeField] private TMP_Text welcomeText;
    [SerializeField] private Button signInButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button anonymousSignInButton;
    [SerializeField] private Slider colorSlider;
    [SerializeField] private GameObject playerBall;
    [SerializeField] private MeshRenderer playerBallMesh;
    [SerializeField] private GameObject metallicButton;
    [SerializeField] private GameObject woodenButton;
    [Header("Check Script for details")]
    [SerializeField] private Material[] ballMaterials; //MetallicMaterial1, Woodmaterial1
    
    #endregion
    
    private SaveManager _saveManager;
    private GameManager _gameManager;
    private MainMenuScript _mainMenu;
    private PlayerStats _playerStats;
    private PlayerInfo _playerInfo;

    private string _registeredUserJson;
    private Color _ballColor;
    private int _ballType = 1;
    private string _playerName;
    

    private void Start()
    {
        _saveManager = FindObjectOfType<SaveManager>();
        _gameManager = FindObjectOfType<GameManager>();
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _playerStats = new PlayerStats();
        colorSlider.onValueChanged.AddListener(delegate { ChangeBallColor();  });
    }
    
    private void Update()
    {
        SpinBall();
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("Logging in test user1");
            _saveManager.UserSignIn("testuser@testaddress.com", "testtest1", SignInCallback);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Debug.Log("Logging in test user2");
            _saveManager.UserSignIn("testuser2@testaddress.com", "testtest1", SignInCallback);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha7)) 
        {
            Debug.Log("Logging in test user3");
            _saveManager.UserSignIn("testuser3@testaddress.com", "testtest1", SignInCallback);
        }
    }

    private void SpinBall()
    {
        //Borpa was here
        playerBall.transform.Rotate(new Vector3(0, -0.1f, 0));
    }
    
    private void ChangeBallColor()
    {
        _ballColor = Color.HSVToRGB(colorSlider.value, 0.85f, 0.85f);
        playerBallMesh.material.color = _ballColor;
    }
    
    public void ToggleBallMaterial()
    {
        if (_ballType == 1)
        {
            _ballType = 2;
            playerBallMesh.material = ballMaterials[1];
            colorSlider.value = 0;
        }
        else if (_ballType == 2)
        {
            _ballType = 1;
            playerBallMesh.material = ballMaterials[0];
            colorSlider.value = 0;
        }
    }

    private void SetWelcomeText()
    {
        welcomeText.SetText("Welcome " + _playerInfo.playerName + "!");
    }
    
    private void UISetup()
    {
        if (_ballType == 2)
        {
            metallicButton.SetActive(false);
            woodenButton.SetActive(true);
        }
    }
    
    public void SaveDataToFirebase()
    {
        _playerStats.ballType = _ballType;
        _playerStats.ballColor = colorSlider.value;
        _saveManager.SaveToFirebase(_playerStats);
    }
    
    #region INITIAL SCREEN/SIGN-IN FUNCTIONS
    
    /////////////////// USER AUTHENTICATION/REGISTRATION/SIGN-IN FUNCTIONS ///////////////////
    
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
        SaveManager.Instance.AnonymousRegistrationIn(anonymousField.text, RegistrationCallback);
        // saveManager.AnonymousSignIn(anonymousField.text, RegistrationCallback);
    }

    public void StartRegisteringUserName()
    {
        SaveManager.Instance.RegisterUserName(usernameField.text, UserNameRegistrationCallback);
        // saveManager.RegisterUserName(usernameField.text, UserNameRegistrationCallback);
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
    private void RegistrationCallback(bool anonymousRegistration, PlayerStats playerStats)
    {
        _playerStats = playerStats;
        
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
        
        SavePlayerInfo();
    }

    public void SignInCallback(string jsonString)
    {
        loginCanvas.SetActive(false);
        playerSetupCanvas.SetActive(true);
        
        LoadPlayerInfo(jsonString);
    }
    
    public void SavePlayerInfo()
    {
        _playerStats.ballType = _ballType;      
        _playerStats.ballColor = colorSlider.value;
    }
    
    #endregion

    #region PLAYER SETUP FUNCTIONS
    
    ////////////////////////////// PLAYER SETUP FUNCTIONS //////////////////////////////
    
    
    public void LoadPlayerInfo(string jsonString)
    {
        Debug.Log("loaded json: " + jsonString);
        _playerStats = JsonUtility.FromJson<PlayerStats>(jsonString);
        Debug.Log("json converted to object: " + _playerStats);
        _playerName = _playerInfo.playerName;
        colorSlider.value = _playerStats.ballColor;
        
        SetWelcomeText();

        if (_playerStats.ballType == 0)
        {
            _ballType = 1;
            playerBallMesh.material = ballMaterials[0];
            _playerStats.ballType = _ballType;
        }
        else if (_playerStats.ballType == 1)
        {
            _ballType = _playerStats.ballType;
            playerBallMesh.material = ballMaterials[0];
        }
        else if (_playerStats.ballType == 2)
        {
            _ballType = _playerStats.ballType;
            playerBallMesh.material = ballMaterials[1];
        }
        
        UISetup();
        
        playerBallMesh.material.color = Color.HSVToRGB(colorSlider.value, 0.85f, 0.85f);
    }

    #endregion

    public void LoadGame()
    {
        float[] playerScore = { 0 };
        _playerInfo.CopyPlayerData(_ballType, colorSlider.value, playerScore);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
