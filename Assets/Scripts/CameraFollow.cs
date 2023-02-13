using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject tiltInstructionsText;
    
    public GameObject playerBall;
    private Vector3 cameraSpeed = Vector3.zero;
    private bool following;
    private float initialZPos;
    

    void Start()
    {
        initialZPos = transform.position.z;   
        MoveToStartPos();
        
    }

    void Update()
    {
        if (following)
            FollowBall();
    }

    private void MoveToStartPos()
    {
        transform.position = new Vector3(transform.position.x, playerBall.transform.position.y/2, transform.position.z);
    }

    private void FollowBall()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(playerBall.transform.position.x, playerBall.transform.position.y, (initialZPos - playerBall.transform.position.z) /1.7f);

        transform.position = Vector3.SmoothDamp(currentPos, targetPos, ref cameraSpeed, 0.3f);
    }

    public void StartFollowing()
    {
        following = true;
        tiltInstructionsText.SetActive(false);
    }
}
