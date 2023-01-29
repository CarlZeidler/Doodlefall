using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZoneScript : MonoBehaviour
{
    private const string TAGKEY_PLAYER = "Player"; 
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TAGKEY_PLAYER))
        {
            other.gameObject.GetComponent<BallScript>().Respawn();
        }
    }
}
