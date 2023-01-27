using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
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
    
    private const string PLAYER_INFO_KEY = "PlayerName";
    private const string PLAYER_BALL_KEY_COLOR = "PlayerBallColor";
    private const string PLAYER_BALL_KEY_MATERIAL = "PlayerBallMaterial";
    private const string PLAYER_SAVE_JSON_KEY = "PlayerSaveInfoInJSON";

    private GameManager gameManager;
    private PlayerInfo playerInfo;
    private PlayerStats playerStats;

    private string playerName;
    private Color ballColor;
    private int ballType = 1;


    void Start()
    {
        playerInfo = FindObjectOfType<PlayerInfo>();
        colorSlider.onValueChanged.AddListener(delegate { ChangeBallColor(); });
        // LoadPlayerInfo();
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
        //This function spins the ball (Borpa was here)
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
        playerInfo.ballType = ballType;
        playerInfo.ballColor = colorSlider.value;

        string jsonString = JsonUtility.ToJson(playerInfo);
        string defaultName = "nada";

        SaveToFile(playerName + "save", jsonString);
        SaveToFile(defaultName, jsonString);
    }

    public void LoadPlayerInfo()
    {
        playerName = playerInfo.playerName;
        colorSlider.value = playerInfo.ballColor;
        
        if (playerInfo.ballType == 0)
            ballType = 1;
        else
            ballType = playerInfo.ballType;
        if (ballType == 1)
            playerBallMesh.material = ballMaterials[0];
        else if (ballType == 2)
            playerBallMesh.material = ballMaterials[1];

        playerBallMesh.material.color = Color.HSVToRGB(colorSlider.value, 0.85f, 0.85f);
    }

    private void SetWelcomeText()
    {
        welcomeText.SetText("Welcome " + playerInfo.playerName + "!");
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

    public void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}