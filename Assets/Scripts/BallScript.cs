using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private const string PLAYER_INFO_KEY = "PlayerName";
    private const string PLAYER_BALL_KEY_COLOR = "PlayerBallColor";
    private const string PLAYER_BALL_KEY_MATERIAL = "PlayerBallMaterial";

    [SerializeField] MeshRenderer playerBallMesh;
    [SerializeField] Material[] ballMaterials;
    [SerializeField] private Transform boardStartTrigger;

    private PlayerInfo _playerInfo;
    
    private int ballType = 1;
    private Vector3 boardStartPos;
    
    
    void Start()
    {
        boardStartPos = boardStartTrigger.position;
        _playerInfo = FindObjectOfType<PlayerInfo>();
        LoadBallSettings();
    }

    void Update()
    {
        
    }

    private void LoadBallSettings()
    {
        ballType = _playerInfo.ballType;

        if (_playerInfo.ballType == 0)
            ballType = 1;
        if (ballType == 1)
            playerBallMesh.material = ballMaterials[0];
        else if (ballType == 2)
            playerBallMesh.material = ballMaterials[1];

        playerBallMesh.material.color = Color.HSVToRGB(_playerInfo.ballColor, 0.85f, 0.85f);
    }

    public void Respawn()
    {
        Debug.Log("Respawning....");
        transform.position = boardStartPos;
    }

    public void Death(float scoreToAdd)
    {
        
    }
    
    public void Scoreup(float scoreToAdd)
    {
        
    }
}
