using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class PlayerInfo : MonoBehaviour
{
    [HideInInspector] public string playerName, playerID, userID, playerEmail;
    [HideInInspector] public bool playerIsAnonymous;
    //playerName = the Player's chosen display name
    //playerID = the Player profile's associated username
    //userID = the Player profile's associated ID
    //playerEmail = the Player profile's associated Email
    //playerIsAnonymous = True if the player is anonymous and not a registered user
    [HideInInspector] public int ballType;
    [HideInInspector] public float ballColor;
    [HideInInspector] public float[] playerScore;
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}

