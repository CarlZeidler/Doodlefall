using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    #region Serialized Fields
        [SerializeField] TMP_Text welcomeText;
        [SerializeField] TMP_InputField nameInputField;
        [SerializeField] Slider colorSlider;
        [SerializeField] GameObject playerBall;
        [SerializeField] MeshRenderer playerBallMesh;
        [Header("Check Script for details")]
        [SerializeField] Material[] ballMaterials; //MetallicMaterial1, Woodmaterial1
        [SerializeField] GameObject metallicButton;
        [SerializeField] GameObject woodenButton;
    #endregion
    
    private GameManager _gameManager;
    private SaveManager _saveManager;
    private PlayerInfo _playerInfo;
    private PlayerStats _playerStats;

    private string _playerName;
    private Color _ballColor;
    private int _ballType = 1;


    private void Awake()
    {
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _saveManager = FindObjectOfType<SaveManager>();
        _playerStats = new PlayerStats();
        colorSlider.onValueChanged.AddListener(delegate { ChangeBallColor(); });
    }


    void Update()
    {
        SpinBall();
    }

    private void SpinBall()
    {
        //Borpa was here
        playerBall.transform.Rotate(new Vector3(0, -0.1f, 0));
    }

    public void SavePlayerInfo(PlayerStats incomingPlayerStats)
    {
        _playerStats = incomingPlayerStats;

        _playerStats.ballType = _ballType;      
        _playerStats.ballColor = colorSlider.value;
    }

    #region Sign-in operations
    ////////////////////////////// SIGN-UN OPERATIONS //////////////////////////////
    public void LoadPlayerInfo(DataSnapshot snapshot)
    {
        string jsonString = JsonUtility.ToJson(snapshot);

        _playerStats = JsonUtility.FromJson<PlayerStats>(jsonString);
        
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

    #endregion
    
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

    public void SaveDataToFirebase()
    {
        _playerStats.ballType = _ballType;
        _playerStats.ballColor = colorSlider.value;
        _saveManager.SaveToFirebase(_playerStats);
    }
    
    public void LoadGame()
    {
        float[] playerScore = {0};
        _playerInfo.CopyPlayerData(_ballType, colorSlider.value, playerScore);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}