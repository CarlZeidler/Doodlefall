using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private TMP_Text scoreDisplay;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private HighScoreManager highScoreManager;
    
    private PlayerInfo playerInfo;
    private int currentScore = 0;
    
    void Start()
    {
        playerInfo = FindObjectOfType<PlayerInfo>();
        InitializeUI();
    }

    void Update()
    {
        
    }
    
    private void InitializeUI()
    {
        nameDisplay.text = playerInfo.playerName;
        scoreDisplay.text = currentScore.ToString();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1f;
    }
}
