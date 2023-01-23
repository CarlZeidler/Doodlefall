using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MainMenuScript : MonoBehaviour
{
    private const string PLAYER_INFO_KEY = "PlayerName";
    private const string PLAYER_BALL_KEY_COLOR = "PlayerBallColor";
    private const string PLAYER_BALL_KEY_MATERIAL = "PlayerBallMaterial";

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

    private Color ballColor;
    private int ballType = 1;

    void Start()
    {
        colorSlider.onValueChanged.AddListener(delegate { ChangeBallColor(); });
        nameInputField.onValueChanged.AddListener(delegate { SetPlayerName(nameInputField.text); SetWelcomeText(); });
        SetWelcomeText();
        LoadBallInfo();
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
        PlayerPrefs.SetString(PLAYER_INFO_KEY, name);
    }

    private string GetPlayerName()
    {
        string name = PlayerPrefs.GetString(PLAYER_INFO_KEY);
        return name;
    }

    public void SaveBallInfo()
    {
        PlayerPrefs.SetFloat(PLAYER_BALL_KEY_COLOR, colorSlider.value);
        PlayerPrefs.SetInt(PLAYER_BALL_KEY_MATERIAL, ballType);
    }

    private void LoadBallInfo()
    {
        ballType = PlayerPrefs.GetInt(PLAYER_BALL_KEY_MATERIAL);

        if (!PlayerPrefs.HasKey(PLAYER_BALL_KEY_MATERIAL))
            ballType = 1;
        if (ballType == 1)
            playerBallMesh.material = ballMaterials[0];
        else if (ballType == 2)
            playerBallMesh.material = ballMaterials[1];

        colorSlider.value = PlayerPrefs.GetFloat(PLAYER_BALL_KEY_COLOR);
        playerBallMesh.material.color = Color.HSVToRGB(colorSlider.value, 0.85f, 0.85f);
    }

    private void SetWelcomeText()
    {
        if (!PlayerPrefs.HasKey(PLAYER_INFO_KEY))
        {
            welcomeText.SetText("Welcome new player!");
        }
        else
            welcomeText.SetText("Welcome " + GetPlayerName() + "!");
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
}
