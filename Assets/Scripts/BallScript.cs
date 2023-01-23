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

    private int ballType = 1;

    void Start()
    {
        LoadBallSettings();
    }

    void Update()
    {
        
    }

    private void LoadBallSettings()
    {
        ballType = PlayerPrefs.GetInt(PLAYER_BALL_KEY_MATERIAL);

        if (!PlayerPrefs.HasKey(PLAYER_BALL_KEY_MATERIAL))
            ballType = 1;
        if (ballType == 1)
            playerBallMesh.material = ballMaterials[0];
        else if (ballType == 2)
            playerBallMesh.material = ballMaterials[1];

        playerBallMesh.material.color = Color.HSVToRGB(PlayerPrefs.GetFloat(PLAYER_BALL_KEY_COLOR), 0.85f, 0.85f);
    }
}
