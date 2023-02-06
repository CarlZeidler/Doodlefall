using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScorePanelScript : MonoBehaviour
{
    [SerializeField] private TMP_Text nameField;
    [SerializeField] private TMP_Text scoreField;
    [SerializeField] private TMP_Text dateField;

    public void updateFields(string name, int score, DateTime dateTime)
    {
        nameField.text = name;
        scoreField.text = score.ToString();
        dateField.text = dateTime.ToString();
    }
}
