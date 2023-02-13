using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private const string TAGKEY_PLAYER = "Player";
    private MeshRenderer mesh;
    
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TAGKEY_PLAYER))
        {
            other.gameObject.GetComponent<BallScript>().Goal();
        }
    }
}