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

    private const string TAGKEY_PLAYER = "Player";
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger Exit");
        if (other.CompareTag("Player"))
        {
            TriggerExitEvent.Invoke();
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerEnterEvent.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && constantTrigger)
        {
            TriggerConstantEvent.Invoke();
        }
    }
}
