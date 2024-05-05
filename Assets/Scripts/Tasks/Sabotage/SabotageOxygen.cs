using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SabotageOxygen : Keypad
{
    [SerializeField] TextMeshProUGUI displayCode;

    public override void ShowTask(Task task)
    {
        base.ShowTask(task);

        displayCode.text = "Today's code: " + code;
    }
    
}
