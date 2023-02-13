using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public UnityEvent TriggerExitEvent;
    public UnityEvent TriggerEnterEvent;
    public UnityEvent TriggerConstantEvent;

    [SerializeField] private bool constantTrigger;

    private MeshRenderer mesh;
    private const string TAGKEY_PLAYER = "Player";

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TAGKEY_PLAYER))
        {
            TriggerExitEvent.Invoke();
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TAGKEY_PLAYER))
        {
            TriggerEnterEvent.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(TAGKEY_PLAYER) && constantTrigger)
        {
            TriggerConstantEvent.Invoke();
        }
    }
}
