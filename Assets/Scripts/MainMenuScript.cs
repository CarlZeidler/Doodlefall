using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuScript : MonoBehaviour
{
    #region Serialized Fields
        [SerializeField] TMP_Text welcomeText;
        [SerializeField] TMP_InputField nameInputField;
        [SerializeField] Material woodMaterial;
        [SerializeField] Material metallicMaterial;
        [SerializeField] Slider colorSlider;
        [SerializeField] GameObject playerBall;
        [SerializeField] MeshRenderer playerBallMesh;
        [SerializeField] Material[] ballMaterials;
        [SerializeField] GameObject metallicButton;
        [SerializeField] GameObject woodenButton;
    #endregion
    
    private const string PLAYER_INFO_KEY = "PlayerName";
    private const string PLAYER_BALL_KEY_COLOR = "PlayerBallColor";
    private const string PLAYER_BALL_KEY_MATERIAL = "PlayerBallMaterial";
    private const string PLAYER_SAVE_JSON_KEY = "PlayerSaveInfoInJSON";

    private GameManager gameManager;
    private PlayerInfo playerInfo;

    private string playerName;
    private Color ballColor;
    private int ballType = 1;


    void Start()
    {
        playerInfo = new PlayerInfo();
        colorSlider.onValueChanged.AddListener(delegate { ChangeBallColor(); });
        nameInputField.onValueChanged.AddListener(delegate { SetPlayerName(nameInputField.text); SetWelcomeText(); });
        LoadPlayerInfo();
        SetWelcomeText();
        UISetup();
    }

    private void UISetup()
    {
        if (ballType == 2)
        {
            metallicButton.SetActive(false);
            woodenButton.SetActive(true);
        }
    }

    void Update()
    {
        SpinBall();
    }

    private void SpinBall()
    {
        playerBall.transform.Rotate(new Vector3(0, -0.1f, 0));
    }

    private void SetPlayerName(string name)
    {
        playerName = name;
        //PlayerPrefs.SetString(PLAYER_INFO_KEY, name);
    }

    private string GetPlayerName()
    {
        string name = PlayerPrefs.GetString(PLAYER_INFO_KEY);
        return name;
    }

    public void SavePlayerInfo()
    {
        playerInfo.playerName = playerName;
        playerInfo.ballType = ballType;
        playerInfo.ballColor = colorSlider.value;

        string jsonString = JsonUtility.ToJson(playerInfo);

        //PlayerPrefs.SetString(PLAYER_SAVE_JSON_KEY, jsonString);

        //PlayerPrefs.SetFloat(PLAYER_BALL_KEY_COLOR, colorSlider.value);
        //PlayerPrefs.SetInt(PLAYER_BALL_KEY_MATERIAL, ballType);
        
        //the default name string is a temporary fix until a profile selection system is implemented.
        string defaultName = "nada";

        SaveToFile(playerName + "save", jsonString);
        SaveToFile(defaultName, jsonString);
    }

    private void LoadPlayerInfo()
    {
        string loadedData;
        string defaultName = "nada";

        if (string.IsNullOrEmpty(playerName))
        {
            loadedData = LoadFromFile(defaultName);
        }
        else
        {
            loadedData = LoadFromFile(playerName + "save");
        }
        
        playerInfo = JsonUtility.FromJson<PlayerInfo>(loadedData);

        //PlayerInfo loadedTemp = JsonUtility.FromJson<PlayerInfo>(loadedData);

        playerName = playerInfo.playerName;
        ballType = playerInfo.ballType;
        colorSlider.value = playerInfo.ballColor;

        //ballType = PlayerPrefs.GetInt(PLAYER_BALL_KEY_MATERIAL);

        if (playerInfo.ballType == 0)
            ballType = 1;
        else
            ballType = playerInfo.ballType;
        //if (!PlayerPrefs.HasKey(PLAYER_BALL_KEY_MATERIAL))
        //    ballType = 1;
        if (ballType == 1)
            playerBallMesh.material = ballMaterials[0];
        else if (ballType == 2)
            playerBallMesh.material = ballMaterials[1];

        playerBallMesh.material.color = Color.HSVToRGB(colorSlider.value, 0.85f, 0.85f);

        //colorSlider.value = PlayerPrefs.GetFloat(PLAYER_BALL_KEY_COLOR);
        //playerBallMesh.material.color = Color.HSVToRGB(colorSlider.value, 0.85f, 0.85f);
    }

    private void SetWelcomeText()
    {
        if (string.IsNullOrEmpty(playerName))
        //if (!PlayerPrefs.HasKey(PLAYER_INFO_KEY))
        {
            welcomeText.SetText("Welcome new player!");
        }
        else
            welcomeText.SetText("Welcome " + playerName + "!");
    }

    private void ChangeBallColor()
    {
        ballColor = Color.HSVToRGB(colorSlider.value, 0.85f, 0.85f);
        playerBallMesh.material.color = ballColor;
        
    }

    public void ToggleBallMaterial()
    {
        if (ballType == 1)
        {
            ballType = 2;
            playerBallMesh.material = ballMaterials[1];
            colorSlider.value = 0;
        }
        else if (ballType == 2)
        {
            ballType = 1;
            playerBallMesh.material = ballMaterials[0];
            colorSlider.value = 0;
        }
    }
    private void SaveToFile(string fileName, string jsonString)
    {
        using (var stream = File.OpenWrite(fileName))
        {
            stream.SetLength(0);

            var bytes = Encoding.UTF8.GetBytes(jsonString);

            stream.Write(bytes, 0, bytes.Length);
        }
    }   

    private string LoadFromFile(string fileName)
    {
        using (var stream = File.OpenText(fileName))
        {
            return stream.ReadToEnd();
        }
    }
}