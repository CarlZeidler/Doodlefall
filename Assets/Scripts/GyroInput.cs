using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroInput : MonoBehaviour
{
    private Rigidbody rgbd;

    void Start()
    {
        Input.gyro.enabled = true;
        rgbd = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        GravityChange();
    }

    private void GravityChange()
    {
        float gravityX = Input.acceleration.x * 9.81f;
        float gravityY = Input.acceleration.y * 9.81f;
        Physics.gravity = new Vector3(gravityX, gravityY, 0);
        rgbd.AddForce(Physics.gravity * rgbd.mass);
    }
}
