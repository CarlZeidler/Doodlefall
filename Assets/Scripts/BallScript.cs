using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BallScript : MonoBehaviour
{
    [SerializeField] MeshRenderer playerBallMesh;
    [SerializeField] Material[] ballMaterials;
    [SerializeField] private Transform boardStartTrigger;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private TMP_Text deathScoreDisplay;
    [SerializeField] private GameObject goalPanel;
    [SerializeField] private TMP_Text goalScoreDisplay;
    
    private PlayerInfo playerInfo;
    private HighScoreManager highScoreManager;
    
    private int ballType = 1;
    private Vector3 boardStartPos;

    void Start()
    {
        boardStartPos = boardStartTrigger.position;
        playerInfo = FindObjectOfType<PlayerInfo>();
        highScoreManager = FindObjectOfType<HighScoreManager>();
        LoadBallSettings();
    }

    private void LoadBallSettings()
    {
        ballType = playerInfo.ballType;

        if (playerInfo.ballType == 0 || ballType == 1)
        {
            ballType = 1;
            playerBallMesh.material = ballMaterials[0];
        }
        else if (ballType == 2)
            playerBallMesh.material = ballMaterials[1];

        playerBallMesh.material.color = Color.HSVToRGB(playerInfo.ballColor, 0.85f, 0.85f);
    }

    public void Respawn()
    {
        highScoreManager.currentScore = 0;
        transform.position = boardStartPos;
        Time.timeScale = 1f;
    }

    public void Death()
    {
        Time.timeScale = 0f;
        deathPanel.SetActive(true);
        deathScoreDisplay.text = highScoreManager.currentScore.ToString();
        highScoreManager.AddScoreToFirebase();
    }

    public void Goal()
    {
        Time.timeScale = 0f;
        goalPanel.SetActive(true);
        goalScoreDisplay.text = highScoreManager.currentScore.ToString();
        highScoreManager.AddScoreToFirebase();
    }
    
    public void Scoreup(int scoreToAdd)
    {
        highScoreManager.UpdateCurrentScore(scoreToAdd);
    }
}
