using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUp : MonoBehaviour
{
    private const string TAGKEY_PLAYER = "Player";
    private MeshRenderer mesh;
    public int scoreValue = 100;
    
    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.enabled = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TAGKEY_PLAYER))
        {
            other.gameObject.GetComponent<BallScript>().Scoreup(scoreValue);
        }
    }
}